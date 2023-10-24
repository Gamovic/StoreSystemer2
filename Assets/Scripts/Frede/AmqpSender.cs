using UnityEngine;
using RabbitMQ.Client;
using System.Text;

public class AmqpSender : MonoBehaviour
{
    //public string queueName = "Hello";
    public string queueName = "task_queue";


    // Start is called before the first frame update
    void Start()
    {
        // Opret forbindelse til RabbitMQ-serveren - med worker queue
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Declare a durable queue
        channel.QueueDeclare(queue: queueName,
                            durable: true, // Make queue durable - durability
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

        const string message = "Hello Rabbitmq";
        var body = Encoding.UTF8.GetBytes(message);

        var properties = channel.CreateBasicProperties();
        //properties.Persistent = true; // Make message persistent - Persistance
        properties.DeliveryMode = 2; // Set message as persistent - Persistance

        channel.BasicPublish(exchange: string.Empty,
                            routingKey: queueName,
                            basicProperties: properties, // Set message as persistent
                            body: body);

        Debug.Log($" [x] Sent {message}");

        /*// Opret forbindelse til RabbitMQ-serveren - uden worker queue
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: queueName,
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);
        
        const string message = "Hello World!";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: string.Empty,
                            routingKey: queueName,
                            basicProperties: null,
                            body: body);
        Debug.Log($" [x] Sent {message}");*/

    }
}
