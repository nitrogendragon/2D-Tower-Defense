using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject boardTile;
    private List<GameObject> boardTiles = new List<GameObject>();//store all our board pieces in here
    private SpriteRenderer lastHoveredBoardSpriteRenderer;
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

    private void HighLightHoveredTile(Collider2D collider)
    {
        if(lastHoveredBoardSpriteRenderer)
        {
            lastHoveredBoardSpriteRenderer.color = initialBoardColor;
        }
        lastHoveredBoardSpriteRenderer = collider.GetComponent<SpriteRenderer>();
        lastHoveredBoardSpriteRenderer.color = Color.green;
    }

    private void TestRayCastHitBoardTile()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            
        if(hit.collider && hit.collider.tag == "BoardTile")
        {
            HighLightHoveredTile(hit.collider);
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("We hit a board tile " + hit.collider.name);
            }
        }
        
        
    }
    // Update is called once per frame
    void Update()
    {
        TestRayCastHitBoardTile();
    }
}
