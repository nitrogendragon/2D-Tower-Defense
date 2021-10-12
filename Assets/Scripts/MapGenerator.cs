using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject mapTile;

    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    private int mapSize;

    private List<GameObject> mapTiles = new List<GameObject>();
    private List<GameObject> pathTiles = new List<GameObject>();

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
        for (int i = 0; i < mapWidth - 1; i++)
        {
            edgeTiles.Add(mapTiles[i]);
        }
        return edgeTiles;
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

                newTile.transform.position = new Vector2(x, y);
            }
        }

        //Creates list to hold top and bottom tiles and fills them
        List<GameObject> topEdgeTiles = getTopEdgeTiles();
        List<GameObject> bottomEdgeTiles = getBottomEdgeTiles();

        //handle beginning and end tiles setup
        GameObject startTile;
        GameObject endTile;

        //Determine indexes
        int rand1= Random.Range(0, mapWidth);
        int rand2 = Random.Range(0, mapWidth);

        //Apply indexes to set start and end tile
        startTile = topEdgeTiles[rand1];
        endTile = bottomEdgeTiles[rand2];
        Destroy(startTile);
        Destroy(endTile);
    }
}
