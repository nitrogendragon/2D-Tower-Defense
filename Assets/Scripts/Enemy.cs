using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float enemyMaxHealth;
    private float enemyHealth;
    [SerializeField]
    private float movementSpeed;
    private float distance;
    private float timePassed;//for frameChecker() keeps track of time passed 
    private int killReward; //amount of money gained when enemy is killed
    private int fortressdamage; // The amount of damage the enemy does when it reaches the end/a fortress
    private int currentIndex;// index for what tile the enemy is on/was on
    public HealthBar healthBar;//reference to our health bar script

    private GameObject targetTile;

    private void Awake()
    {
        Enemies.enemies.Add(gameObject);// add to active enemies
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
        enemyHealth = enemyMaxHealth;
        healthBar.setHealth(enemyHealth, enemyMaxHealth);
    }

    public void takeDamage(float amount)
    {
        enemyHealth -= amount;
        healthBar.setHealth(enemyHealth,enemyMaxHealth);
        if(enemyHealth <= 0)
        {
            die();
        }
    }

    private void die()
    {
        Enemies.enemies.Remove(gameObject);//get rid of this enemy from the enemies list
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
    private bool limitedUpdatesChecker(int maxUpdatesPerSecond)
    {
        timePassed += Time.deltaTime;
        if(1f/maxUpdatesPerSecond <= timePassed)//if a x fraction of a second has passed 
        {
            timePassed = 0;
            return true;
        }
        return false;
    }

    private void damageFortress()
    {
        //to do : Add fortresses or at least a health bar for the player to deal dmg to so you can lose towers,fortresses, and the game

        die();
    }

    private void Update()
    {
        checkPosition();
        moveEnemy();
    }

}
