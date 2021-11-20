using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUnit : Unit
{
    public GameObject projectile;
    //public List<GameObject> activeProjectiles = new List<GameObject>();
    //private List<GameObject> tempProjectiles = new List<GameObject>();
    public Transform unit;
    // each + adds one to the modifier per point
    public Transform weapon; // the units weapons transform, used in UnitRotation script
    private int strength = 10; //affects damage+++, dmg resistance++, skill recharge rate++, and hp++
    private int intelligence = 10; //affects mana+++, spell power++, spell dmg resistance++
    private int agility = 10; //affects movement speed+++, attack speed++, dodge++, hit+, dmgResistance+, crit rate+ and crit dmg+
    private int wisdom = 10; //affects spell casting speed+++, mana regen+++, spell cooldown++, hit++, critResist+, dmgResistance+
    private int charm = 10; //affects critResist+++, dodge++, crit rate++, crit damage++, hit+
    private int luck = 10; //affects crit rate+++, crit dmg+, dodge+, hit+

    private int damage;
    private int dmgResistance;
    private int hp;
    private int baseHp = 10;
    private int mana;
    private int dodgeRate;
    private int critRate;
    private int critResist;
    private int baseCritRate = 10;
    private int critDmg;
    private int hitChance;
    private int baseHitChance = 80;
    private float movementSpeed;
    private float rangedAttackSpeedMod;


    
    private void Start()
    {
        nearestEnemyWaiter = new WaitForSeconds(.1f);
        decideIfShouldAttackWaiter = new WaitForSeconds(maxAttackSpeed);
        decideIfShouldAttack();//only need to run once then co-routine will manage updates
        updateNearesetEnemy();//only need to run once then coRoutine will manage updates 
        hp = strength * 20 + baseHp;
        damage = strength * 30;
        dmgResistance = strength * 2 + agility + wisdom;
        dodgeRate = charm * 2 + luck + dodgeRate * 2;
        hitChance = baseHitChance + agility + wisdom * 2 + luck;
        critRate = baseCritRate + luck * 3 + charm * 2 + agility;
        critDmg = damage * 2 + luck * 10 + charm * 20 + agility* 10;
        critResist = luck + charm + agility;
        rangedAttackSpeedMod = Mathf.Sqrt(agility);
        timeBetweenAttacks = 1.0f /rangedAttackSpeedMod;//We will have to go over 400 agility to stop increasing attack speed
        range = 1.1f + (agility + .6f*(strength + intelligence + wisdom) / 10);
        //Debug.Log("we finished everything in here");
    }


    protected override void attack()
    {
        //base.attack();
        GameObject newProjectile = Instantiate(projectile, weapon.position, unit.rotation);
        newProjectile.GetComponent<Projectile>().expirationTime = 3f; // will be determined by unit stats and specific abilities later
        newProjectile.GetComponent<Projectile>().speed = 10f + rangedAttackSpeedMod; // will be determined by unit stat and or specific abilities later
        newProjectile.GetComponent<Projectile>().myUnit = this;
    }

    public void dealRangedDamage(GameObject enemy)
    {
        enemyScript = enemy.GetComponent<Enemy>();//grab the enemy script from the currentTarget/enemy
        enemyScript.takeDamage(damage, critDmg, hitChance, critRate);

    }






}
