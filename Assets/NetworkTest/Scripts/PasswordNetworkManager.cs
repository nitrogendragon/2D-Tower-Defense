using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System;
using System.Text;

public class PasswordNetworkManager : MonoBehaviour
{
    [SerializeField] private InputField passwordInputField;
    [SerializeField] private GameObject passwordEntryUI;
    [SerializeField] private GameObject leaveButton;


    public void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();
        Debug.Log("Hosting");
    }


    public void Client()
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(passwordInputField.text);//sends encoded password to managers config
        NetworkManager.Singleton.StartClient();
        Debug.Log("Client");
    }

    public void Leave()
    {
        Debug.Log("Left");
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientID, NetworkManager.ConnectionApprovedDelegate callback)
    {
        string password = Encoding.ASCII.GetString(connectionData);

        bool approveConnection = password == passwordInputField.text;
        callback(true, null, approveConnection, null, null);
    }
}



