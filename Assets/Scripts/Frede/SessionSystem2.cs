using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class SessionSystem2 : MonoBehaviour
{
    public MsgConsumerAPI consumer;
    public MsgProducer producer;
    public LoginManager loginManager;

    public TMP_Text notificationText;
    public Button privateMessageButton;
    public Button publicMessageButton;
    public Button groupMessageButton;

    public Button loginButton;
    public Button logoutButton;
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    private string currentPlayerName;
    private bool isLoggedIn = false;
    //private bool isAdmin = false;

    public TMP_InputField targetPlayerInput;
    public TMP_InputField messageInput;
    //public TMP_InputField publicMessageInput;
    //public TMP_InputField groupMessageInput;

    private string targetPlayerName;
    private string privateMessageContent;
    private string publicMessageContent;
    private string groupMessageContent;

    private void Start()
    {
        privateMessageButton.onClick.AddListener(SendPrivateMessage);
        publicMessageButton.onClick.AddListener(SendPublicMessage);
        groupMessageButton.onClick.AddListener(SendGroupMessage);

        consumer.MessageReceived += OnMessageReceived;
        //SetMessageButtonsActive(false);
    }

    private void HandleLogin(string enteredUsername, string enteredPassword)
    {
        
        
    }

    

    private void SetMessageButtonsActive(bool active)
    {
        privateMessageButton.interactable = active;
        publicMessageButton.interactable = active;
        groupMessageButton.interactable = active;
    }

    public string GenerateUniqueMessageName()
    {
        string uniqueName = RandomString(10);
        return uniqueName;
    }

    private string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[UnityEngine.Random.Range(0, s.Length)]).ToArray());
    }

    public void SendPrivateMessage()
    {
        string newName = GenerateUniqueMessageName();
        targetPlayerName = targetPlayerInput.text;
        string newMessage = messageInput.text;

        // Create new message object
        Message message = new Message
        {
            name = newName,
            content = newMessage
        };

        string jsonMessage = JsonUtility.ToJson(message);

        /*Debug.Log("Sending private message to: " + targetPlayerName);
        Debug.Log("Message content: " + privateMessageContent);*/

        if (isLoggedIn)
        {
            // Send a private message to the target player
            //consumer.SendPrivateMessage(targetPlayerName, privateMessageContent);

            // Sends message to API
            StartCoroutine(producer.PostMessage(jsonMessage));

            // Send message to RabbitMQ
            producer.SendToRabbitMQ(newMessage);

            consumer.messageText.text = newMessage;

            consumer.msgIsSent = true;
        }
        else
        {
            Debug.LogWarning("You must be logged in to send private messages.");
        }
    }

    public void SendPublicMessage()
    {
        string newName = GenerateUniqueMessageName();
        targetPlayerName = targetPlayerInput.text;
        string newMessage = messageInput.text;

        // Create new message object
        Message message = new Message
        {
            name = newName,
            content = newMessage
        };

        string jsonMessage = JsonUtility.ToJson(message);

        if (isLoggedIn)
        {
            /*if (IsValidAdmin(currentPlayerName))
            {*/
                // Send a public message (admin role required)
                //consumer.SendPublicMessage(publicMessageContent);

                // Sends message to API
                StartCoroutine(producer.PostMessage(jsonMessage));

                // Send message to RabbitMQ
                producer.SendToRabbitMQ(newMessage);

                consumer.messageText.text = newMessage;

                consumer.msgIsSent = true;
            /*}
            else
            {
                Debug.LogWarning("Only administrators can send public messages.");
            }*/
        }
        else
        {
            Debug.LogWarning("You must be logged in as Admin to send public messages.");
        }
    }

    public void SendGroupMessage()
    {
        string groupName = "Group1";
        groupMessageContent = messageInput.text;
        /*string groupName = "GroupName"; // Replace with the actual group name
        string message = "Your group message content";*/

        if (isLoggedIn)
        {
            // Send a group message
            //consumer.SendGroupMessage(groupName, groupMessageContent);
        }
        else
        {
            Debug.LogWarning("You must be logged in to send group messages.");
        }

        // You can implement your group message logic here

        /*// Example: Send a group message to the specified group
        string groupMessage = currentPlayerName + " says to group " + groupName + ": " + message;
        consumer.SendGroupMessage(groupName, groupMessage);*/
    }

    // Add a method to initiate the login process
    public void InitiateLogin()
    {
        loginManager.LoginButtonClicked();
    }

    // Update the session after successful login
    public void UpdateSession(string username)
    {
        currentPlayerName = username;
        isLoggedIn = true;
        notificationText.text = "Logged in as " + currentPlayerName;
    }

    private void OnMessageReceived(string message)
    {
        Debug.Log("Received message: " + message);
    }
}
