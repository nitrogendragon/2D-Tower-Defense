using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float enemyHealth;
    [SerializeField]
    private float movementSpeed;
    private float distance;

    private int killReward; //amount of money gained when enemy is killed
    private int fortressdamage; // The amount of damage the enemy does when it reaches the end/a fortress
    private int currentIndex;// index for what tile the enemy is on/was on

    private GameObject targetTile;

    private void Awake()
    {
        Enemies.enemies.Add(gameObject);// add to active enemies
    }

    private void Start()
    {
        initializeEnemy();
    }

    private void initializeEnemy()
    {
        targetTile = MapGenerator.startTile;
    }

    public void takeDamage(float amount)
    {
        enemyHealth -= amount;
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
