using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVariants : MonoBehaviour
{
    //just here for my  stat referencing purposes
    private int enemyMaxHealth;
    private float movementSpeed;
    private int killReward; //amount of money gained when enemy is killed
    private int fortressDamage; // The amount of damage the enemy does when it reaches the end/a fortress
    private int dmgResistance; // basic damage resistance stat
    private int damage; // basic damage stat for attacking units
    private int agility; // basic evasion and speed modifier stat
    private int mana;
    private int spellPower;
    private int dodgeRate;
    private int critRate;
    private int critResist;
    private int critDmg;
    private int hitChance;
    private int baseHitChance = 80;
    public int rank; // the rank of the enemy which will have a huge effect on stats and in the future available abilities
    private int maxRank = 7;
    private int mod = 4;
    private int rankModifier;
    public Color spriteColor;
    public List<object> stats;
    private List<string> names = new List<string>() {"fodder", "minion", "lesser mob", "ravenous chicken",
        "mob", "hydra", "Demon Lord Headless Chicken"};
    private string name;

    private int DetermineRank()
    {
        return rank = Random.Range(1, maxRank+1);
    }
    // function for determining what the values should be based on the rank of the enemy
    private int RankModifier()
    {
        int value = 0;
        
            if (rank < (int)(maxRank / 2+1))
            {
               return value =  (int)Mathf.Pow((rank-1)*mod, 2);
            }
            else
            {
                
                return value = (int)(Mathf.Pow(2*mod,2)+ Mathf.Sqrt((float)((rank-3)*(mod+rank))));
            }
    }

    public void DetermineStats(ref int eMaxHealth,ref int eRank, ref int efortressDamage, ref int eAgility, ref int eDmgResistance,
        ref int eDamage, ref float eMovementSpeed, ref int eKillReward,ref int eDodgeRate, ref int eCritDmg,
        ref int eCritRate, ref int eHitChance, ref int eMana, ref int eSpellPower,
        ref int eCritResist, ref string eName, ref Color eSpriteColor)
    {
        DetermineRank();
        rankModifier = RankModifier();
        enemyMaxHealth = 100 + rankModifier;
        name = names[rank - 1];
        fortressDamage = rank;
        agility = (int)(1 + .15 * rankModifier);
        damage = (int)(2 + 1.5 * rankModifier);
        dmgResistance = (int)(1 + .3 * damage);
        spellPower = (int)(4 + 2 * rankModifier);
        movementSpeed = (float)(Mathf.Sqrt(agility));
        killReward = (int)(50 * rankModifier);
        mana = 50 * rankModifier;
        hitChance = baseHitChance + mod * (int)Mathf.Pow(rank,2);
        dodgeRate = mod * (int)Mathf.Pow(rank, 2) / 4;//keep it under 50 for now
        critDmg = damage * rank;//OP asf
        critRate = 1 * rank;//can't be having this be too high now
        critResist = 2 * rank;//somewhat OP or maybe not?
        spriteColor = new Color(1.0f / (float)rank, (float)Random.Range(0,100)/100, 0);
        //Debug.Log("enemy Max Health " + enemyMaxHealth);
        //Debug.Log("enemy fortressDamage " + fortressDamage);
        //Debug.Log("attack " + attack);
        //Debug.Log("defense " + defense);
        //Debug.Log("agility " + agility);
        //Debug.Log("movementSpeed " + movementSpeed);
        //Debug.Log("killReward " + killReward);
        //Debug.Log("Color " + spriteColor);
        eMaxHealth = enemyMaxHealth;
        eRank = rank;
        eName = name;
        efortressDamage = fortressDamage;
        eAgility = agility;
        eDmgResistance = dmgResistance;
        eDamage = damage;
        eMovementSpeed = movementSpeed;
        eKillReward = killReward;
        eDodgeRate = dodgeRate;
        eCritDmg = critDmg;
        eCritRate = critRate;
        eHitChance = hitChance;
        eMana = mana;
        eSpellPower = spellPower;
        eCritResist = critResist;
        eSpriteColor = spriteColor;

    }
}
