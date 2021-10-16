using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    Enemy enemyScript;//reference to the Enemy script for easy access
    [SerializeField] private float range;//attack range
    //[SerializeField] private float damage;//dmg we deal to enemies
    private float distance;//distance to enemy/currentNearestEnemy, starts at infinity so first check always lower;
    private float newDistance;//new distance to enemy, for checking distance and if a new enemy is closer than another/current target
    [SerializeField] private float timeBetweenAttacks;//attack delay
    private float timeToNextAttack;//actual time checkpoint to determine when the next attack will happen

    public GameObject currentTarget;
    private GameObject currentNearestEnemy;

    private void Start()
    {
        timeToNextAttack = Time.time;
    }

    private void updateNearestEnemy()
    {
        distance = Mathf.Infinity;//reset each time to make sure we will choose a new target if one is available
        //go through all the enemies and check distance to determine current target if any are in range
        foreach (GameObject enemy in Enemies.enemies)
        {
            
            if (enemy!=null)//make sure the enemy exists
            {
                newDistance = (transform.position - enemy.transform.position).magnitude;//basic distance formula
                if (newDistance < distance)
                {
                    distance = newDistance;
                    currentNearestEnemy = enemy;
                }

                //check distance <= range and update target enemy appropriately
                currentTarget = distance <= range ? currentNearestEnemy : null;
            }
            
        }
    }
    
    //protected will only let this class and classes that inherit/derive from it(its children) use it
    //virtual will allow us to override in the children too
    protected virtual void attack()
    {
            //enemyScript = currentTarget.GetComponent<Enemy>();//grab the enemy script from the currentTarget/enemy
            //enemyScript.takeDamage(damage);//deal damage 
    }

    public void dealRangedDamage(float damage, GameObject enemy)
    {
        enemyScript = enemy.GetComponent<Enemy>();//grab the enemy script from the currentTarget/enemy
        enemyScript.takeDamage(damage);
        Debug.Log("we got to the end of ranged dmg");
    }

    private void Update()
    {
        updateNearestEnemy();
        if (Time.time >= timeToNextAttack && currentTarget)
        {
            attack();
            timeToNextAttack = Time.time + timeBetweenAttacks;//update our next time checkpoint to attack
        }
       

    }
}
