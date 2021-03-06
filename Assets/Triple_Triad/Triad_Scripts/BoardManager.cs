using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject boardTile;
    public GameObject MobCard;
    private List<GameObject> boardTiles = new List<GameObject>();//store all our board pieces in here
    private SpriteRenderer lastHoveredBoardTileSpriteRenderer;
    private GameObject selectedBoardTile;
    private Color initialBoardColor;
    public int rows = 4;
    public int columns = 4;
    private int verticalOffset;
    private int horizontalOffset;
    // Start is called before the first frame update
    void Start()
    {
        createBoard(rows, columns, 1);
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
        if((float)rows%2.0f == 0)
        {
            verticalOffset = -rows / 2;
            horizontalOffset = -columns / 2;
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
