using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class PublicMessages : MonoBehaviour
{
    private RabbitMqManager rabbitMqManager;

    private void Start()
    {
        rabbitMqManager = new RabbitMqManager();
    }

    public void SendPublicMessage(string content)
    {
        // Add logic for sending public messages (e.g., only by administrators)
        Message message = new Message
        {
            name = "Administrator", // Or a specific admin's name
            content = content
        };
        string jsonMessage = JsonUtility.ToJson(message);
        rabbitMqManager.SendToRabbitMQ(jsonMessage);
    }
}