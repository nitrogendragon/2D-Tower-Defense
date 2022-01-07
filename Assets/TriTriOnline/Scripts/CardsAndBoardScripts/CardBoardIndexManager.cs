using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CardBoardIndexManager : NetworkBehaviour
{
    private void Start()
    {
        
    }

    //for now we have 9 board tiles so size of 9. the index will relate to position on field
    /**
     * 6 7 8
     * 3 4 5
     * 0 1 2
     * 
     * if a card is placed on 4 they wil check index - 1 for left attack, index +1 for right attack, index -3 for bottom attack and index +3 for top attack
     * left attacks right, right attacks left, top attacks bottom, bottom attacks top, ezpz
     */
    private NetworkObject[] cardsOnField = new NetworkObject[9];
    public void SetCardIndex(NetworkObject Card, int cardBoardTilePositionIndex)
    {
        cardsOnField[cardBoardTilePositionIndex] = Card;
        Debug.Log(cardsOnField[cardBoardTilePositionIndex].name);
    }

    public bool CheckIfCardAtIndex(int cardBoardIndex)
    {
        if (cardsOnField[cardBoardIndex]) { return true; }
        return false;
    }

    //when doing something whether attacking or otw, pass through our cards playerOwnerIndex/Id (aka player 1 or 2?) and compare to the target card on the board playerIndex/Id
    public bool CheckIfCardAtIndexIsOwnedByMe(int myCardPlayerOwnerIndex, int targetCardBoardIndex)
    {
        if( myCardPlayerOwnerIndex == cardsOnField[targetCardBoardIndex].GetComponent<MobCardNetwork>().GetPlayerOwner())
        {
            return true;
        }
        return false;
    }
}
