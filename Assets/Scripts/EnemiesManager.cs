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
    public static void supplyKillReward(int killReward,int rank, int unitID)
    {
        //GameObject moneyManager = GameObject.Find("MoneyManager");
        moneyManager.GetComponent<MoneyManager>()
            .addMoney(killReward);
        foreach(GameObject unit in UnitManager.activeUnits)
        {
            if (unit.GetComponent<BasicUnit>().getUnitID() == unitID)
            {
                Debug.Log(rank);
                unit.GetComponent<BasicUnit>().gainExperience(rank);
            }
        }
    }
}
