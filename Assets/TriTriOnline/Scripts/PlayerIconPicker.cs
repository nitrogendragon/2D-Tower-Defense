using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerIconPicker : NetworkBehaviour
{
    private ulong localClientId;

    public void GetLocalClientIDAndUpdatePlayerIndex(int playerIndex)
    {
        localClientId = NetworkManager.Singleton.LocalClientId;//get our clients Id   
        SelectPlayerIndexServerRpc(playerIndex, localClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SelectPlayerIndexServerRpc(int playerIndex, ulong targetLocalClientId)
    {
        //Debug.Log(" We started the selectTeamS");
        //make sure we are the server
        if (IsLocalPlayer || !NetworkManager.Singleton.IsServer)
        {
            //Debug.Log("We are not the server");
            return;
        }
        // take the index passed and the localClientId from the client that pressed the button and have the server handle updating target Clients team
        SelectPlayerIndexClientRpc(playerIndex, targetLocalClientId);
    }

    [ClientRpc]
    private void SelectPlayerIndexClientRpc(int playerIndex, ulong localClientId)
    {
        if (!IsServer) { /*Debug.Log("Again, we are not the server... clientRpc function call this time")*/ ; return; }
        //Debug.Log("The host is running the function");

        //if we don't find the local client with our Id in the connectedClients return otw output our NetworkClient as networkClient
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient))
        {

            return;
        }


        //check if we have a TeamPlayer component on our networkClient's player object and output it as teamPlayer otw return  if we don't
        if (!networkClient.PlayerObject.TryGetComponent<PlayerProfileIcon>(out var playerProfileIconSetter))
        {
            return;
        }
        //Debug.Log("The host found this for teamPlayer: " + teamPlayer);

        //send a message to the server to set the local client's team
        playerProfileIconSetter.SetPlayerIndexServerRpc((byte)playerIndex);

        //Debug.Log("The host ran the setteamserverrpc");
    }



    
}
