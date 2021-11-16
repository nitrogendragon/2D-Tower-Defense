using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public UnitManager unitManager;

    public ShopManager shopManager;//reference to the shopManager script

    public GameObject basicUnitObject;

    public GameObject validPlacementTileObject;//for referencing the validtile prefab

    public GameObject invalidPlacementTileObject;//for referencing our invalidtile prefab

    private GameObject placementTile;//our actual placementTile that will be instantiated and moved around

    private GameObject currentUnitDeploying;//the current unit we are deploying

    private GameObject dummyPlacement;

    public Camera cam;

    private GameObject hoverTile;

    public LayerMask mask;//specify layer to use in Physics.Raycast

    public LayerMask UnitMask;// specify layer of units for detecting places we can't place a new unit

    public bool isDeploying = false;

    private void Start()
    {
        //startDeploying();   
    }

    /*gets our mouse position, creates a 2D raycast which hits wherever our mouse is and checks for appropriate conditions to determine if we 
     * are hovering over a valid tile to place a unit and sets hoverTile to the gameObject that the collider hits if so*/
    public void GetCurrentHoverTile()
    {
        Vector2 mousePosition = GetMousePosition();
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, new Vector2(0, 0), 0.1f, mask, -100, 100);
        //make sure we hit a collider and that its a mapTile and that it wasn't a pathTile
        if (hit.collider && MapGenerator.mapTiles.Contains(hit.collider.gameObject))
        {
            hoverTile = hit.collider.gameObject;//set the hoverTile to the one our raycast hit
        }
    }

    //tell the game we are deploying and instantiate a unit but remove its scripts so its just a placeholder sprite/gameobject
    public void startDeploying(GameObject UnitToDeploy)
    {
        isDeploying = true;

        currentUnitDeploying = UnitToDeploy;
        placementTile = Instantiate(validPlacementTileObject);
        dummyPlacement = Instantiate(UnitToDeploy);
        //Debug.Log(dummyPlacement);

        if (dummyPlacement.GetComponent<Unit>())
        {
            Destroy(dummyPlacement.GetComponent<Unit>());
        }
        if (dummyPlacement.GetComponent<UnitRotaton>())
        {
            Destroy(dummyPlacement.GetComponent<UnitRotaton>());
        }
    }

    public bool CheckForUnit()
    {
        bool UnitOnSlot = false;

        //Vector2 mousePosition = GetMousePosition();
        if (isDeploying) {
            foreach (GameObject unit in unitManager.activeUnits)
            {
                if (unit.transform.position == dummyPlacement.transform.position)
                {
                    UnitOnSlot = true;
                }
            }
        }
        //RaycastHit2D hit = Physics2D.Raycast(mousePosition, new Vector2(0, 0), UnitMask, -100, 100);

        //if (hit.collider)
        //{
        //    Debug.Log("We are hitting another unit");
        //    UnitOnSlot = true;
        //}
        return UnitOnSlot;

    }

    public void PlaceUnit()
    {
        //make sure we have a tile, it doesn't have a unit on it and it's not a pathTile
        if (hoverTile && !CheckForUnit() && !MapGenerator.pathTiles.Contains(hoverTile) && shopManager.canBuyUnit(currentUnitDeploying) == true)
        {
            GameObject newUnitObject = Instantiate(currentUnitDeploying);
            newUnitObject.layer = LayerMask.NameToLayer("unit");
            newUnitObject.transform.position = hoverTile.transform.position;
            unitManager.addToActiveUnits(newUnitObject);
            shopManager.buyUnit(currentUnitDeploying);
            endDeploying();
        }
        else
        {
            Debug.Log("probably insufficient funds or maybe something else is stopping us from placing the unit here.");
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
            Debug.Log("we started attempting to deploy our unit 1.");
            startDeploying(currentUnitDeploying);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("we stopped attempting to deploy our unit 1.");
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
