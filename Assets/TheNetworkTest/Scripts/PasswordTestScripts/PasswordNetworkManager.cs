using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Text;

public class PasswordNetworkManager : MonoBehaviour
{
    
    [SerializeField] private GameObject passwordInputField;
    [SerializeField] private GameObject teamPickerUI;
    [SerializeField] private GameObject passwordEntryUI;
    [SerializeField] private GameObject leaveButton;
   
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void OnDestroy()
    {
        // Prevent error in the editor
        if (NetworkManager.Singleton == null) { return; }

        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    public void Host()
    {
        // Hook up password approval check
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();
    }

    public void Client()
    {
        // Set password ready to send to the server to validate
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(passwordInputField.GetComponent<TMP_InputField>().text);
        NetworkManager.Singleton.StartClient();
    }

    //public void Leave() //for reference if things go wrong
    //{
    //    //if we are a connected client then have the server disconnect us
    //    if (NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsHost){ 
    //        DisconnectClientServerRpc(NetworkManager.Singleton.LocalClientId);
    //        Debug.Log("The server disconnected the connectedclient");
    //        return;
    //    }
        
        

    //    if (NetworkManager.Singleton.IsHost)
    //    {
    //        NetworkManager.Singleton.Shutdown();
    //        NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
    //    }

    //    passwordEntryUI.SetActive(true);
    //    teamPickerUI.SetActive(false);
    //    leaveButton.SetActive(false);
    //}

    public void Leave()
    {
        ShutdownEveryoneServerRpc();
    }

    [ServerRpc]
    private void ShutdownEveryoneServerRpc()
    {
        ShutdownEveryoneClientRpc();  
    }

    [ClientRpc]
    private void ShutdownEveryoneClientRpc()
    {
        //if host, do these things
        //if (NetworkManager.Singleton.IsHost)
        //{
            Debug.Log("The host is cleaning up");
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        //}
        Debug.Log("The host or a client is setting active states");
        //everyone does this clean up
        passwordEntryUI.SetActive(true);
        teamPickerUI.SetActive(false);
        leaveButton.SetActive(false);

    }

    private void HandleServerStarted()
    {
        // Temporary workaround to treat host as client
        if (NetworkManager.Singleton.IsHost)
        {
            //HandleClientConnected(NetworkManager.Singleton.ServerClientId);
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        //if (!this.GetComponent<NetworkObject>().IsSpawned)
        //{
        //    Debug.Log("We aren't spawned right now");
        //    SpawnNetworkObjectServerRpc(this.gameObject);
        //}
        // Are we the client that is connecting?
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            passwordEntryUI.SetActive(false);
            leaveButton.SetActive(true);
            teamPickerUI.SetActive(true);
        }
        
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        Debug.Log("A client disconnected");
        // Are we the client that is disconnecting?
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            passwordEntryUI.SetActive(true);
            leaveButton.SetActive(false);
            teamPickerUI.SetActive(false);
        }
    }

    

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        string password = Encoding.ASCII.GetString(connectionData);

        bool approveConnection = password == passwordInputField.GetComponent<TMP_InputField>().text;

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        switch (NetworkManager.Singleton.ConnectedClients.Count)
        {
            case 0:
                spawnPos = new Vector3(1f, 0f, 0f);
                spawnRot = Quaternion.Euler(0f, 0f, 0f);
                break;
            case 1:
                spawnPos = new Vector3(15f, 0f, 0f);
                spawnRot = Quaternion.Euler(0f, 0f, 0f);
                break;
            //case 2:
            //    spawnPos = new Vector3(2f, 0f, 0f);
            //    spawnRot = Quaternion.Euler(0f, 0f, 0f);
            //    break;
        }

        callback(true, null, approveConnection, spawnPos, spawnRot);

        
    }
}



