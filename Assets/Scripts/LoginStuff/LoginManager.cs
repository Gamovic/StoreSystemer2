using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public UnityEngine.UI.Button loginButton;
    public TMP_Text feedbackText;

    private const string loginUrl = "https://localhost:5001/login"; // tmp skal være API endpoint

    public void Start()
    {
        // Attach a method to the "Log In" button's click event.
        loginButton.onClick.AddListener(LoginButtonClicked);
    }


    private void LoginButtonClicked()
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

        if (/*www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError*/ www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Login request error: " + www.error);
            feedbackText.text = "Login failed. Please try again.";
        }
        else
        {
            string responseText = www.downloadHandler.text;

            LoginArray loginArray = JsonHelper.FromJson<LoginArray>(responseText);


            //try
            //{
            //    LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(responseText);

            //    // Check if deserialization was successful
            //    if (loginResponse != null)
            //    {
            //        if (loginResponse.success)
            //        {
            //            feedbackText.text = "Login successful!";
            //            // Store the token securely and transition to the game scene.
            //        }
            //        else
            //        {
            //            feedbackText.text = "Invalid credentials. Please try again.";
            //        }
            //    }
            //    else
            //    {
            //        feedbackText.text = "Invalid response from the server.";
            //    }
            //}
            //catch (System.Exception e)
            //{
            //    Debug.LogError("Deserialization error: " + e.Message);
            //    feedbackText.text = "Error during deserialization. Please try again.";
            //}


            LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(responseText);

            Debug.Log(loginResponse);

            if (loginResponse.token != null)
            {
                feedbackText.text = "Login successful!";
            }

            else
            {
                feedbackText.text = "Invalid credentials. Please try again.";
            }

            //if (loginResponse.success)
            //{
            //    feedbackText.text = "Login successful!";
            //    // Store the token securely and transition to the game scene.
            //}
            //else
            //{
            //    feedbackText.text = "Invalid credentials. Please try again.";
            //}
        }


        //StartCoroutine(PostLogin(jsonLogin));

    }


    IEnumerator PostLogin(string loginJson)
    {
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(loginUrl, loginJson))
        {
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(loginJson));
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
                string responseText = request.downloadHandler.text;
                LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(responseText);

                Debug.Log(loginResponse);

                //if (loginResponse.success)
                //{
                //    feedbackText.text = "Login successful!";
                //}
                //else
                //{
                //    feedbackText.text = "Invalid credentials. Please try again.";
                //}
            }
        }
    }


    //OLD

    //private IEnumerator SendLoginRequest(string username, string password)
    //{
    //    WWWForm form = new WWWForm();
    //    form.AddField("username", username);
    //    form.AddField("password", password);

    //    UnityWebRequest www = UnityWebRequest.Post(loginUrl, form);

    //    www.SetRequestHeader("Content-Type", "application/json");

    //    yield return www.SendWebRequest();

    //    if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
    //    {
    //        Debug.LogError("Login request error: " + www.error);
    //    }
    //    else
    //    {
    //        // Handle the response here.
    //        string responseText = www.downloadHandler.text;
    //        LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(responseText);

    //        if (loginResponse.success)
    //        {
    //            feedbackText.text = "Login successful!";
    //            // Store the token securely and transition to the game scene.
    //        }
    //        else
    //        {
    //            feedbackText.text = "Invalid credentials. Please try again.";
    //        }
    //    }
    //}



    [System.Serializable]
    public class Login
    {
        public string Id;
        public string Username;
        public string Password;
        public string TimeStamp;
    }


    [System.Serializable]
    public class LoginArray
    {
        public Login[] logins;
    }

}
