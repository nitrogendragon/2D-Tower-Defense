using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVariants : MonoBehaviour
{
    //just here for my  stat referencing purposes
    public int enemyMaxHealth;
    public float movementSpeed;
    public int killReward; //amount of money gained when enemy is killed
    public int fortressDamage; // The amount of damage the enemy does when it reaches the end/a fortress
    public int defense; // basic damage resistance stat
    public int attack; // basic damage stat for attacking units
    public int agility; // basic evasion and speed modifier stat
    public int rank; // the rank of the enemy which will have a huge effect on stats and in the future available abilities
    private int maxRank = 7;
    private int mod = 5;
    private int rankModifier;
    public Color spriteColor;
    public List<object> stats;
    

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
                
                return value = (int)(Mathf.Pow(2*mod,2)+ Mathf.Sqrt((float)((rank-3)*Mathf.Pow(mod,(float)rank))));
            }
    }

    public void DetermineStats(ref int eMaxHealth, ref int efortressDamage, ref int eAgility, ref int eDefense,
        ref int eAttack, ref float eMovementSpeed, ref int eKillReward, ref Color eSpriteColor)
    {
        DetermineRank();
        rankModifier = RankModifier();
        enemyMaxHealth = 100 + rankModifier;
        fortressDamage = rank;
        agility = (int)(1 + .05 * rankModifier);
        defense = (int)(1 + 1 * rankModifier);
        attack = (int)(2 + 1.5 * rankModifier);
        movementSpeed = (float)(Mathf.Sqrt(agility));
        killReward = (int)(50 * rankModifier);
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
        efortressDamage = fortressDamage;
        eAgility = agility;
        eDefense = defense;
        eAttack = attack;
        eMovementSpeed = movementSpeed;
        eKillReward = killReward;
        eSpriteColor = spriteColor;

    }
}
