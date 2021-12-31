using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class TeamPicker : NetworkBehaviour
{
    private ulong localClientId;

    [ServerRpc(RequireOwnership = false)]
    public void SelectTeamServerRpc(int teamIndex, ulong targetLocalClientId)
    {
        Debug.Log(" We started the selectTeamS");
        //make sure we are the server
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.Log("We are not the server");
            return;
        }
        // take the index passed and the localClientId from the client that pressed the button and have the server handle updating target Clients team
        SelectTeamClientRpc(teamIndex, targetLocalClientId);
    }

    [ClientRpc]
    private void SelectTeamClientRpc(int teamIndex, ulong localClientId)
    {
        if (!IsServer) { Debug.Log("Again, we are not the server... clientRpc function call this time"); return; }
        Debug.Log("The host is running the function");
        //if we don't find the local client with our Id in the connectedClients return otw output our NetworkClient as networkClient
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient))
        {
      
            return;
        }
        Debug.Log("The host found this for networkClient " + networkClient);
        //check if we have a TeamPlayer component on our networkClient's player object and output it as teamPlayer otw return  if we don't
        if (!networkClient.PlayerObject.TryGetComponent<TeamPlayer>(out var teamPlayer))
        {
            return;
        }
        Debug.Log("The host found this for teamPlayer: " + teamPlayer);
        //send a message to the server to set the local client's team
        teamPlayer.SetTeamServerRpc((byte)teamIndex);
        Debug.Log("The host ran the setteamserverrpc");
    }

    

    public void GetLocalClientID(int teamId)
    {
        localClientId = NetworkManager.Singleton.LocalClientId;//get our clients Id   
        SelectTeamServerRpc(teamId, localClientId);
    }
}
