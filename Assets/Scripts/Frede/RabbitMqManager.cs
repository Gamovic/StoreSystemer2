using System.Text;
using RabbitMQ.Client;

public class RabbitMqManager
{
    private IConnection connection;
    private IModel channel;
    private const string rabbitMqHost = "localhost";
    private const string rabbitMqUsername = "guest";
    private const string rabbitMqPassword = "guest";

    public RabbitMqManager()
    {
        InitializeRabbitMq();
    }

    private void InitializeRabbitMq()
    {
        var factory = new ConnectionFactory
        {
            HostName = rabbitMqHost,
            UserName = rabbitMqUsername,
            Password = rabbitMqPassword
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
    }

    public void SendToRabbitMQ(string messageJson)
    {
        byte[] body = Encoding.UTF8.GetBytes(messageJson);
        // Implement sending to RabbitMQ as needed.
    }

    public void CloseConnection()
    {
        channel.Close();
        connection.Close();
    }
}
