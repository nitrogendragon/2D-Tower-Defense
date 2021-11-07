using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    private int currentPlayerMoney;
    public int starterMoney;

    private void Awake()
    {
        addMoney(starterMoney);
        Debug.Log(currentPlayerMoney);
    }

    public int getCurrentMoney()
    {
        return currentPlayerMoney;
    }

    public void addMoney(int amount)
    {
        currentPlayerMoney += amount;
    }

    public void removeMoney(int amount)
    {
        currentPlayerMoney = currentPlayerMoney - amount;
        Debug.Log("Removed " + amount + " from player's money. Player has $" + currentPlayerMoney + " left.");
    }
}
