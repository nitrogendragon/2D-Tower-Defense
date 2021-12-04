using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDeploymentCards : MonoBehaviour
{
    private List<string> unitNames = new List<string>(){"gunner", "mage", "priest", "knight", "samurai", "ninja", "dancer"};
    private List<int> unitCosts = new List<int>() {250,300,400,200,350,600,700};
    //private List<Sprite> unitSprites = new List<Sprite>();
    public List<GameObject> unitDeploymentCards = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //for(int i=0;i < 7; i++) 
        //{
        //    Debug.Log(unitNames[i]);
        //    Debug.Log(unitCosts[i]);
        //}
        //grab/create all the unit info needed to the up the cards
        //setup our deployment cards
        GameObject[] tempList = GameObject.FindGameObjectsWithTag("UnitDeploymentCards");
        for(int i = 0; i < 7; i++)
        {
            foreach(GameObject unitDCard in tempList)
            {

                if (unitDCard.name == "UnitSelectionButton" + (i+1).ToString())
                {
                    unitDCard.GetComponent<UnitDeploymentCard>().setName(unitNames[i]); 
                    unitDCard.GetComponent<UnitDeploymentCard>().setCost(unitCosts[i]); 
                    unitDCard.GetComponent<UnitDeploymentCard>().setID(i); 
                    unitDeploymentCards.Add(unitDCard);
                    //Debug.Log(unitDeploymentCards[(i)].name);

                }
            }
        }
        Debug.Log(unitDeploymentCards + " " + unitDeploymentCards.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
