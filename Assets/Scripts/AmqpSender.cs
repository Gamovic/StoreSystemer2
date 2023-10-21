using UnityEngine;
using RabbitMQ.Client;
using System.Text;

public class AmqpSender : MonoBehaviour
{
    public string queueName = "Hello";

    // Start is called before the first frame update
    void Start()
    {
        // Opret forbindelse til RabbitMQ-serveren
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
        Debug.Log($" [x] Sent {message}");

        
        /*{
            // Opret en kanal og kø
            channel.QueueDeclare(queue: "my_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            string message = "Hello, RabbitMQ from Unity!";
            var body = Encoding.UTF8.GetBytes(message);

            // Publicer en besked til køen
            channel.BasicPublish(exchange: "", routingKey: "my_queue", basicProperties: null, body: body);
            Debug.Log($" [x] Sent: {message}");
        }*/
    }
}
