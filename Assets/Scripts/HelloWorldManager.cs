using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldManager : MonoBehaviour
    {

        public  LoginManager loginManager;
        public  Canvas canvas;


        void OnGUI()
        {

            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();

                SubmitNewPosition();
            }

            GUILayout.EndArea();
        }

         void StartButtons()
        {
            //if (GUILayout.Button("Client"))
            //{
            //    NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("room password");
            //    NetworkManager.Singleton.StartClient();
            //}

            if (GUILayout.Button("Host"))
            {
                NetworkManager.Singleton.StartHost();
                loginManager.gameObject.SetActive(false);
                canvas.gameObject.SetActive(false);
            }
       

            if (GUILayout.Button("Client"))
            {
                loginManager.gameObject.SetActive(true);
                canvas.gameObject.SetActive(true);
            }

            if (GUILayout.Button("Server"))
            {
                NetworkManager.Singleton.StartServer();
                loginManager.gameObject.SetActive(false);
                canvas.gameObject.SetActive(false);
            }
        }

        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

        static void SubmitNewPosition()
        {
            if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change"))
            {
                if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
                {
                    foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<HelloWorldPlayer>().Move();
                }
                else
                {
                    var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                    var player = playerObject.GetComponent<HelloWorldPlayer>();
                    player.Move();
                }
            }
        }
    }
}