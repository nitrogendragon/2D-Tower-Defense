using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{
    public GameObject basicEnemy;

    public float timeBetweenWaves;
    public float timeBeforeRoundStarts;
    public float timeVariable;
    public bool isRoundGoing;
    public bool isIntermission;
    public bool isStartOfRound;
    public int round;
    private int i = 0;
    private WaitForSeconds manageRoundsWaiter;
    private WaitForSeconds spawnEnemiesWaiter;


    private void Start()
    {
        isRoundGoing = false;
        isIntermission = false;
        isStartOfRound = true;
        timeVariable = 0;
        round = 1;//always start with the first round
        manageRoundsWaiter = new WaitForSeconds(1f);
        spawnEnemiesWaiter = new WaitForSeconds(1f);
        manageRounds();
    }

    private void manageRounds()
    {
        StartCoroutine("ImanageRounds");
    }

    IEnumerator ImanageRounds()
    {
        while (true)//this should be fine since it's an enumerator/coRoutine and runs independently/side by side with the other scripts
        {
            if (isStartOfRound && timeVariable >= timeBeforeRoundStarts)
            {
                //Debug.Log("The first round has started");
                isStartOfRound = false;
                isIntermission = false;
                isRoundGoing = true;
                timeVariable = 0;
                SpawnEnemies();
                

            }
            else if (isIntermission && timeVariable >= timeBeforeRoundStarts)
            {
                //Debug.Log("Round " + round + " has started.");
                isStartOfRound = false;
                isIntermission = false;
                isRoundGoing = true;
                timeVariable = 0;
                SpawnEnemies();
                
            }
            else if (isRoundGoing && timeVariable >= timeBetweenWaves)
            {
                if (EnemiesManager.enemies.Count > 0)
                {
                    //do nothing-don't try starting next round
                }
                else
                {
                    //Debug.Log("Round " + round + " has ended.");
                    isIntermission = true;
                    isRoundGoing = false;
                    timeVariable = 0;
                    round += 1;
                }
            }
            yield return manageRoundsWaiter;
        }
    }

    private void SpawnEnemies()
    {
        StartCoroutine("ISpawnEnemies");
    }

    IEnumerator ISpawnEnemies()//only runs after the yield waitforseconds period has come instead of every frame/update
    {
        for (i = 0; i < round; i++)
        {
            GameObject newEnemy = Instantiate(basicEnemy, MapGenerator.startTile.transform.position, Quaternion.identity);
            yield return spawnEnemiesWaiter;
        }
    }


    private void Update()
    {
        timeVariable += Time.deltaTime;//run here outside of manageRounds for now since deltaTime doesn't work in coRoutine like it does here.
    }
}
