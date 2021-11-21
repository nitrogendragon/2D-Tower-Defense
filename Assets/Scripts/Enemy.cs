using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    [SerializeField]
    protected int enemyMaxHealth;
    private int enemyHealth;
    [SerializeField]
    protected float movementSpeed;
    private float distanceToTargetTile;
    private float timePassed;//for frameChecker() keeps track of time passed 
    protected int killReward; //amount of money gained when enemy is killed
    protected int fortressDamage = 1; // The amount of damage the enemy does when it reaches the end/a fortress
    //protected int strength = 1; //affects damage+++, dmg resistance++, skill recharge rate++, and hp++
    //protected int intelligence = 1; //affects mana+++, spell power++, spell dmg resistance++
    protected int agility = 1; //affects movement speed+++, attack speed++, dodge++, hit+, dmgResistance+, crit rate+ and crit dmg+
    //protected int wisdom = 1; //affects spell casting speed+++, mana regen+++, spell cooldown++, hit++, critResist+, dmgResistance+
    //protected int charm = 1; //affects critResist+++, dodge++, crit rate++, crit damage++, hit+
    //protected int luck = 1; //affects crit rate+++, crit dmg+, dodge+, hit+
    protected int rank; // the rank of the enemy which will have a huge effect on stats and in the future available abilities

    private int damage;
    private int dmgResistance;
    private int mana;
    private int spellPower;
    private int dodgeRate;
    private int critRate;
    private int critResist;
    private int baseCritRate = 10;
    private int critDmg;
    private int hitChance;
    private int baseHitChance = 80;
    private float rangedAttackSpeedMod;

    protected string enemyName; //the name of the enemy
    protected string type; // the type of the enemy for bonus resistances and damage modifiers
    protected Color spriteColor; // a simple way to represent the rank of the enemy and differentiate them
    private int currentIndex;// index for what tile the enemy is on/was on
    public HealthBar healthBar;//reference to our health bar script
    public GameObject enemyVariants;// reference to our EnemyVariants script
    private GameObject targetTile;
    public GameObject guildManager;
    SpriteRenderer sprite;
    
    
    private void Awake()
    {
        isUnit = false;
        EnemiesManager.enemies.Add(gameObject);// add to active enemies
    }
    

    private void Start()
    {
        initializeEnemy();
    }

    public float getHealth()
    {
        return enemyHealth;
    }

    private void initializeEnemy()
    {
        targetTile = MapGenerator.startTile;

        enemyVariants.GetComponent<EnemyVariants>().DetermineStats(ref enemyMaxHealth, ref fortressDamage, 
            ref agility, ref dmgResistance, ref damage, ref movementSpeed, ref killReward, 
            ref dodgeRate, ref critDmg, ref critRate, ref hitChance, ref mana, ref spellPower, 
            ref critResist, ref spriteColor);
        rangedAttackSpeedMod = Mathf.Sqrt(agility);
        timeBetweenAttacks = 1.0f / rangedAttackSpeedMod;//We will have to go over 400 agility to stop increasing attack speed
        range = 1.5f + (agility/ 10);
        Debug.Log("Enemy dodge rate is " + dodgeRate);
        Debug.Log("Enemy critResist is " + critResist);
        Debug.Log("Enemy dmgResist is " + dmgResistance);
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b);
        
        enemyHealth = enemyMaxHealth;
        healthBar.setHealth(enemyHealth, enemyMaxHealth);
        //for dealing with finding enemy player units and determining if we should attempt to attack
        nearestEnemyWaiter = new WaitForSeconds(.1f);
        decideIfShouldAttackWaiter = new WaitForSeconds(maxAttackSpeed);
        decideIfShouldAttack();//only need to run once then co-routine will manage updates
        updateNearesetEnemy();//only need to run once then coRoutine will manage updates 

    }
    //going to just do a simple damage check without projectiles for now
    protected override void attack()
    {
        ////base.attack();
        //GameObject newProjectile = Instantiate(projectile, weapon.position, unit.rotation);
        //newProjectile.GetComponent<Projectile>().expirationTime = 3f; // will be determined by unit stats and specific abilities later
        //newProjectile.GetComponent<Projectile>().speed = 10f + rangedAttackSpeedMod; // will be determined by unit stat and or specific abilities later
        //newProjectile.GetComponent<Projectile>().myUnit = this;
        dealRangedDamage(currentTarget);
    }

    public void dealRangedDamage(GameObject enemyPlayerUnit)
    {
        
        enemyPlayerUnit.GetComponent<BasicUnit>().takeDamage(damage, critDmg, hitChance, critRate);
        Debug.Log("somehow this is already working");
    }

    

    public void takeDamage(int damageDealt,int critDamageDealt, int hitChance, int critRate)
    {
        if (checkSuccess(hitChance, dodgeRate))
        {
            Debug.Log("We hit the enemy");
            if(checkSuccess(critRate,critResist))
            {
                enemyHealth -= critDamageDealt - dmgResistance > 0 ? critDamageDealt - dmgResistance : 0;
                Debug.Log("We dealt a critical hit");
                Debug.Log("We did " + (critDamageDealt - dmgResistance) + " damage");
            }
            else
            {
                enemyHealth -= (damageDealt - dmgResistance) > 0 ? damageDealt - dmgResistance : 0;
                Debug.Log("We did " + (damageDealt - dmgResistance) + " damage");
            }

            healthBar.setHealth(enemyHealth, enemyMaxHealth);
            if (enemyHealth <= 0)
            {
                die();
            }
        }
        else
        {
            Debug.Log("We missed");
        }
       
        
    }

    private void die()
    {
        EnemiesManager.enemies.Remove(gameObject);//get rid of this enemy from the enemies list
        Destroy(transform.gameObject);
    }

    private void moveEnemy()
    {
        transform.position = targetTile ? 
            Vector3.MoveTowards(transform.position, targetTile.transform.position, movementSpeed * Time.deltaTime) : transform.position;
    }

    private void checkPosition()
    {
        if(targetTile != null)
        {
            distanceToTargetTile = (transform.position - targetTile.transform.position).magnitude;//get distance to targetTile from enemy
        }
        if(targetTile != MapGenerator.endTile && distanceToTargetTile < 0.001f)
        {
                currentIndex = MapGenerator.pathTiles.IndexOf(targetTile);
                targetTile = MapGenerator.pathTiles[currentIndex + 1];
        }
        else if(distanceToTargetTile < 0.001f)
        {
            damageFortress();
            
        }
        
    }


    private void damageFortress()
    {
        //to do : Add fortresses or at least a health bar for the player to deal dmg to so you can lose towers,fortresses, and the game
        GameObject targetGuild = guildManager.GetComponent<GuildManager>().GrabGuild();
        
        if (targetGuild)
        {
            targetGuild.GetComponent<Guild>().TakeDamage(fortressDamage);
            
        }
        die();
    }

    private void Update()
    {
        checkPosition();
        moveEnemy();
    }

}
