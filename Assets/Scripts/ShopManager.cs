using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public MoneyManager moneyManager;
    public GameObject basicUnitPrefab;
    public GameObject costText;
    public GameObject nameText;
    public GameObject unitIcon;
    public int basicUnitCost;

    public int getUnitCost(GameObject unitPrefab)
    {
        int cost = 0;
        if(unitPrefab == basicUnitPrefab)
        {
            cost = basicUnitCost;
        }
        return cost;
    }

    public void getUnitName(GameObject unitPrefab)
    {

    }

    public void buyUnit(GameObject unitPrefab)
    {
        
        moneyManager.removeMoney(getUnitCost(unitPrefab));
    }

    public bool canBuyUnit(GameObject unitPrefab)
    {
        int cost = getUnitCost(unitPrefab);
        Debug.Log(cost);
        Debug.Log("current Money " + moneyManager.getCurrentMoney());
        bool canBuy = false;
        if( moneyManager.getCurrentMoney() - cost >= 0)
        {
            canBuy = true;
            Debug.Log("We have enough money");
        }
        else
        {

            Debug.Log("We don't have enough money for some reason " + moneyManager.getCurrentMoney());
        }
        return canBuy;
    }
}
