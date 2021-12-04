using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ClickManager : MonoBehaviour
{
    public Text loggingText;
    public  GameObject unitStatsUI;
    private GameObject selectedUnit = null;
    // Start is called before the first frame update
    void Start()
    {
        loggingText.text = "Updated on Start";
    }

    public void DeselectUnit()
    {
        selectedUnit = null;
        loggingText.text = "unit deselected";
    }

    public void HandleSelectionAndUnitMovement()
    {
        
            //Get the mouse position on the screen and send a raycast into the game world from that position.
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            //handle unit selection, always select new unit if we click on one at least for now(magic/healing eventually needs considerations
            if (hit.collider && hit.collider.CompareTag("Player"))
            {
                //set our selectedUnit gameObject
                selectedUnit = hit.collider.gameObject;
                unitStatsUI.GetComponent<UnitStatsUI>().setSelectedUnit(selectedUnit, hit.collider.GetComponent<Unit>().nameText.text);
                loggingText.text = hit.collider.GetComponent<Unit>().nameText.text;
            }
            //handle movement and resetting selectedUnit
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
    }
}
