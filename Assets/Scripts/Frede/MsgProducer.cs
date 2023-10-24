using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class MsgProducer : MonoBehaviour
{
    public string apiUrl = "https://localhost:5001/messages";
    public TMP_InputField nameInput;
    public TMP_InputField messageInput;
    public Button sendButton;

    private void Start()
    {
        sendButton.onClick.AddListener(SendNewMessage);
    }

    public void SendNewMessage()
    {
        string newName = nameInput.text;
        string newMessage = messageInput.text;

        // Create new message object
        Message message = new Message
        {
            name = newName,
            content = newMessage
        };

        string jsonMessage = JsonUtility.ToJson(message);

        StartCoroutine(PostMessage(jsonMessage));
    }

    IEnumerator PostMessage(string messageJson)
    {
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(apiUrl, messageJson))
        {
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(messageJson));
            request.downloadHandler = new DownloadHandlerBuffer();

            // Set the content type header to indicate JSON data
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Message sent successfully!");
            }
        }
    }
}