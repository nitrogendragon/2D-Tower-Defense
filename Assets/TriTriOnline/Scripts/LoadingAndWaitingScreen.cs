using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class LoadingAndWaitingScreen : NetworkBehaviour
{
    [SerializeField] private Text loadingAndWaitingTextObject;
    [SerializeField] private GameObject messageScreen;
    [SerializeField] private Button readyUpButton;
    [SerializeField] private TMP_Text readyUpText;
    [SerializeField] private GameObject UI_Password;//reference to our client's UI_Password gameobject
    [SerializeField] private GameObject boardManagerNetwork;
    [SerializeField] private GameObject cardsControllerNetwork;//on our UI_Main gameobject now
    [SerializeField] private GameObject cardsContainer;//empty container to hold our card and deck related objects
    [SerializeField] private GameObject endTurnButton;
    private Color initialButtonNormalColor;
   
    
    private string[] readyUpTexts = new string[] { "ready up", "ready" };
    //are we player 1? if not we will be player2
    private string[] textsToDisplay = new string[] 
    { "Loading...", "Waiting for Opponent...", "Players are Prepping for Battle..." };

    private NetworkVariable<int> playerReadyCount = new NetworkVariable<int>(0);

    private void Start()
    {
        //just initializing text to what it should be on start
        readyUpText.text = readyUpTexts[0];
        initialButtonNormalColor = readyUpButton.GetComponent<Button>().colors.normalColor;
    }

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

    public void ReadyUp(bool didWeLeave)
    {
        var colors = readyUpButton.GetComponent<Button>().colors;
        if(readyUpText.text == readyUpTexts[1] && didWeLeave && IsHost)
        {
            Debug.Log("We ran the host disable version");
            readyUpText.text = readyUpTexts[0];
            colors.normalColor = initialButtonNormalColor;
            colors.selectedColor = initialButtonNormalColor;
            readyUpButton.GetComponent<Button>().colors = colors;
            UI_Password.GetComponent<PasswordNetworkManager>().HandlePlayerReadyUp(false);
            if (IsLocalPlayer) { Debug.Log("Not local on unready"); return; }
            SetReadyPlayerCountToZeroServerRpc();
        }
        if (readyUpText.text == readyUpTexts[0] && !didWeLeave)
        {
            readyUpText.text = readyUpTexts[1];
            colors.normalColor = new Color(.5f, .8f, .2f);
            colors.selectedColor = colors.normalColor;
            //adjust our button colors for being readied up
            readyUpButton.GetComponent<Button>().colors = colors;
            UI_Password.GetComponent<PasswordNetworkManager>().HandlePlayerReadyUp(true);
            if (IsLocalPlayer) { Debug.Log("Not local on ready up"); return; }
            //we readied up so we are adding
            ChangeReadyPlayerCountServerRpc(true);
        }
        else
        {
            //Debug.Log("We ran the disable version");
            readyUpText.text = readyUpTexts[0];
            colors.normalColor = initialButtonNormalColor;
            colors.selectedColor = initialButtonNormalColor;
            readyUpButton.GetComponent<Button>().colors = colors;
            UI_Password.GetComponent<PasswordNetworkManager>().HandlePlayerReadyUp(false);
            if (IsLocalPlayer) { Debug.Log("Not local on unready"); return; }
            //we are no longer ready so subtracting
            ChangeReadyPlayerCountServerRpc(false);
        }

        
        
    }

    [ServerRpc(RequireOwnership =false)]
    private void ChangeReadyPlayerCountServerRpc(bool playerReadiedUp)
    {
        //only server should be changing this networkvariable playersReadyCount
        if (!IsServer) { return; }
        //add if readied up otherwise subtract 1 unless already zero
        playerReadyCount.Value += playerReadiedUp ? 1 : playerReadyCount.Value == 0 ? 0 : -1;
        //Debug.Log(playerReadyCount.Value + " number of players ready");
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetReadyPlayerCountToZeroServerRpc()
    {
        //only server should be changing this networkvariable playersReadyCount
        if (!IsServer) { return; }
        //add if readied up otherwise subtract 1 unless already zero
        playerReadyCount.Value = 0;
        //Debug.Log(playerReadyCount.Value + " number of players ready");
    }

    private void OnEnable()
    {
        playerReadyCount.OnValueChanged += OnPlayerReadyCountChanged;
    }

    private void OnDisable()
    {
        playerReadyCount.OnValueChanged -= OnPlayerReadyCountChanged;
    }

    private void OnPlayerReadyCountChanged(int oldCount, int newCount)
    {
        if (!IsClient) { return; }
        //set things to initial values
        if(playerReadyCount.Value == 2) 
        {
            //Debug.Log("We got here");
            //if (IsHost)
            //{
            //    //We'll try doing it this way
            //    playerReadyCount.Value = 0;
            //    //ChangeReadyPlayerCountServerRpc(false);

            //}
            messageScreen.SetActive(false);
            endTurnButton.SetActive(true);
            loadingAndWaitingTextObject.text = "";
            SetUpField();
            //we are no longer ready so subtracting
            
        }
    }

    //not used atm but probably will soon
    private void DisableField()
    {
        boardManagerNetwork.GetComponent<BoardManagerNetwork>().DestroyBoard();
        cardsContainer.SetActive(false);
    }

    private void SetUpField()
    {
        boardManagerNetwork.GetComponent<BoardManagerNetwork>().CreateBoard(2.4f,2.5f);
        cardsContainer.SetActive(true);
        cardsControllerNetwork.GetComponent<CardsControllerNetwork>().StartGame();
    }








}
