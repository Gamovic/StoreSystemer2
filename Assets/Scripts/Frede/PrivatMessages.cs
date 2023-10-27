using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class PrivatMessages : MonoBehaviour
{
    private RabbitMqManager rabbitMqManager;

    private void Start()
    {
        rabbitMqManager = new RabbitMqManager();
    }

    public void SendPrivateMessage(string recipient, string content)
    {
        // Add logic for sending private messages
        Message message = new Message
        {
            name = recipient,
            content = content
        };
        string jsonMessage = JsonUtility.ToJson(message);
        rabbitMqManager.SendToRabbitMQ(jsonMessage);
    }
}