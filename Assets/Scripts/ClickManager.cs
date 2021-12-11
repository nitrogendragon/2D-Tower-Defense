using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ClickManager : MonoBehaviour
{
    
    public  GameObject unitStatsUI;
    private GameObject selectedUnit = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void DeselectUnit()
    {
        selectedUnit = null;
        unitStatsUI.GetComponent<UnitStatsUI>().setSelectedUnit(selectedUnit,"");
    }

    public void HandleSelectionAndUnitMovement()
    {
            
            //Get the mouse position on the screen and send a raycast into the game world from that position.
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            //handle unit selection, always select new unit if we click on one at least for now(magic/healing eventually needs considerations
            if (hit.collider && hit.collider.CompareTag("Player"))
            {
                //Debug.Log("WE COLLIDED with a player and should be updating");
                //set our selectedUnit gameObject
                selectedUnit = hit.collider.gameObject;
                unitStatsUI.GetComponent<UnitStatsUI>().setSelectedUnit(selectedUnit, hit.collider.GetComponent<Unit>().nameText.text);
                
            }
            //handle movement
            else if (hit.collider && selectedUnit)
            {
                if (hit.collider.CompareTag("ValidMovementTile") || hit.collider.CompareTag("Enemy"))
                {
                    selectedUnit.GetComponent<BasicUnit>().movementTarget = hit.collider.transform.position;
                }

            }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            HandleSelectionAndUnitMovement();
        }
        if(Input.GetMouseButtonDown(0) && selectedUnit && selectedUnit.GetComponent<BasicUnit>().getCastingStatus() == false)
        {
            DeselectUnit();
        }
        
    }
}
