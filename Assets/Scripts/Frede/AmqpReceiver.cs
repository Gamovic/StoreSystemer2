using UnityEngine;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

public class AmqpReceiver : MonoBehaviour
{
    [Header("Queue Settings")]
    // Med worker queue
    public string queueName = "task_queue";

    private IModel channel;
    private EventingBasicConsumer consumer;

    private void Start()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = factory.CreateConnection();
        channel = connection.CreateModel();

        // Declare a durable queue
        channel.QueueDeclare(queue: queueName,
                            durable: true, // Make sure queue is declared as durable
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

        consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);
            Debug.Log($" [x] Received {message}");

            // Simulate a time-consuming task
            System.Threading.Thread.Sleep(2000);

            // Acknowledge message
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        // Set the prefetch count to limit the number of unacknowledged messages per consumer - Dispatch
        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        // Start consuming messages from the specified queue
        channel.BasicConsume(queue: queueName,
                            noAck: false,
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

    // Uden worker queue
    /*public string queueName = "Hello"; // Queue name to receive messages from

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
    }*/
}
