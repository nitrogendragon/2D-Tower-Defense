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
    private int rows;//always do an odd number
    private int columns;//always do an odd number
    private int verticalOffset;
    private int horizontalOffset;
    // Start is called before the first frame update
    void Start()
    {
        createBoard(3, 3, 1);
        initialBoardColor = boardTiles[0].GetComponent<SpriteRenderer>().color;
    }

    private void createBoard(int rows, int columns, int scale)
    {
        SetOffSets(rows, columns);
        for(int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject newBoardPiece = Instantiate(boardTile);
                newBoardPiece.transform.position = new Vector3((horizontalOffset + column) * scale, (verticalOffset + row) * scale, 0);
                boardTiles.Add(newBoardPiece);
            }
        }
    }

    private void SetOffSets(int rows, int columns)
    {
        verticalOffset = -(rows - 1) / 2;//subtract 1 because we will always use an odd number to keep things centered
        horizontalOffset = -(columns - 1) / 2;//subtract 1 because we will always use an odd number to keep things centered
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
                //set to 
                if(selectedBoardTile.transform.position == boardTiles[i].transform.position)
                {
                    targetIndex = i;
                    Debug.Log("The selectedBoardTilesIndex is: " + targetIndex);
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
