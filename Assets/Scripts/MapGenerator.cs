using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject mapTile;
    public static GameObject startTile;
    public static GameObject endTile;
    private GameObject currentTile;

    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;

    private int mapSize;
    private int currentIndex;
    private int nextIndex;
    
    public static List<GameObject> mapTiles = new List<GameObject>();
    public static List<GameObject> pathTiles = new List<GameObject>();
    private List<GameObject> topEdgeTiles;
    private List<GameObject> bottomEdgeTiles;

    public Color pathColor;
    public Color startColor;
    public Color endColor;

    //for path generation, have we reached x or y end points?
    private bool reachedX,reachedY = false;

    private void Start()
    {
        generateMap();
    }

    //Grabs the top row of tiles
    private List<GameObject> getTopEdgeTiles()
    {
        List<GameObject> edgeTiles = new List<GameObject>();

        for (int i = mapWidth * (mapHeight-1); i < mapHeight * mapWidth; i++)
        {
            edgeTiles.Add(mapTiles[i]);
        }
        return edgeTiles;
    }

    //Grabs the bottom row of tiles
    private List<GameObject> getBottomEdgeTiles()
    {
        List<GameObject> edgeTiles = new List<GameObject>();
        for (int i = 0; i < mapWidth; i++)
        {
            edgeTiles.Add(mapTiles[i]);
        }
        return edgeTiles;
    }

    //Move down
    private void movePathDown()
    {
        pathTiles.Add(currentTile);
        currentIndex = mapTiles.IndexOf(currentTile);
        nextIndex = currentIndex-mapWidth;//need to go down a row so we need to remove a row's worth of tiles from the index
        currentTile = mapTiles[nextIndex];
        Debug.Log(currentTile);
    }

    //Add currentTile to pathTiles list, set currentIndex to the mapTiles list index for the current Tile, set the next index to be 1 less, 
    // then set the current tile to the tile in MapTiles[nextIndex].. this will be the comment for explaining all move functions for pathing.
    private void movePathLeft()
    {
        pathTiles.Add(currentTile);
        currentIndex = mapTiles.IndexOf(currentTile);
        nextIndex = currentIndex-1;
        currentTile = mapTiles[nextIndex];
        Debug.Log(currentIndex.ToString());
    }
    
    private void movePathRight()
    {
        pathTiles.Add(currentTile);
        currentIndex = mapTiles.IndexOf(currentTile);
        nextIndex = currentIndex+1;
        currentTile = mapTiles[nextIndex];
        Debug.Log(currentIndex.ToString());
    }

    private void movePathUp()
    {
        pathTiles.Add(currentTile);
        currentIndex = mapTiles.IndexOf(currentTile);
        nextIndex = currentIndex + mapWidth;//need to go up a row so we need to add a row's worth of tiles from the index
        currentTile = mapTiles[nextIndex];
    }

    //Creates lists to hold top and bottom tiles and fills them
    private void GenerateStartEndPoints()
    {
        topEdgeTiles = getTopEdgeTiles();
        bottomEdgeTiles = getBottomEdgeTiles();

        //Determine indexes
        int rand1 = Random.Range(0, mapWidth);
        int rand2 = Random.Range(0, mapWidth);

        //Apply indexes to set start and end tile
        startTile = topEdgeTiles[rand1];
        endTile = bottomEdgeTiles[rand2];
    }

    //called within generateMap() to create a path for enemies to follow
    private void createPath()
    {
        currentTile = startTile;
        //just a temp so we go down one to start
        movePathDown();
        int loopCount = 0;
        while (reachedX == false)
        {
            loopCount += 1;
            if (currentTile.transform.position.x > endTile.transform.position.x)
            {
                movePathLeft();
            }
            else if (currentTile.transform.position.x < endTile.transform.position.x)
            {
                movePathRight();
            }
            else
            {
                reachedX = true;
            }
            if(loopCount == mapWidth)
            {
                Debug.Log("something went wrong in the X position moving");
                loopCount = 0;
                break;
            }
        }

        while (reachedY == false)
        {
            if (currentTile.transform.position.y > endTile.transform.position.y)
            {
                movePathDown();
            }
            else
            {
                reachedY = true;
                Debug.Log(endTile.transform.position);
                pathTiles.Add(endTile);
                Debug.Log(pathTiles[(pathTiles.Count-1)].transform.position);

            }
            if (loopCount == mapWidth)
            {
                Debug.Log("something went wrong in the Y position moving");
                loopCount = 0;
                break;
            }
        }

        foreach (GameObject tile in pathTiles)
        {
            tile.GetComponent<SpriteRenderer>().color = pathColor;
        }

        startTile.GetComponent<SpriteRenderer>().color = startColor;
        endTile.GetComponent<SpriteRenderer>().color = endColor;
    }

    //Generates a rectangular map of tiles
    private void generateMap()
    {
        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                GameObject newTile = Instantiate(mapTile);

                mapTiles.Add(newTile);

                newTile.transform.position = new Vector2(x-(mapWidth/2), y-(mapHeight/2));
            }
        }

        //handle start end point generation
        GenerateStartEndPoints();
        //make a path for enemies to move across
        createPath();
        
    }
}
