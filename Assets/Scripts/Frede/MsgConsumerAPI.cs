using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class MsgConsumerAPI : MonoBehaviour
{
    public string apiUrl = "https://localhost:5001/messages";
    public TMP_Text messageText;
    //public TextMeshProUGUI contentText;

    void Start()
    {
        StartCoroutine(GetDataFromAPI());
    }

    IEnumerator GetDataFromAPI()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
        {
            // Handle response data
            string responseJson = request.downloadHandler.text;
            MessageArray messageArray = JsonHelper.FromJson<MessageArray>(responseJson);

            if (messageArray != null && messageArray.messages != null && messageArray.messages.Length > 0)
            {
                messageText.text = messageArray.messages[0].content;
            }
            else
            {
                Debug.LogError("No messages found in the API response.");
            }
        }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
        /*using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                string response = request.downloadHandler.text;
                // Process and use the response data
                Debug.Log("API Response: " + response);
            }
        }*/
    }
}

[System.Serializable]
public class Message
{
    public string id;
    public string name;
    public string content;
    public string timeStamp;
}

/*[System.Serializable]
public class MessageList
{
    public List<Message> messages;
}*/

[System.Serializable]
public class MessageArray
{
    public Message[] messages;
}

public class JsonHelper
{
    public static T FromJson<T>(string json)
    {
        // Wrap the JSON array with an object
        string wrappedJson = "{ \"messages\": " + json + " }";
        return JsonUtility.FromJson<T>(wrappedJson);
    }
}
