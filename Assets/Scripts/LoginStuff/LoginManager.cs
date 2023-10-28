using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public UnityEngine.UI.Button loginButton;
    public TMP_Text feedbackText;

    public Canvas canvas;


    private const string loginUrl = "https://localhost:5001/login"; // tmp skal v�re API endpoint

    public void Start()
    {
        // Attach a method to the "Log In" button's click event.
        loginButton.onClick.AddListener(LoginButtonClicked);
    }


    public void LoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        Debug.Log("LoginButtonClicked");

       // SendLoginRequest(username, password);

        StartCoroutine(SendLoginRequest(/*username, password*/));
    }

    private IEnumerator SendLoginRequest(/*string username, string password*/)
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        //Create JSON object
        var loginData = new Login
        {
            Username = username,
            Password = password
        };

        string jsonLogin = JsonUtility.ToJson(loginData);

        //
        // Create the request
        UnityWebRequest www = new UnityWebRequest(loginUrl, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonLogin);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        // Set the Content-Type header to specify JSON data
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();


        string responseText = www.downloadHandler.text;



        if (www.responseCode == 200)
        {
            LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(responseText);

            if (loginResponse.token != null)
            {
                feedbackText.text = "Login successful!";
                // Handle successful login and token storage here

                NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("room password");
                NetworkManager.Singleton.StartClient();

                canvas.gameObject.SetActive(false);

            }
            else
            {
                feedbackText.text = "Invalid credentials. Please try again.";
            }
        }
        else if (www.responseCode == 401)
        {
            feedbackText.text = "Invalid credentials. Please try again.";
        }
        else
        {
            Debug.LogError("Login request error: " + www.error);
            feedbackText.text = "Login failed. Please try again.";
        }




    }
}


    [System.Serializable]
    public class Login
    {
        public string Id;
        public string Username;
        public string Password;
        public string TimeStamp;
    }





