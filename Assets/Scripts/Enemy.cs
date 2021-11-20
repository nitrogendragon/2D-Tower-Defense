using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected int enemyMaxHealth;
    private int enemyHealth;
    [SerializeField]
    protected float movementSpeed;
    private float distance;
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
        Debug.Log("Enemy dodge rate is " + dodgeRate);
        Debug.Log("Enemy critResist is " + critResist);
        Debug.Log("Enemy dmgResist is " + dmgResistance);
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b);
        
        enemyHealth = enemyMaxHealth;
        healthBar.setHealth(enemyHealth, enemyMaxHealth);
        
    }

    private bool checkSuccess(int successMod, int failMod)
    {
        int successChance = successMod - failMod;
        int result = Random.Range(1, 101);
        if (successChance >= result)
        {
            return true;
        }
        return false;

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
            distance = (transform.position - targetTile.transform.position).magnitude;//get distance to targetTile from enemy
        }
        if(targetTile != MapGenerator.endTile && distance < 0.001f)
        {
                currentIndex = MapGenerator.pathTiles.IndexOf(targetTile);
                targetTile = MapGenerator.pathTiles[currentIndex + 1];
        }
        else if(distance < 0.001f)
        {
            damageFortress();
            
        }
        
    }

    //takes in a limit to updates per second and checks if enough time has passed so we don't do something more than x times per second
    //private bool limitedUpdatesChecker(int maxUpdatesPerSecond)
    //{
    //    timePassed += Time.deltaTime;
    //    if(1f/maxUpdatesPerSecond <= timePassed)//if a x fraction of a second has passed 
    //    {
    //        timePassed = 0;
    //        return true;
    //    }
    //    return false;
    //}

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
