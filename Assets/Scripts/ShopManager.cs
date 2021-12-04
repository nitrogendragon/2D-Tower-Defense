using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public MoneyManager moneyManager;
    public GameObject basicUnitPrefab;
    
    public int unitCost;

    public int getUnitCost(int uCost)
    {
        unitCost = uCost;
        return unitCost;

    }

    

    public void buyUnit(int uCost)
    {
        
        moneyManager.removeMoney(getUnitCost(uCost));
    }

    public bool canBuyUnit(int uCost)
    {
        int cost = getUnitCost(uCost);
        //Debug.Log(cost);
        //Debug.Log("current Money " + moneyManager.getCurrentMoney());
        bool canBuy = false;
        if( moneyManager.getCurrentMoney() - cost >= 0)
        {
            canBuy = true;
            //Debug.Log("We have enough money");
        }
        else
        {

            //Debug.Log("We don't have enough money for some reason " + moneyManager.getCurrentMoney());
        }
        return canBuy;
    }
}
