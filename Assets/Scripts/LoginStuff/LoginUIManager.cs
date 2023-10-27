using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UIElements;
using TMPro;

public class LoginUIManager : MonoBehaviour
{

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public UnityEngine.UI.Button loginButton;
    public TMP_Text feedbackText;

    private const string loginUrl = /*"https://your-api-url.com/login"*/ "https://localhost:5001/api/login/login"; // Replace with your API endpoint.

    public void Start()
    {
        // Attach a method to the "Log In" button's click event.
        loginButton.onClick.AddListener(OnLoginButtonClick);
    }

    private void OnLoginButtonClick()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        Debug.Log("LoginButtonClicked");

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
            feedbackText.text = "Login failed. Please try again.";

            Debug.Log("Error: " + www.error);
            Debug.Log("Result: " + www.result);
        }
        else
        {
            // Handle the response here.
            string responseText = www.downloadHandler.text;
            LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(responseText);

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
    }
}
