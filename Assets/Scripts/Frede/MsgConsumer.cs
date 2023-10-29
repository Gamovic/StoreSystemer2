using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using TMPro;

public class MsgConsumer : MonoBehaviour
{
    public string queueName = "receive_msg_queue";

    public TMP_Text messageText;

    private IModel channel;
    private EventingBasicConsumer consumer;

    void Start()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = factory.CreateConnection();
        channel = connection.CreateModel();

        channel.QueueDeclare(queue: queueName,
                            durable: true, // Make sure the queue is declared as durable
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

        consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);
            Debug.Log($" [x] Received {message}");

            // Use the UnityMainThreadDispatcher to update the messageText
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                messageText.text = message;
            });

            // Acknowledge message
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        // Set the prefetch count to limit the number of unacknowledged messages per consumer
        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        // Start consuming messages from the specified queue
        channel.BasicConsume(queue: queueName,
                            noAck: false,
                            consumer: consumer);
    }

    private void OnDestroy()
    {
        if (channel != null)
        {
            channel.Close();
        }
    }
}
