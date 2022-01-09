using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManagerNetwork : MonoBehaviour
{
    public GameObject boardTile;
    private List<GameObject> boardTiles = new List<GameObject>();//store all our board pieces in here
    private SpriteRenderer lastHoveredBoardTileSpriteRenderer;
    private GameObject selectedBoardTile;
    private Color initialBoardColor;
    private int rows = 6;//always do an odd number
    private int columns = 6;//always do an odd number
    private float verticalOffset;
    private float horizontalOffset;
    // Start is called before the first frame update
    

    public int GetRowsCount()
    {
        return rows;
    }
     
    public int GetColumnsCount()
    {
        return columns;
    }

    private void Start()
    {
        //CreateBoard(1.5f, 2f);//only for testing purposes when needed
    }

    public void CreateBoard(float xScale, float yScale)
    {
        SetOffSets(rows, columns);
        for(int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject newBoardPiece = Instantiate(boardTile);
                newBoardPiece.transform.position = new Vector3((horizontalOffset + column) * xScale, (verticalOffset + row) * yScale, 0);
                boardTiles.Add(newBoardPiece);
            }
        }
        initialBoardColor = boardTiles[0].GetComponent<SpriteRenderer>().color;
    }

    public void DestroyBoard()
    {
        foreach(GameObject tile in boardTiles)
        {
            Destroy(tile);
        }
    }

    private void SetOffSets(int rows, int columns)
    {
        if ((float)rows % 2.0f == 0.0f)
        {
            //Debug.Log("even board count");
            verticalOffset = -rows / 2 + 1f;
            horizontalOffset = -columns / 2 + .5f;
            return;
        }
        verticalOffset = -(rows - 1) / 2;//subtract 1 because we used an odd number to keep things centered
        horizontalOffset = -(columns - 1) / 2;//subtract 1 because we used an odd number to keep things centered
    }

    private void HighLightAndSelectHoveredTile(Collider2D collider)
    {
        if(lastHoveredBoardTileSpriteRenderer)
        {
            lastHoveredBoardTileSpriteRenderer.color = initialBoardColor;
        }
        selectedBoardTile = collider.gameObject;
        lastHoveredBoardTileSpriteRenderer = collider.GetComponent<SpriteRenderer>();
        lastHoveredBoardTileSpriteRenderer.color = Color.green;
    }

    public Vector3 GetSelectedBoardCoordinates()
    {
        if (selectedBoardTile)
        {
            return selectedBoardTile.transform.position;
        }
        else
        {
            return new Vector3(9999, 0, 0);//just as a general unusable coordinate
        }

    }

    public int GetSelectedBoardIndex()
    {
        //the index of the board we are hovering over
        int targetIndex;
        if (selectedBoardTile)
        {
            for(int i = 0; i < boardTiles.Count; i++)
            {
                //check to which tile position our selected tile is at
                if(selectedBoardTile.transform.position == boardTiles[i].transform.position)
                {
                    targetIndex = i;
                    //Debug.Log("The selectedBoardTilesIndex is: " + targetIndex);
                    return targetIndex;
                }
            }
        }
        //we will want to account for this returning 9999 when we try to use the function so that it doesn't actually get set to 9999
        return 9999;
    }

    private void TestRayCastHitBoardTile()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            
        if(hit.collider && hit.collider.tag == "BoardTile")
        {
            HighLightAndSelectHoveredTile(hit.collider);
        }
        else if(selectedBoardTile)
        {
            //if we aren't hitting a board tile we don't want one as being selected
            selectedBoardTile.GetComponent<SpriteRenderer>().color = initialBoardColor;
            selectedBoardTile = null;
        }
        
        
    }
    // Update is called once per frame
    void Update()
    {
        TestRayCastHitBoardTile();
    }
}
