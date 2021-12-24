using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnitStatsUI : MonoBehaviour
{
    private float timePassed;
    private float timeTilUpdate = .3f;
    private GameObject selectedUnit;
    private List<int> unitsStats;
    private List<int> unitsBaseStats;
    public Text unitName;
    public Text unitLevel;
    public Text unitExperience;
    public Text unitHP;
    public Text unitMana;
    public Text unitStrength;
    public Text unitIntelligence;
    public Text unitAgility;
    public Text unitWisdom;
    public Text unitCharm;
    public Text unitLuck;
    public Text unitStatPoints;
    public Text weaponName;
    public Image weaponImage;
    public Text armorName;
    public Image armorImage;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setSelectedUnit(GameObject unit, string name)
    {
        selectedUnit = unit;
        unitName.text = name;
        updateStatsUI();
    }

    public void useStatPoints(int statIndex)
    {

        selectedUnit.GetComponent<BasicUnit>().ApplyStats(statIndex);
    }

    public void updateStatsUI()
    {
        //current stats{unitMaxHp,remainingUnitHp, level, exp, currentStrength, currentIntelligence,
        //currentAgility, currentWisdom, currentLuck, currentCharm, statPoints, unitMaxMana, remainingMana};
        //unitsBaseStats{strength, intelligence,
        //    agility, wisdom, luck, charm};
        if (selectedUnit)
        {
            BasicUnit selectedUnitsScript = selectedUnit.GetComponent<BasicUnit>();
            unitsStats = selectedUnitsScript.getUnitStats();
            unitsBaseStats = selectedUnitsScript.getUnitBaseStats();
            weaponName.text = selectedUnitsScript.grabWeaponName();
            weaponImage.sprite = selectedUnitsScript.grabWeaponSprite();
            armorName.text = selectedUnitsScript.grabArmorName();
            armorImage.sprite = selectedUnitsScript.grabArmorSprite();
            unitHP.text = "HP " + unitsStats[1].ToString() + " / " + unitsStats[0].ToString();
            unitMana.text = " Mana " + unitsStats[12].ToString() + " / " + unitsStats[11].ToString();
            unitLevel.text = "Level " + unitsStats[2].ToString();
            unitExperience.text = "XP " + unitsStats[3].ToString();
            unitStrength.text = "STR " + unitsStats[4].ToString() + " / " + unitsBaseStats[0].ToString();
            unitIntelligence.text = "INT " + unitsStats[5].ToString() + " / " + unitsBaseStats[1].ToString();
            unitAgility.text = "AGI " + unitsStats[6].ToString() + " / " + unitsBaseStats[2].ToString();
            unitWisdom.text = "WIS " + unitsStats[7].ToString() + " / " + unitsBaseStats[3].ToString();
            unitLuck.text = "LCK " + unitsStats[8].ToString() + " / " + unitsBaseStats[4].ToString();
            unitCharm.text = "CHR " + unitsStats[9].ToString() + " / " + unitsBaseStats[5].ToString();
            unitStatPoints.text = "Stat Points: " + unitsStats[10].ToString();
        }
        else
        {
            unitsStats = null;
            unitsBaseStats = null;
            unitName.text = "No Raider Selected ";
            unitHP.text = "HP 0 / 0" ;
            unitMana.text = " Mana " + "0 / 0";
            unitLevel.text = "Level ";
            unitExperience.text = "XP 0 / 0";
            unitStrength.text = "STR 0 / 0" ;
            unitIntelligence.text = "INT 0 / 0";
            unitAgility.text = "AGI 0 / 0";
            unitWisdom.text = "WIS 0 / 0";
            unitLuck.text = "LCK 0 / 0";
            unitCharm.text = "CHR 0 / 0";
            unitStatPoints.text = "Stat Points: 0";
            weaponName.text = "Unequipped";
            weaponImage.sprite = null;
            armorName.text = "Unequipped";
            armorImage.sprite = null;
        }
    }


    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if(timePassed >= timeTilUpdate)
        {
            timePassed = 0;
            updateStatsUI();
        }
    }
}
