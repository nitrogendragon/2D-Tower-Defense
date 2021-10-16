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

    private void Start()
    {
        isRoundGoing = false;
        isIntermission = false;
        isStartOfRound = true;
        timeVariable = 0;
        round = 1;//always start with the first round
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
            yield return new WaitForSeconds(1f);
        }
    }

    private void Update()
    {
        timeVariable += Time.deltaTime;
        if (isStartOfRound && timeVariable >= timeBeforeRoundStarts)
        {
            Debug.Log("The first round has started");
            isStartOfRound = false;
            isIntermission = false;
            isRoundGoing = true;
            timeVariable = 0;
            SpawnEnemies();
            return;//don't check others
            
        }
        else if (isIntermission && timeVariable >= timeBeforeRoundStarts)
        {
            Debug.Log("Round " + round + " has started.");
            isStartOfRound = false;
            isIntermission = false;
            isRoundGoing = true;
            timeVariable = 0;
            SpawnEnemies();
            return;
        }
        else if (isRoundGoing && timeVariable >= timeBetweenWaves)
        {
            if (Enemies.enemies.Count > 0)
            {
                //do nothing-don't try starting next round
            }
            else
            {
                Debug.Log("Round " + round + " has ended.");
                isIntermission = true;
                isRoundGoing = false;
                timeVariable = 0;
                round += 1;
            }
        }
    }
}
