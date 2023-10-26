using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
    private const string loginUrl = "https://localhost:5001/api/login/login"; // tmp skal være API endpoint

    public void Login (string username, string password)
    {
        StartCoroutine(SendLoginRequest(username, password));
    }

    private IEnumerator SendLoginRequest(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(loginUrl, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Login request error: " + www.error);
        }
        else
        {
            // Handle the response here.
            string responseText = www.downloadHandler.text;
            Debug.Log("Login response: " + responseText);

            // Parse the JSON response if applicable.
            // You might receive a JWT or a success message.
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
