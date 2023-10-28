using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class MsgProducer : MonoBehaviour
{
    public SessionSystem2 sessionSystem;
    public string apiUrl = "https://localhost:5001/messages";
    //public TMP_InputField nameInput;
    public TMP_InputField messageInput;
    public Button sendButton;

    public string queueName = "receive_msg_queue";
    public string rabbitMqExchange = "direct_logs";
    public string rabbitMqRoutingKey = "black";

    private IConnection connection;
    private IModel channel;
    private const string rabbitMqHost = "localhost";
    private const string rabbitMqUsername = "guest";
    private const string rabbitMqPassword = "guest";

    private void Start()
    {
        sendButton.onClick.AddListener(SendNewMessage);
        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory
        {
            HostName = rabbitMqHost,
            UserName = rabbitMqUsername,
            Password = rabbitMqPassword
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.ExchangeDeclare(exchange: rabbitMqExchange, type: ExchangeType.Direct);
    }

    public void SendNewMessage()
    {
        string newName = sessionSystem.GenerateUniqueMessageName();
        string newMessage = messageInput.text;

        // Create new message object
        Message message = new Message
        {
            name = newName,
            content = newMessage
        };

        string jsonMessage = JsonUtility.ToJson(message);

        // Sends message to API
        StartCoroutine(PostMessage(jsonMessage));

        // Send message to RabbitMQ
        SendToRabbitMQ(newMessage);
    }

    public IEnumerator PostMessage(string messageJson)
    {
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(apiUrl, messageJson))
        {
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(messageJson));
            request.downloadHandler = new DownloadHandlerBuffer();

            // Set the content type header to indicate JSON data
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Message sent to API successfully!");
            }
        }
    }

    public void SendToRabbitMQ(string messageJson)
    {
        byte[] body = Encoding.UTF8.GetBytes(messageJson);

        // Declare a durable queue
        channel.QueueDeclare(queue: queueName, 
                            durable: false, // Make queue durable - durability
                            exclusive: false, 
                            autoDelete: false, 
                            arguments: null);

        channel.BasicPublish(exchange: rabbitMqExchange, 
                            routingKey: rabbitMqRoutingKey, 
                            basicProperties: null, 
                            body: body);

        Debug.Log("Message sent to RabbitMQ successfully!");
    }

    private void OnDestroy()
    {
        channel.Close();
        connection.Close();
    }
}