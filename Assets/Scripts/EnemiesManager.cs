using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{

    public static GameObject moneyManager;
    public static List<GameObject> enemies = new List<GameObject>();
    private void Start()
    {
        moneyManager = GameObject.Find("MoneyManager");
    }
    public static void supplyKillReward(int killReward)
    {
        //GameObject moneyManager = GameObject.Find("MoneyManager");
        moneyManager.GetComponent<MoneyManager>()
            .addMoney(killReward);
    }
}
