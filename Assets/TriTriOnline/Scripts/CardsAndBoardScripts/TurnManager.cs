using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
public class TurnManager : NetworkBehaviour
{
    //will be used to determine what a player should be doing, drawing, placing a mob card, placing an ability card / activating abilities/spells/traps/field cards, ending turn
    private NetworkVariable<int> turnActionIndex = new NetworkVariable<int>(0);
    //Network Specific variables below
    //who's turn is it? player1(host) or player2(client), determines if we should draw and can place cards and so on in the turn execution cycle
    private NetworkVariable<bool> isPlayer1Turn = new NetworkVariable<bool>(true);

    //time to wait til ending turn
    private float endTurnWait = 1.2f;

    private bool isWaitingToChangePhase = false;

    //the text we will be changing
    [SerializeField] private TMP_Text turnInfoText;

    [SerializeField] private Button endTurnButton;

    

    //this will handle all the logic for determining whose turn it is and what phase of their turn they are in, plan is to have draw phase, play mob phase, play ability card phase(magic,trap,field, whatever), and then end phase/just switch whose turn it is
    [ServerRpc(RequireOwnership =false)]
    public void UpdateTurnManagerServerRpc()
    {
        if (!IsServer) { return; }
        //Debug.Log("we are running updateturnmanagerSErverRpc");
        if (isPlayer1Turn.Value)
        {
            if(turnActionIndex.Value != 2)
            {
                turnActionIndex.Value += 1;
                return;
            }
            isPlayer1Turn.Value = false;
            turnActionIndex.Value = 0;
            return;
        }
        else if (!isPlayer1Turn.Value)
        {
            if (turnActionIndex.Value != 2)
            {
                turnActionIndex.Value += 1;
                return;
            }
            isPlayer1Turn.Value = true;
            turnActionIndex.Value = 0;
            return;
        }
        //Debug.Log("after running updateturnmanagerserverrpc these are our isplayer1turn and turnactionindex values: " + isPlayer1Turn + "  " + turnActionIndex);
        
    }

    [ServerRpc(RequireOwnership =false)]
    public void InitializeTurnManagerServerRpc()
    {
        if (!IsServer) { return; }
        //get it so it's player 1's turn and it's their summon phase
        isPlayer1Turn.Value = true;
        turnActionIndex.Value = 1;
        //Debug.Log("we initialized the turn manager");
        //Debug.Log("isplayer1Turn and turnactionindex are: " + isPlayer1Turn.Value + "  " + turnActionIndex.Value);
    }

    private void OnEnable()
    {
        turnActionIndex.OnValueChanged += OnTurnActionIndexChanged;
        isPlayer1Turn.OnValueChanged += OnIsPlayer1TurnChanged;
    }

    private void OnDisable()
    {
        turnActionIndex.OnValueChanged -= OnTurnActionIndexChanged;
        isPlayer1Turn.OnValueChanged -= OnIsPlayer1TurnChanged;
    }

    private void OnTurnActionIndexChanged(int oldIndex, int newIndex)
    {
        if (!IsClient) { return; }
        switch (turnActionIndex.Value)
        {
            case 0:
                if (isPlayer1Turn.Value)
                {
                    turnInfoText.text = "Player 1 Draw Phase";
                }
                else
                {
                    turnInfoText.text = "Player 2 Draw Phase";
                }
                break;
            case 1:
                if (isPlayer1Turn.Value)
                {
                    turnInfoText.text = "Player 1 Summon Phase";
                }
                else
                {
                    turnInfoText.text = "Player 2 Summon Phase";
                }
                break;
                //to be added later
            //case 2:
            //    if (isPlayer1Turn.Value)
            //    {
            //        turnInfoText.text = "Player 1 Secondary Action Phase";
            //    }
            //    else
            //    {
            //        turnInfoText.text = "Player 2 Secondary Action Phase";
            //    }
            //    break;
            case 2:
                if (isPlayer1Turn.Value)
                {
                    turnInfoText.text = "Player 1 End Phase";
                }
                else
                {
                    turnInfoText.text = "Player 2 End Phase";
                }
                break;


        }
                
    }

    public IEnumerator WaitToEndPhase()
    {

        isWaitingToChangePhase = true;
        //Debug.Log("We started waiting" + Time.time);
        yield return new WaitForSeconds(endTurnWait);
        //Debug.Log("We have finished waiting " + Time.time);
        //after x number of seconds after the action has been processed, go to the next phase
        UpdateTurnManagerServerRpc();
        isWaitingToChangePhase = false;
        
        
    }

    private void OnIsPlayer1TurnChanged(bool oldBool, bool newBool)
    {
        if (!IsClient) { return; }
    }

    public bool GetIsPlayer1Turn()
    {
        bool isP1Turn = isPlayer1Turn.Value ? true : false;
        return isP1Turn;
    }

    public int GetTurnActionIndex()
    {
        return turnActionIndex.Value;
    }

    public bool GetWaitingForPhaseChangeStatus()
    {
        return isWaitingToChangePhase;
    }

    public void TryEndTurn()
    {
        if(IsHost && isPlayer1Turn.Value && turnActionIndex.Value == 2 ||
            !IsHost && IsClient && !isPlayer1Turn.Value && turnActionIndex.Value == 2)
        {
            UpdateTurnManagerServerRpc();
        }
        else { Debug.Log("you can't end your turn right now"); }
    }
    
}
