using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GroupMessages : MonoBehaviour
{
    private RabbitMqManager rabbitMqManager;

    private void Start()
    {
        rabbitMqManager = new RabbitMqManager();
    }

    public void SendGroupMessage(string group, string content)
    {
        // Add logic for sending group messages
        Message message = new Message
        {
            name = group,
            content = content
        };
        string jsonMessage = JsonUtility.ToJson(message);
        rabbitMqManager.SendToRabbitMQ(jsonMessage);
    }
}
