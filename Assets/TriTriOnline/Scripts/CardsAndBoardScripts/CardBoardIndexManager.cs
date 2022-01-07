using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CardBoardIndexManager : NetworkBehaviour
{
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

    public NetworkObject GetCardAtIndex(int cardBoardIndex)
    {
        return cardsOnField[cardBoardIndex];
    }
}
