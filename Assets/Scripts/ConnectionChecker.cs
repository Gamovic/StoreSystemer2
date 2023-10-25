using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConnectionChecker : MonoBehaviour
{
    private NetworkManager m_NetworkManager;

    private void Start()
    {
        m_NetworkManager = GetComponentInParent<NetworkManager>();
        if (m_NetworkManager != null)
        {
            m_NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
            m_NetworkManager.ConnectionApprovalCallback = ApprovalCheck;
            m_NetworkManager.OnClientConnectedCallback += OnClientConnectedCallBack;

            //Setup();

        }

    }

    private void OnClientConnectedCallBack(ulong obj)
    {

    }

    private void Setup()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.StartServer();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var clientPass = System.Text.Encoding.ASCII.GetString(request.Payload);

        if (clientPass == "room password")
        {
            response.CreatePlayerObject = true;
            print("rifght! passs!!!");
            response.Approved = true;
            response.Reason = "correct pass";

        }
        else
        {
            response.Approved = false;
            print("wroing passs!!!");
            response.Reason = "incorrect pass";
        }

    }

    private void OnClientDisconnectCallback(ulong obj)
    {
        if (!m_NetworkManager.IsServer && m_NetworkManager.DisconnectReason != string.Empty)
        {
            print("and should be here..");
            Debug.Log($"Approval Declined Reason: {m_NetworkManager.DisconnectReason}");
        }
    }


}
