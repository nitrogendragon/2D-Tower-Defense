using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ClickManager : MonoBehaviour
{
    
    public  GameObject unitStatsUI;
    public GameObject selectedUnit = null;
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
        bool canMove = true;
        //handle unit selection, always select new unit if we click on one at least for now(magic/healing eventually needs considerations
        if (selectedUnit)
        {
            foreach (GameObject unit in UnitManager.activeUnits)
            {
                if (hit.collider && hit.collider.transform.position == unit.transform.position)
                {
                    canMove = false;
                }
            }
            //if (canMove && (hit.collider.CompareTag("ValidMovementTile") || hit.collider.CompareTag("Enemy")))
            if (canMove && hit.collider && hit.collider.CompareTag("ValidMovementTile"))
            {
                selectedUnit.GetComponent<BasicUnit>().movementTarget = hit.collider.transform.position;
            }
        }

        else if (hit.collider)
        {
            foreach(GameObject unit in UnitManager.activeUnits)
            {
                float distance = Vector2.Distance(hit.collider.transform.position, unit.transform.position);
                if(distance <= .2f)
                {
                    selectedUnit = unit;
                    unitStatsUI.GetComponent<UnitStatsUI>().setSelectedUnit(selectedUnit, selectedUnit.GetComponent<Unit>().nameText.text);
                    break;
                }
            }
            

        }
        
        

        
            //handle movement
            //else if (hit.collider && selectedUnit)
            //{
            //    if (hit.collider.CompareTag("ValidMovementTile") || hit.collider.CompareTag("Enemy"))
            //    {
            //        selectedUnit.GetComponent<BasicUnit>().movementTarget = hit.collider.transform.position;
            //    }

            //}

        }

   

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(1))
        {
            HandleSelectionAndUnitMovement();
        }
        if (Input.GetKeyDown(KeyCode.Tab) && selectedUnit)
        {
            DeselectUnit();
        }

        

    }
}
