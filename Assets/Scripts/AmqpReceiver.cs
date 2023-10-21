using UnityEngine;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

public class AmqpReceiver : MonoBehaviour
{
    [Header("Queue Settings")]
    public string queueName = "Hello"; // Queue name to receive messages from

    private IModel channel;
    private EventingBasicConsumer consumer;

    void Start()
    {
        var factory = new ConnectionFactory() {HostName = "localhost" };
        var connection = factory.CreateConnection();
        channel = connection.CreateModel();

        channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);
            Debug.Log($" [x] Received {message}");
        };

        // Start consuming messages fromm the specified queue
        channel.BasicConsume(queue: queueName, 
                            noAck: true, 
                            consumer: consumer);
    }

    // Clean up when the Monobehaviour is destroyed
    private void OnDestroy()
    {
        if (channel != null)
        {
            channel.Close();
        }
    }
}
