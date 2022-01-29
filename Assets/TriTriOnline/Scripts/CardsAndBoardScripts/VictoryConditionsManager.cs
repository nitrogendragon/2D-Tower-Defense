using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class VictoryConditionsManager : NetworkBehaviour
{
    [SerializeField] private GameObject ScoresContainer;
    [SerializeField] private GameObject player1ScoresContainer;
    [SerializeField] private GameObject player2ScoresContainer;
    [SerializeField] private Slider player1Slider;
    [SerializeField] private Slider player2Slider;
    [SerializeField] private GameObject VictoryTextContainer;
    [SerializeField] private TMP_Text VictoryText;
    private float player1Count = 0f;
    private float player2Count = 0f;
    private bool firstUpdate = true;

    // Start is called before the first frame update
    void Start()
    {
        ScoresContainer.SetActive(false);
        VictoryTextContainer.SetActive(false);
    }
    [ClientRpc]
    public void UpdateSliderClientRpc(bool isPlayer1Slider, bool isCardBeingPlaced)
    {
        if (firstUpdate)
        {
            firstUpdate = false;
            ScoresContainer.SetActive(true);
        }
        //handles cards being placed on the field
        if (isCardBeingPlaced)
        {
            if (isPlayer1Slider)
            {
                player1Count += 1;
                player1Slider.value = player1Count / 20f;
                
            }
            else
            {
                player2Count += 1;
                player2Slider.value = player2Count / 20f;
                
            }
        }
        //handle cards changing ownership
        else
        {
            if (isPlayer1Slider)
            {
                player1Count += 1;
                player2Count -= 1;
                player1Slider.value = player1Count / 20f;
                player2Slider.value = player2Count / 20f;
                
            }
            else
            {
                player1Count -= 1;
                player2Count += 1;
                player1Slider.value = player1Count / 20f;
                player2Slider.value = player2Count / 20f;
            }
        }
        StartCoroutine(CheckForVictoryConditionsMet());
        
    }

    IEnumerator CheckForVictoryConditionsMet()
    {
        yield return new WaitForSeconds(1.2f);
        if(player1Count >= 4)
        {
            player1ScoresContainer.SetActive(false);
            player2ScoresContainer.SetActive(false);
            VictoryTextContainer.SetActive(true);
            VictoryText.text = "Player 1 Wins!";
        }
        else if(player2Count >= 4)
        {
            player1ScoresContainer.SetActive(false);
            player2ScoresContainer.SetActive(false);
            VictoryTextContainer.SetActive(true);
            VictoryText.text = "Player 2 Wins!"; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
