using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using System.Text;

public class PasswordNetworkManager : MonoBehaviour
{
    
    [SerializeField] private GameObject passwordInputField;
    [SerializeField] private GameObject teamPickerUI;
    [SerializeField] private GameObject passwordEntryUI;
    [SerializeField] private GameObject leaveButton;
    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private GameObject uiMain;
    

    private static ulong hostClientId;//our host client
    private static ulong player2ClientId;// our connected client aka player 2
    private static NetworkVariable<byte> playerCount = new NetworkVariable<byte>();
   
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
       
    }

    private void Update()
    {
        if(NetworkManager.Singleton.IsHost && uiMain.GetComponent<LoadingAndWaitingScreen>().GetCurrentMessage(2) && NetworkManager.Singleton.ConnectedClients.Count == 1)
        {
                uiMain.GetComponent<LoadingAndWaitingScreen>().SetScreenMessage(1);        
        }
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

   

    public void Leave()
    {
        ShutdownEveryoneServerRpc();
    }

    [ServerRpc(RequireOwnership =false)]
    private void ShutdownEveryoneServerRpc()
    {
        ShutdownEveryoneClientRpc();  
    }

    [ClientRpc]
    private void ShutdownEveryoneClientRpc()
    {
        NetworkManager.Singleton.Shutdown();
        NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        //}
        //Debug.Log("The host or a client is setting active states");

        //everyone does this clean up
        passwordEntryUI.SetActive(true);
        teamPickerUI.SetActive(false);
        leaveButton.SetActive(false);
        playerCanvas.SetActive(false);
        //if (player2ClientId == NetworkManager.Singleton.LocalClientId)
        //{
        //    uiMain.GetComponent<LoadingAndWaitingScreen>().AdjustClientScreenMessageServerRpc(-1, hostClientId);
        //}

    }

    private void HandleServerStarted()
    {
        // Temporary workaround to treat host as client
        //if (NetworkManager.Singleton.IsHost)
        //{
        //    //HandleClientConnected(NetworkManager.Singleton.ServerClientId);
        //}
    }

    private void HandleClientConnected(ulong clientId)
    {
        // Are we the client that is connecting?
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            
            passwordEntryUI.SetActive(false);
            leaveButton.SetActive(true);
            teamPickerUI.SetActive(true);
            playerCanvas.SetActive(true);

            //If we are the host we want to set our screen active and text to waiting text
            if (NetworkManager.Singleton.IsHost)
            {
                //Debug.Log("we ran host screen check");
                //set waiting for player message for host
                uiMain.GetComponent<LoadingAndWaitingScreen>().SetScreenMessage(1);
                //no reason for this to fail that i can think of
                
                hostClientId = NetworkManager.Singleton.LocalClientId;
                return;
            }

            //if we are the client we want to set all screens to prepping message texxt
            if (NetworkManager.Singleton.IsClient)
            {

                //Debug.Log("we ran the client check for screens");
                //player prep message for player2
                uiMain.GetComponent<LoadingAndWaitingScreen>().SetScreenMessage(2);
                //player prep message for host
                uiMain.GetComponent<LoadingAndWaitingScreen>().AdjustClientScreenMessageServerRpc(2, hostClientId);
                
                player2ClientId = NetworkManager.Singleton.LocalClientId;
            }
            


        }
        
        
    }

    private void HandleClientDisconnect(ulong clientId)
    {
       
        // Are we the client that is disconnecting?
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            passwordEntryUI.SetActive(true);
            leaveButton.SetActive(false);
            teamPickerUI.SetActive(false);
            playerCanvas.SetActive(false);
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
        //int clientIndex = NetworkManager.Singleton.ConnectedClients.Count;
        //playerCanvas.GetComponent<AdjustPlayerUI>().MovePlayerName(clientIndex);

        callback(true, null, approveConnection, spawnPos, spawnRot);
        
        
    }
}



