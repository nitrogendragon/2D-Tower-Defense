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
    private int unitMaxHp;
    private int remainingUnitHp;
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
    public Vector3 movementTarget;
    private Vector3 nullTarget;//impossible to move to
    public HealthBar healthBar;//reference to our health bar script
    private List<string> unitNames = new List<string>() { "gunner", "mage", "priest", "knight", "samurai", "ninja", "dancer" };//reference
    private List<int> unitCosts = new List<int>() { 250, 300, 400, 200, 350, 600, 700 };
    private List<int> unitsStrengths = new List<int> { 7, 3, 5, 9, 10, 6, 4 };
    private List<int> unitsIntelligences = new List<int> { 5, 9, 7, 2, 0, 4, 5};
    private List<int> unitsAgilities = new List<int> { 4, 2, 5, 5, 6, 8, 7 };
    private List<int> unitsWisdoms = new List<int> { 6, 7, 8, 3, 4, 6, 5 };
    private List<int> unitsCharms = new List<int> { 2, 3, 6, 5, 4, 5, 9 };
    private List<int> unitsLucks = new List<int> { 8, 4, 4, 3, 6, 9, 6 };
    private List<int> unitsTotalPoints = new List<int> { 32, 28, 35, 27, 30, 38, 36 };

    private void Awake()
    {
        isUnit = true;
        UnitManager.activeUnits.Add(gameObject);

        //Debug.Log(UnitManager.activeUnits.Count);
    }

    private void Start()
    {
        nullTarget = new Vector3(10000, 0, 0);
        movementTarget = nullTarget;//this will never be possible to move to
        nearestEnemyWaiter = new WaitForSeconds(.1f);
        decideIfShouldAttackWaiter = new WaitForSeconds(maxAttackSpeed);
        decideIfShouldAttack();//only need to run once then co-routine will manage updates
        updateNearesetEnemy();//only need to run once then coRoutine will manage updates 
        strength = unitsStrengths[unitID];
        intelligence = unitsIntelligences[unitID];
        wisdom = unitsWisdoms[unitID];
        agility = unitsAgilities[unitID];
        charm = unitsCharms[unitID];
        luck = unitsLucks[unitID];
        Debug.Log("strength: " + strength + " " + "intelligence: " + intelligence + " " + "wisdom: " + wisdom + " " +
            "agility: " + agility + " " + "charm: " + charm + " " + "luck: " + luck + " ");
        unitMaxHp = strength * 200 + baseHp;
        damage = strength * 3;
        dmgResistance = strength * 2 + agility + wisdom;
        dodgeRate = charm * 2 + luck + dodgeRate * 2;
        hitChance = baseHitChance + agility + wisdom * 2 + luck;
        critRate = baseCritRate + luck * 3 + charm * 2 + agility;
        critDmg = (int)(damage * 1.5 + luck + charm * 2 + agility);
        critResist = luck + charm + agility;
        rangedAttackSpeedMod = Mathf.Sqrt(agility);
        timeBetweenAttacks = 1.0f /rangedAttackSpeedMod;//We will have to go over 400 agility to stop increasing attack speed
        range = 1.1f + (agility + .6f*(strength + intelligence + wisdom) / 10);
        movementSpeed = (float)(Mathf.Sqrt(agility));
        remainingUnitHp = unitMaxHp;
        healthBar.setHealth(remainingUnitHp, unitMaxHp);
        
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

    public void takeDamage(int damageDealt, int critDamageDealt, int hitChance, int critRate)
    {
        if (checkSuccess(hitChance, dodgeRate))
        {
            //Debug.Log("We hit the enemy");
            if (checkSuccess(critRate, critResist))
            {
                remainingUnitHp -= critDamageDealt - dmgResistance > 0 ? critDamageDealt - dmgResistance : 0;
                //Debug.Log("We dealt a critical hit");
                //Debug.Log("We did " + (critDamageDealt - dmgResistance) + " damage");
            }
            else
            {
                remainingUnitHp -= (damageDealt - dmgResistance) > 0 ? damageDealt - dmgResistance : 0;
                //Debug.Log("We did " + (damageDealt - dmgResistance) + " damage");
            }

            healthBar.setHealth(remainingUnitHp, unitMaxHp);
            if (remainingUnitHp <= 0)
            {
                die();
            }
        }
        else
        {
            //Debug.Log("We missed");
        }


    }

    public void movePlayerToTarget()
    {
        transform.position = movementTarget!= nullTarget && movementTarget != transform.position ?
            Vector3.MoveTowards(transform.position, movementTarget, movementSpeed * Time.deltaTime) : transform.position;
    }

    private void die()
    {
        UnitManager.activeUnits.Remove(gameObject);//get rid of this unit from the list
        Destroy(transform.gameObject);
    }

    private void Update()
    {
        movePlayerToTarget();
    }







}
