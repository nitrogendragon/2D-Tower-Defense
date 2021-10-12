using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject mapTile;
    private GameObject startTile;
    private GameObject endTile;

    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    private int mapSize;
    
    private List<GameObject> mapTiles = new List<GameObject>();
    private List<GameObject> pathTiles = new List<GameObject>();
    private List<GameObject> topEdgeTiles;
    private List<GameObject> bottomEdgeTiles;

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
        Destroy(startTile);
        Destroy(endTile);
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

        
        List<GameObject> topEdgeTiles = getTopEdgeTiles();
        List<GameObject> bottomEdgeTiles = getBottomEdgeTiles();

        //handle start end point generation
        GenerateStartEndPoints();
    }
}
