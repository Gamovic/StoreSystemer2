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
    //public SessionSystem sessionSystem;

    public string apiUrl = "https://localhost:5001/messages";
    public TMP_Text messageText;

    public string rabbitMqHost = "localhost";
    public string rabbitMqUsername = "guest";
    public string rabbitMqPassword = "guest";
    public string rabbitMqExchange = "direct_logs";
    public string rabbitMqRoutingKey = "black";
    public bool msgIsSent = false;

    private IConnection connection;
    private IModel channel;

    public event Action<string> MessageReceived;

    public MsgConsumerAPI()
    {
    }

    void Start()
    {
        Debug.Log("MsgConsumerAPI Start() called.");

        InitializeRabbitMQ();

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

                /*// Display first message from API
                if (messageArray != null && messageArray.messages != null && messageArray.messages.Length > 0)
                {
                    // Display message received from API
                    messageText.text = messageArray.messages[0].content;

                    // Send received message to RabbitMQ
                    SendToRabbitMQ(messageArray.messages[0].content);
                }*/
                // Display latest message from API
                if (messageArray != null && messageArray.messages != null && messageArray.messages.Length > 0 && msgIsSent == true)
                {
                    // Display the latest message received from the API
                    string latestMessageContent = messageArray.messages[messageArray.messages.Length - 1].content;
                    messageText.text = latestMessageContent;

                    // Send the latest received message to RabbitMQ
                    SendToRabbitMQ(latestMessageContent);

                    //msgIsSent = false;
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

    public void InitializeRabbitMQ()
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

    /*public void SubscribeToPrivateMessages(string playerName)
    {
        // Implement subscription to private messages
        // You should create a queue for the player to receive private messages.
        // Here is a simplified example of how you might do it:

        string privateQueueName = playerName + "private";
        channel.QueueDeclare(queue: privateQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queue: privateQueueName, exchange: rabbitMqExchange, routingKey: playerName);
    }

    public void UnsubscribeFromPrivateMessages(string playerName)
    {
        // Implement unsubscription from private messages
        // You should delete or unbind the player's private queue.
        // Here is a simplified example:

        string privateQueueName = playerName + "private";
        channel.QueueDelete(privateQueueName);
    }*/

    /*public void SendPrivateMessage(string targetPlayer, string message)
    {
        // Implement sending a private message
        // Publish the message to the target player's private queue.
        // Here is a simplified example:

        if (!string.IsNullOrEmpty(targetPlayer))
        {
            string privateQueueName = targetPlayer + "private";
            byte[] body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "", routingKey: privateQueueName, basicProperties: null, body: body);
        }
        else
        {
            Debug.LogWarning("Target player name is invalid.");
        }

        /*string privateQueueName = targetPlayer + "_private";
        byte[] body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "", routingKey: privateQueueName, basicProperties: null, body: body);
    }*/

    /*public void SendPublicMessage(string message)
    {
        // Implement sending a public message
        // Publish the message to a public channel that all players can subscribe to.
        // Here is a simplified example:

        byte[] body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "public_exchange", routingKey: "", basicProperties: null, body: body);

        /*
        byte[] body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: rabbitMqExchange, routingKey: "public", basicProperties: null, body: body);*/
    //}

    /*public void SendGroupMessage(string groupName, string message)
    {
        // Implement sending a group message
        // Publish the message to a group-specific channel.
        // You should have a mechanism to map group names to exchange names.
        // Here is a simplified example:

        if (!string.IsNullOrEmpty(groupName))
        {
            string groupExchangeName = "group_" + groupName;
            byte[] body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: groupExchangeName, routingKey: "", basicProperties: null, body: body);
        }
        else
        {
            Debug.LogWarning("Group name is invalid.");
        }

        /*string groupExchangeName = "group_" + groupName;
        byte[] body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: groupExchangeName, routingKey: "", basicProperties: null, body: body);
    }*/
    private void OnMessageReceived(string message)
    {
        Debug.Log("Received message: " + message);
        msgIsSent = true;
        messageText.text = message; // Update the messageText with the received message content.
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
