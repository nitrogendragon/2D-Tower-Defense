using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Text;

public class PasswordNetworkManager : MonoBehaviour
{
    [SerializeField] private InputField passwordInputField;
    [SerializeField] private GameObject passwordEntryUI;
    [SerializeField] private GameObject leaveButton;
    [SerializeField] private GameObject localGameObjects;//basically the container for all the non network objects

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void OnDestroy()
    {
        if(NetworkManager.Singleton == null) { return; }//prevent editor error
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

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
        NetworkManager.Singleton.Shutdown();

        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
        Debug.Log("Left");

        passwordEntryUI.SetActive(true);
        leaveButton.SetActive(false);
        localGameObjects.SetActive(false);
    }

    private void HandleClientConnected(ulong clientId)
    {
        if(clientId == NetworkManager.Singleton.LocalClientId)
        {
            passwordEntryUI.SetActive(false);
            leaveButton.SetActive(true);
            localGameObjects.SetActive(true);//turn on the local game elements basically
        }
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            passwordEntryUI.SetActive(true);
            leaveButton.SetActive(false);
            localGameObjects.SetActive(false);//leaving the game so don't want the local GameObjects to run/be active
        }
    }

    private void HandleServerStarted() 
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HandleClientConnected(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientID, NetworkManager.ConnectionApprovedDelegate callback)
    {
        string password = Encoding.ASCII.GetString(connectionData);

        bool approveConnection = password == passwordInputField.text;

        Vector3 spawnPos = Vector3.zero;

        switch (NetworkManager.Singleton.ConnectedClients.Count)
        {
            case 0:
                spawnPos = new Vector3(-2f, 0f, 0f);
                break;
            case 1:
                spawnPos = new Vector3(0f, 0f, 0f);
                break;
            case 2:
                spawnPos = new Vector3(2f, 0f, 0f);
                break;
            

        }

        callback(true, null, approveConnection, spawnPos, null);
    }
}



