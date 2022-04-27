using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SomethingFromNothing;

public class HexGrid : MonoBehaviour
{
    public HexTile pfHexTile;
    public VertexNode pfVertexNode;
    public GameManager gameManager;

    GridLayout gridLayout;
    Dictionary<Vector2Int, HexTile> tiles;
    Dictionary<Vector2Int, VertexNode> nodes;   // Node positions are written as the cell directly to the south of the node

    // Start is called before the first frame update
    void Start()
    {
        gridLayout = transform.GetComponent<GridLayout>();
        tiles = new Dictionary<Vector2Int, HexTile>();
        nodes = new Dictionary<Vector2Int, VertexNode>();

        // TODO: populate the game start state
        CreateStartTile(Vector2Int.zero);
        CreateStartTile(Vector2Int.up);
        CreateStartTile(Vector2Int.up + Vector2Int.left);

        gameManager.ResetScore();
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
                tiles[cellPos].Place(gameManager.GetUpcomingVertexPermutationIndex());
                Debug.Log(gameManager.GetUpcomingVertexPermutationIndex());
                gameManager.NewUpcomingTile();
            } else {
                Debug.Log("Invalid position!");
            }
        }

        if (Input.GetKeyDown("escape")) {
            Application.Quit();
        }
    }

    // Given as clock directions pointing to the center of the sides
    public enum EHexDirection {
        One,
        Three,
        Five,
        Seven,
        Nine,
        Eleven
    }

    // Helper to find hex coordinates in a direction
    public static Vector2Int getAdjacentCell(Vector2Int startCell, EHexDirection direction)
    {
        // Even cells are shifted right, Odd cells are shifted left
        if (startCell.y % 2 == 0) {
            switch (direction) {
                case EHexDirection.One:     return startCell + Vector2Int.up;
                case EHexDirection.Three:   return startCell + Vector2Int.right;
                case EHexDirection.Five:    return startCell + Vector2Int.down;
                case EHexDirection.Seven:   return startCell + Vector2Int.down + Vector2Int.left;
                case EHexDirection.Nine:    return startCell + Vector2Int.left;
                case EHexDirection.Eleven:  return startCell + Vector2Int.up + Vector2Int.left;
                default: return startCell;
            }
        } else {
            switch (direction) {
                case EHexDirection.One:     return startCell + Vector2Int.up + Vector2Int.right;
                case EHexDirection.Three:   return startCell + Vector2Int.right;
                case EHexDirection.Five:    return startCell + Vector2Int.down + Vector2Int.right;
                case EHexDirection.Seven:   return startCell + Vector2Int.down;
                case EHexDirection.Nine:    return startCell + Vector2Int.left;
                case EHexDirection.Eleven:  return startCell + Vector2Int.up;
                default: return startCell;
            }
        }
    }

    // Create a new tile at cellPos and initialize its data to be an empty space
    void CreateEmptyTile(Vector2Int cellPos)
    {
        Vector3 worldPos = gridLayout.CellToWorld((Vector3Int)cellPos);
        tiles[cellPos] = Instantiate(pfHexTile, worldPos, Quaternion.identity);
        tiles[cellPos].setGrid(this);
        tiles[cellPos].setGameManager(gameManager);
        tiles[cellPos].setCellPosition(cellPos);
        tiles[cellPos].setAdjacentNodes(getAdjacentNodes(cellPos));
        tiles[cellPos].setAdjacentTiles(getAdjacentTilesToTile(cellPos));
    }

    void CreateStartTile(Vector2Int cellPos)
    {
        CreateEmptyTile(cellPos);
        tiles[cellPos].MarkStartTile();
    }

    void CreateNode(Vector2Int vertPos)
    {
        Vector3 worldPos = gridLayout.CellToWorld((Vector3Int)vertPos)+Vector3.up;
        nodes[vertPos] = Instantiate(pfVertexNode, worldPos, Quaternion.identity);
        nodes[vertPos].setGrid(this);
        nodes[vertPos].setVertexPosition(vertPos);
        nodes[vertPos].setAdjacentTiles(getAdjacentTilesToNode(vertPos));
    }

    public VertexNode[] getAdjacentNodes(Vector2Int cellPos)
    {
        return new VertexNode[]
        {
            getNode(cellPos),
            getNode(getAdjacentCell(cellPos, EHexDirection.Five)),
            getNode(getAdjacentCell(cellPos, EHexDirection.Seven))
        };
    }

    // Returns the node at requested position, or a new node if it doesn't exist.
    VertexNode getNode(Vector2Int vertPos)
    {
        if (!nodes.ContainsKey(vertPos))
            CreateNode(vertPos);
        
        return nodes[vertPos];
    }

    #nullable enable
    public HexTile?[] getAdjacentTilesToTile(Vector2Int cellPos)
    {
        HexTile?[] adjacent = new HexTile?[6];
        adjacent[0] = getTile(getAdjacentCell(cellPos, EHexDirection.One));
        adjacent[1] = getTile(getAdjacentCell(cellPos, EHexDirection.Three));
        adjacent[2] = getTile(getAdjacentCell(cellPos, EHexDirection.Five));
        adjacent[3] = getTile(getAdjacentCell(cellPos, EHexDirection.Seven));
        adjacent[4] = getTile(getAdjacentCell(cellPos, EHexDirection.Nine));
        adjacent[5] = getTile(getAdjacentCell(cellPos, EHexDirection.Eleven));

        return adjacent;
    }

    public HexTile?[] getAdjacentTilesToNode(Vector2Int vertPos)
    {
        // The order here is reversed to make indexing in the match step easier
        HexTile?[] adjacent = new HexTile?[3];
        adjacent[0] = getTile(vertPos);
        adjacent[1] = getTile(getAdjacentCell(vertPos, EHexDirection.Eleven));
        adjacent[2] = getTile(getAdjacentCell(vertPos, EHexDirection.One));

        return adjacent;
    }

    public void PopulateAdjacentTiles(Vector2Int cellPos)
    {
        HexTile?[] adjacent = getAdjacentTilesToTile(cellPos);
        for(int i = 0; i<6; i++)
        {
            if (adjacent[i] == null) {
                CreateEmptyTile(getAdjacentCell(cellPos, (EHexDirection)i));
            }
        }
    }

    HexTile? getTile(Vector2Int cellPos)
    {
        if (tiles.ContainsKey(cellPos))
            return tiles[cellPos];
        else
            return null;
    }
    
}
