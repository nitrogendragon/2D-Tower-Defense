using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnitStatsUI : MonoBehaviour
{
    private GameObject selectedUnit;
    private List<int> unitsStats;
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setSelectedUnit(GameObject unit, string name)
    {
        selectedUnit = unit;
        //unitMaxHp,remainingUnitHp, level, exp, currentStrength, currentIntelligence,
        //currentAgility, currentWisdom, currentLuck, currentCharm};
        unitsStats = selectedUnit.GetComponent<BasicUnit>().getUnitStats();
        unitName.text = name;
        unitHP.text = unitsStats[1].ToString() + " / " + unitsStats[0].ToString();
        unitMana.text = "0 / 0";
        unitLevel.text = unitsStats[2].ToString();
        unitExperience.text = unitsStats[3].ToString();
        unitStrength.text = unitsStats[4].ToString();
        unitIntelligence.text = unitsStats[5].ToString();
        unitAgility.text = unitsStats[6].ToString();
        unitWisdom.text = unitsStats[7].ToString();
        unitLuck.text = unitsStats[8].ToString();
        unitCharm.text = unitsStats[9].ToString();


    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
