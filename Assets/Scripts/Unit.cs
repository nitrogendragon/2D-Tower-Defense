using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    protected Enemy enemyScript;//reference to the Enemy script for easy access
    protected BasicUnit playerUnitScript; //reference to the BasicUnit script for easy access
    protected float range;//attack range
    private float distance;//distance to enemy/currentNearestEnemy, starts at infinity so first check always lower;
    private float newDistance;//new distance to enemy, for checking distance and if a new enemy is closer than another/current target
    protected float timeBetweenAttacks = 1f;//attack delay, set in editor otw 1f
    protected float maxAttackSpeed = 0.05f;//we will never allow more than 20 attacks a second per unit
    //private float timeSinceLastAttack;// keep track of how long it's been since an attack occured
    private float lastAttackTime;//keeps track of the time when the last attack happened;
    protected bool isUnit; //are we a player unit?
    protected WaitForSeconds nearestEnemyWaiter;
    protected WaitForSeconds decideIfShouldAttackWaiter;
    public GameObject currentTarget;
    private GameObject currentNearestEnemy;
    public Text nameText;
    protected int unitID;
    protected bool isCasting;

    public void setUnitID(int id)
    {
        unitID = id;
    }

    public int getUnitID()
    {
        return unitID;
    }

    public bool getCastingStatus()
    {
        return isCasting;
    }

    protected void updateNearesetEnemy()
    {
        StartCoroutine("IupdateNearestEnemy");
    }

    IEnumerator IupdateNearestEnemy()
    {
        while (true)
        {
            distance = Mathf.Infinity;//reset each time to make sure we will choose a new target if one is available
                                      //go through all the enemies and check distance to determine current target if any are in range

            foreach (GameObject enemy in isUnit ? EnemiesManager.enemies : UnitManager.activeUnits)
            {
                if (enemy != null)//make sure the enemy exists
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
            yield return nearestEnemyWaiter;//only check for enemies/update currentTarget every tenth of a second
        }
    }
    
    //protected will only let this class and classes that inherit/derive from it(its children) use it
    //virtual will allow us to override in the children too
    protected virtual void attack()
    {
            //enemyScript = currentTarget.GetComponent<Enemy>();//grab the enemy script from the currentTarget/enemy
            //enemyScript.takeDamage(damage);//deal damage 
    }

    protected bool checkSuccess(int successMod, int failMod)
    {
        int successChance = successMod - failMod;
        int result = Random.Range(1, 101);
        if (successChance >= result)
        {
            Debug.Log("Our unit was hit or critted " + successChance);
            return true;
        }
        Debug.Log("Our unit was not hit or critted " + successChance);
        return false;

    }

    protected virtual void decideIfShouldAttack()
    {
        StartCoroutine("IdecideIfShouldAttack");
    }

    IEnumerator IdecideIfShouldAttack()
    {
        while (true)
        {
            //Debug.Log("We decided whether to attack or not");
            if (Time.time >= lastAttackTime + timeBetweenAttacks && currentTarget)
            {
                attack();
                lastAttackTime = Time.time;//update to current time
            }
            yield return decideIfShouldAttackWaiter;//note that this will limit attack speed to 20 attacks a second.. probably a good thing though
        }
    }
    
}
