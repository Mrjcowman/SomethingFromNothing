using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SomethingFromNothing;

public class HexGrid : MonoBehaviour
{
    public HexTile pfHexTile;
    GridLayout gridLayout;
    Dictionary<Vector2Int, HexTile> tiles;
    Dictionary<Vector2Int, VertexNode> nodes;   // Node positions are written as the cell directly to the south of the node

    // Start is called before the first frame update
    void Start()
    {
        gridLayout = transform.GetComponent<GridLayout>();
        tiles = new Dictionary<Vector2Int, HexTile>();

        // TODO: populate the game start state
        CreateEmptyTile(new Vector2Int(0,0));

        SFNGame.ResetScore();
    }

    // Update is called once per frame
    void Update()
    {
        // Get clicked coordinates
        if (Input.GetMouseButtonDown(0)) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2Int cellPos = (Vector2Int) gridLayout.WorldToCell(pos);   // conversion drops the z
            
            // If there is an open space in thatt position, place the tile
            if (tiles.ContainsKey(cellPos) && tiles[cellPos].IsEmpty()) {
                tiles[cellPos].Place();
                Debug.Log("Valid position!");
            } else {
                Debug.Log("Invalid position!");
            }
        }
    }

    // Create a new tile at cellPos and initialize its data to be an empty space
    void CreateEmptyTile(Vector2Int cellPos)
    {
        Vector3 worldPos = gridLayout.CellToWorld((Vector3Int)cellPos);
        tiles[cellPos] = Instantiate(pfHexTile, worldPos, Quaternion.identity);
        tiles[cellPos].setGrid(this);
        tiles[cellPos].setCellPosition(cellPos);
    }

    public VertexNode[] getAdjacentNodes(Vector2Int cellPos)
    {
        return new VertexNode[3];
    }

    #nullable enable
    public HexTile?[] getAdjacentTiles(Vector2Int cellPos)
    {
        HexTile?[] adjacent = new HexTile?[6];
        adjacent[0] = getTile(cellPos + Vector2Int.up);
        adjacent[1] = getTile(cellPos + Vector2Int.right);
        adjacent[2] = getTile(cellPos + Vector2Int.down + Vector2Int.right);
        adjacent[3] = getTile(cellPos + Vector2Int.down);
        adjacent[4] = getTile(cellPos + Vector2Int.left);
        adjacent[5] = getTile(cellPos + Vector2Int.up + Vector2Int.left);

        return adjacent;
    }

    HexTile? getTile(Vector2Int cellPos)
    {
        if (tiles.ContainsKey(cellPos))
            return tiles[cellPos];
        else
            return null;
    }
    
}
