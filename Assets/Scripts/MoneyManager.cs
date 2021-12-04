using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    private int currentPlayerMoney;
    public int starterMoney;
    public Text playerFundsText;

    private void Awake()
    {
        addMoney(starterMoney);
        //Debug.Log(currentPlayerMoney);
    }

    public int getCurrentMoney()
    {
        return currentPlayerMoney;
    }

    public void addMoney(int amount)
    {
        currentPlayerMoney += amount;
        playerFundsText.text = "Aurum: $" + currentPlayerMoney;
    }

    public void removeMoney(int amount)
    {
        currentPlayerMoney = currentPlayerMoney - amount;
        playerFundsText.text = "Aurum: $" + currentPlayerMoney;
        //Debug.Log("Removed " + amount + " from player's money. Player has $" + currentPlayerMoney + " left.");
    }
}
