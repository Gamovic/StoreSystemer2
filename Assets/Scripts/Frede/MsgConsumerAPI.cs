using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class MsgConsumerAPI : MonoBehaviour
{
    public string apiUrl = "https://localhost:5001/messages";
    public TMP_Text messageText;
    
    public string rabbitMqHost = "localhost";
    public string rabbitMqUsername = "guest";
    public string rabbitMqPassword = "guest";
    public string rabbitMqExchange = "direct_logs";
    public string rabbitMqRoutingKey = "black";

    private IConnection connection;
    private IModel channel;

    public event Action<string> MessageReceived;

    public MsgConsumerAPI()
    {
        InitializeRabbitMQ();
    }

    void Start()
    {
        StartCoroutine(GetDataFromAPI());
    }

    IEnumerator GetDataFromAPI()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
        {
            // Handle response data
            string responseJson = request.downloadHandler.text;
            MessageArray messageArray = JsonHelper.FromJson<MessageArray>(responseJson);

            if (messageArray != null && messageArray.messages != null && messageArray.messages.Length > 0)
            {
                // Display message received from API
                messageText.text = messageArray.messages[0].content;

                // Send received message to RabbitMQ
                SendToRabbitMQ(messageArray.messages[0].content);
            }
            else
            {
                Debug.LogError("No messages found in the API response.");
            }
        }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
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
        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queue: queueName, exchange: rabbitMqExchange, routingKey: rabbitMqRoutingKey);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);
            MessageReceived?.Invoke(message);
        };

        channel.BasicConsume(queue: queueName, noAck: true, consumer: consumer);
    }

    private void SendToRabbitMQ(string messageJson)
    {
        byte[] body = Encoding.UTF8.GetBytes(messageJson);

        channel.BasicPublish(exchange: rabbitMqExchange, routingKey: rabbitMqRoutingKey, basicProperties: null, body: body);

        Debug.Log("Message sent to RabbitMQ successfully!");
    }

    public void CloseConectione()
    {
        channel.Close();
        connection.Close();
    }
}

[System.Serializable]
public class Message
{
    public string id;
    public string name;
    public string content;
    public string timeStamp;
}

/*[System.Serializable]
public class MessageList
{
    public List<Message> messages;
}*/

[System.Serializable]
public class MessageArray
{
    public Message[] messages;
}

public class JsonHelper
{
    public static T FromJson<T>(string json)
    {
        // Wrap the JSON array with an object
        string wrappedJson = "{ \"messages\": " + json + " }";
        return JsonUtility.FromJson<T>(wrappedJson);
    }
}
