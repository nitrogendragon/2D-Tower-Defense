using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Collections;

public class LoadingAndWaitingScreen : NetworkBehaviour
{
    [SerializeField] private Text loadingAndWaitingTextObject;
    [SerializeField] private GameObject messageScreen;
    //are we player 1? if not we will be player2
    private string[] textsToDisplay = new string[] 
    { "Loading...", "Waiting for Opponent...", "Players are Prepping for Battle..." };

    //the first time this is called we don't have things set up
    //so we can't just use the serverrpc below even though they are virtually the same
    public void SetScreenMessage(int messageIndex)
    {
        //just to make sure but really unnecessary, more for myself to look at
        if (IsClient)
        {
            //if we have a message change text otw disable
            if (messageIndex == -1) 
            {
                messageScreen.SetActive(false);
                return; 
            }
            //if we have a text for this index then change our text accordingly
            loadingAndWaitingTextObject.text = textsToDisplay[messageIndex];
            
        }


    }

    public bool GetCurrentMessage(int messageIndexToCompare)
    {
        return (loadingAndWaitingTextObject.text == textsToDisplay[messageIndexToCompare]);
    }

    //particularly necessary for when a client joins, only does anything on/for host
    [ServerRpc(RequireOwnership =false)]
    public void AdjustClientScreenMessageServerRpc(int messageIndex, ulong clientTargetId)
    {
        if (IsHost && NetworkManager.Singleton.LocalClientId != clientTargetId/*|| !NetworkManager.Singleton.IsServer*/)
        {

            if (messageIndex == -1)
            {

                messageScreen.SetActive(false);
                return;
            }
            //if we have a text for this index then change our text accordingly
            loadingAndWaitingTextObject.text = textsToDisplay[messageIndex];
            return;
        }
        //we are the host and want to update our screen now 
        if ( IsHost && NetworkManager.Singleton.LocalClientId == clientTargetId)
        {

            if(messageIndex == -1) {
                messageScreen.SetActive(false);
                return; 
            }
            //if we have a text for this index then change our text accordingly
            loadingAndWaitingTextObject.text = textsToDisplay[messageIndex];

            return;
        }
    }

    





}
