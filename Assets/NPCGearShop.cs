using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCGearShop : MonoBehaviour
{
    public ShopManager shopManager;
    public ClickManager clickManager;

    public List<Sprite> gearIcons = new List<Sprite>();//our sprites for the gear that we filled in the prefab in the editor
    private List<string> gearNames = new List<string>() {"bloodstained pistol", "dark sorcerors cowl", "gold santa suit", 
        "green hunters garb", "iron shortsword", "jeweled hunter's garb", "krok scale armor", "pig skinner", "rusty hilt", 
        "rusty pistol", "sludge covered breastplate", "sorcerer's cowl", "the bee's knees", "wakizashi", "wizard's wand" };
    private List<int> gearAgiStats = new List<int>() {0, 0, 0, 0, 8, 5, 0, 10, 10, 0, 0, 0, 0, 13, 0};//last update was wizards wand
    private List<int> gearChaStats = new List<int>() {0, 0, 8, 0, 0, 0, 0, 14, 0, 0, 5, 0, 0, 6, 7};
    private List<int> gearIntStats = new List<int>() {0, 16, 6, 5, 0, 7, 19, 7, 0, 0, 0, 13, 17, 0, 14};
    private List<int> gearLucStats = new List<int>() {15, 9, 0, 0, 0, 0, 9, 0, 0, 11, 0, 8, 29, 0, 0};
    private List<int> gearStrStats = new List<int>() {25, 8, 24, 5, 8, 7, 26, 0, 7, 15, 12, 5, 0, 0, 0};
    private List<int> gearWisStats = new List<int>() {10, 18, 12, 0, 5, 5, 12, 0, 14, 6, 11, 15, 21, 10, 16};
    private List<bool> isWeaponBools = new List<bool>() {true, false, false, false, true, false, false, true, true, true, false, false, true,
        true, true};
    private List<int> gearAurCostStats = new List<int>();//determined with function on start
    
    private void Awake()
    {
        determineGearCosts();
    }

    private void determineGearCosts()
    {
        for(int i = 0; i < gearNames.Count-1; i++)
        {
            int tempCost = gearAgiStats[i] + gearStrStats[i] + gearIntStats[i] + gearWisStats[i] + gearLucStats[i] + gearChaStats[i];
            //tempCost = tempCost * tempCost;
            gearAurCostStats.Add(tempCost);
        }
    }

    public void createShopGear(ref int strStat, ref int intStat, ref int agiStat, ref int wisStat, ref int lucStat, ref int chaStat,
        ref int aurCostStat,ref string gearName, ref bool isWeapon, ref Sprite gearIconSprite)
    {
        int gearIndex = Random.Range(0,gearNames.Count-1);
        strStat = gearStrStats[gearIndex];
        intStat = gearIntStats[gearIndex];
        agiStat = gearAgiStats[gearIndex];
        wisStat = gearWisStats[gearIndex];
        lucStat = gearLucStats[gearIndex];
        chaStat = gearChaStats[gearIndex];
        aurCostStat = gearAurCostStats[gearIndex];
        gearName = gearNames[gearIndex];
        isWeapon = isWeaponBools[gearIndex];
        gearIconSprite = gearIcons[gearIndex];
        //Debug.Log("We go to the end of the createShopGear function");
    }

    public bool VerifyUnitToEquipGear()
    {
        
        if (clickManager.selectedUnit != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool BuyGear(int aurCostStat, string gearName )//spend aurum to buy the gear and equip it on the selected raider/unit
    {
        if (shopManager.canBuyUnit(aurCostStat))
        {
            shopManager.buyUnit(aurCostStat);
            Debug.Log("We spent " + aurCostStat + " aurum on " + gearName + ".");
            return true;
        }
        else
        {
            Debug.Log("We don't have enough aurum right now");
            return false;//we don't have the money to buy so don't try to move on to next function to equip
        }
    }

    public void EquipGearToSelectedUnit(int strStat, int intStat, int agiStat, int wisStat, int chaStat, int lucStat, string gearName,
        bool isWeapon, Sprite gearSprite)
    {
        if (clickManager.selectedUnit)//just to make doubly sure for now at least
        {
            clickManager.selectedUnit.GetComponent<Unit>().EquipGear(strStat, intStat, agiStat, wisStat, chaStat,lucStat, 
                gearName, isWeapon, gearSprite);

            clickManager.selectedUnit.GetComponent<BasicUnit>().ApplyStats(6);//6 is for equpping gear to tell game to update status for it
        }
    }
}
