using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public ShopManager shopManager;//reference to the shopManager script

    public UnitDeploymentCards unitDeploymentCards;// reference to the unitDeloymentCards script

    public ClickManager clickManager;//reference to the clickManager objects clickManager script

    public GameObject basicUnitObject;

    public GameObject dummyUnitSprite;

    public GameObject validPlacementTileObject;//for referencing the validtile prefab

    public GameObject invalidPlacementTileObject;//for referencing our invalidtile prefab

    private GameObject placementTile;//our actual placementTile that will be instantiated and moved around

    private GameObject currentUnitDeploying;//the current unit we are deploying

    private GameObject dummyPlacement;

    private int currentUnitDeployingID;

    private string currentUnitDeployingName;

    public Camera cam;

    private GameObject hoverTile;

    public LayerMask mask;//specify layer to use in Physics.Raycast

    public LayerMask UnitMask;// specify layer of units for detecting places we can't place a new unit

    public bool isDeploying = false;

 

    /*gets our mouse position, creates a 2D raycast which hits wherever our mouse is and checks for appropriate conditions to determine if we 
     * are hovering over a valid tile to place a unit and sets hoverTile to the gameObject that the collider hits if so*/
    public void GetCurrentHoverTile()
    {
        //Get the mouse position on the screen and send a raycast into the game world from that position.
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        
        //make sure we hit a collider and that its a mapTile and that it wasn't a pathTile
        if (hit.collider && MapGenerator.mapTiles.Contains(hit.collider.gameObject))
        {
            hoverTile = hit.collider.gameObject;//set the hoverTile to the one our raycast hit
        }
        else { hoverTile = null; }
    }

    //tell the game we are deploying and instantiate a unit but remove its scripts so its just a placeholder sprite/gameobject
    public void startDeploying(int unitID)
    {
        isDeploying = true;
        clickManager.DeselectUnit();//we don't want an active unit to do things while deploying
        currentUnitDeployingID = unitID;
        currentUnitDeployingName = unitDeploymentCards.unitDeploymentCards[unitID].GetComponent<UnitDeploymentCard>().GetName();
        //currentUnitDeployingName = unitDeploymentCards.unitDeploymentCards[unitID].GetComponent<UnitDeploymentCard>().GetUnit4Sprite();
        currentUnitDeploying = basicUnitObject;//temp til i have different sprites then code above will get implemented
        currentUnitDeploying.GetComponent<Unit>().spriteRenderer.sprite =
            unitDeploymentCards.unitDeploymentCards[unitID].GetComponent<UnitDeploymentCard>().GetSprite();
        placementTile = Instantiate(validPlacementTileObject);
        dummyPlacement = Instantiate(dummyUnitSprite);
        if (hoverTile)
        {
            dummyPlacement.transform.position = hoverTile.transform.position;
        }
        
    }



    public bool CheckForUnit()
    {
        bool UnitOnSlot = false;

        //Vector2 mousePosition = GetMousePosition();
        if (isDeploying && UnitManager.activeUnits != null) {
            foreach( GameObject unit in UnitManager.activeUnits)
            {
                if (unit.transform.position == dummyPlacement.transform.position)
                {
                    UnitOnSlot = true;
                }
            }
        }
        return UnitOnSlot;

    }

    public void PlaceUnit()
    {
        //make sure we have a tile, it doesn't have a unit on it and it's not a pathTile and we have enough money and we are trying to deploy
        if (hoverTile && !CheckForUnit() && !MapGenerator.pathTiles.Contains(hoverTile) && 
            shopManager.canBuyUnit(currentUnitDeployingID) == true && isDeploying)
        {
            GameObject newUnitObject = Instantiate(currentUnitDeploying);
            newUnitObject.GetComponent<Unit>().nameText.text = currentUnitDeployingName;
            newUnitObject.GetComponent<Unit>().setUnitID(currentUnitDeployingID);
            newUnitObject.layer = LayerMask.NameToLayer("unit");
            newUnitObject.transform.position = hoverTile.transform.position;
            shopManager.buyUnit(unitDeploymentCards.unitDeploymentCards[currentUnitDeployingID].GetComponent<UnitDeploymentCard>().GetCost());
            endDeploying();
        }
        
    }

    public void endDeploying()
    {
        if (dummyPlacement)//just extra precaution but it should be there
        {
            Destroy(dummyPlacement);//don't need our placeholder anymore
            Destroy(placementTile);//don't need our placementTile anymore either
        }
        isDeploying = false;
    }

    public void updateDummyPositionAndPlacementTile()
    {
        //if we are deploying, have a dummyPlacement object
        if (isDeploying && dummyPlacement)
        {
            GetCurrentHoverTile();//find and determine the hoverTile
            if (hoverTile)//make sure hoverTile isn't null
            {
                dummyPlacement.transform.position = hoverTile.transform.position;//update dummyPlacement objects position
                
                if (MapGenerator.pathTiles.Contains(hoverTile) || CheckForUnit())
                {
                    Destroy(placementTile);
                    placementTile = Instantiate(invalidPlacementTileObject);
                }
                else
                {
                    Destroy(placementTile);
                    placementTile = Instantiate(validPlacementTileObject);
                }
                placementTile.transform.position = hoverTile.transform.position;//update placementTile objects position too
            }
            
        }
    }

    public void checkforDeploymentInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            
            PlaceUnit();
        }
    }

    public void checkForUnitSelection()
    {
        if(!isDeploying && Input.GetKeyDown(KeyCode.Alpha1)){
            isDeploying = true;
            currentUnitDeploying = basicUnitObject;//temp code for now
            //Debug.Log("we started attempting to deploy our unit 1.");
            startDeploying(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Debug.Log("we stopped attempting to deploy our unit 1.");
            endDeploying();
        }
    }

    public Vector2 GetMousePosition()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }


    public void Update()
    {
        updateDummyPositionAndPlacementTile();
        checkForUnitSelection();
        checkforDeploymentInput();
    }
}
