using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LoginResponse : MonoBehaviour
{
    public bool success;      // Indicates if the login was successful.
    public string message;    // A message from the server (e.g., for error feedback).
    public string token;      // The authentication token, if login is successful.


// Start is called before the first frame update
void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
