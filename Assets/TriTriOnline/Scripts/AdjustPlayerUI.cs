using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class AdjustPlayerUI : MonoBehaviour
{
    [SerializeField] Image playerNameBackground;
    public void MovePlayerName(int clientIndex)
    {
        if(clientIndex == 0)
        {
            playerNameBackground.transform.position = new Vector3(-900, -400, 0);
            return;
        }
        playerNameBackground.transform.position = new Vector3(900, -400, 0);

    }
}
