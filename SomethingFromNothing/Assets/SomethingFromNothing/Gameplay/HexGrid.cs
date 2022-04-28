using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SomethingFromNothing;

public class HexGrid : MonoBehaviour
{

    // PREFABS
    public HexTile pfHexTile;
    public VertexNode pfVertexNode;

    // FIELDS
    GameManager gameManager;
    GridLayout gridLayout;
    Dictionary<Vector2Int, HexTile> tiles;
    Dictionary<Vector2Int, VertexNode> nodes;   // Node positions are written as the cell directly to the south of the node
    int activeTiles;
    int viableSpaces;

    bool firstTick = true;


    // ================================================================================================================================
    // GRID METHODS

    void Start()
    {
        gridLayout = transform.GetComponent<GridLayout>();

        // populate the game start state
        CreateStartTile(Vector2Int.zero);
        CreateStartTile(Vector2Int.up);
        CreateStartTile(Vector2Int.up + Vector2Int.left);
    }

    void Update()
    {
        if (firstTick) {
            firstTick = false;
            return;
        }
        // Get clicked coordinates
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log(string.Format("viable spaces: {0}", viableSpaces));
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
            gameManager.EndGame();
        }

        // TODO: handle game end better
        if (viableSpaces < 1) {
            Debug.Log("Game end!!");
            gameManager.EndGame();
        }
    }

    /// <summary>
    /// Set up a clean hex grid with starting tiles
    /// </summary>
    /// <param name="_gameManager">The owning GameManager</param>
    public void Initialize(GameManager _gameManager)
    {
        gameManager = _gameManager;
        tiles = new Dictionary<Vector2Int, HexTile>();
        nodes = new Dictionary<Vector2Int, VertexNode>();
        activeTiles = 0;
        viableSpaces = 0;

        gameManager.ResetScore();
    }

    /// <summary>
    /// Handle mouse input
    /// </summary>
    void HandleInput()
    {
        // TODO: implement
    }

    public void AddActiveTile()
    {
        activeTiles++;
        RemoveViableSpace();
    }

    public void RemoveActiveTile()
    {
        activeTiles--;
    }

    /// <summary>
    /// Decrement the number of viable spaces when an empty HexTile is lost or state changed
    /// </summary>
    public void RemoveViableSpace()
    {
        viableSpaces--;
    }
    // TILE HANDLERS

    #nullable enable
    /// <summary>
    /// Look up the tile at a given position
    /// </summary>
    /// <param name="cellPos">The coordinates of the grid cell for the tile</param>
    /// <returns>The tile at the position, or null if no tile exists there</returns>
    HexTile? GetTile(Vector2Int cellPos)
    {
        if (tiles.ContainsKey(cellPos))
            return tiles[cellPos];
        else
            return null;
    }
    #nullable disable

    /// <summary>
    /// Create a new tile at cellPos and initialize its data to be an empty space
    /// </summary>
    /// <param name="cellPos">The coordinates of the grid cell for the tile</param>
    void CreateEmptyTile(Vector2Int cellPos)
    {
        Vector3 worldPos = gridLayout.CellToWorld((Vector3Int)cellPos);
        tiles[cellPos] = Instantiate(pfHexTile, worldPos, Quaternion.identity);
        tiles[cellPos].Initialize(gameManager, this, cellPos, false);
        viableSpaces++;
    }

    /// <summary>
    /// Create a new tile at cellPos and initialize its data to be a starting tile
    /// </summary>
    /// <param name="cellPos">The coordinates of the grid cell for the tile</param>
    void CreateStartTile(Vector2Int cellPos)
    {
        Vector3 worldPos = gridLayout.CellToWorld((Vector3Int)cellPos);
        tiles[cellPos] = Instantiate(pfHexTile, worldPos, Quaternion.identity);
        tiles[cellPos].Initialize(gameManager, this, cellPos, true);
        viableSpaces++; // These immediately get removed when tiles grow
    }

    #nullable enable
    /// <summary>
    /// Look up all tiles adjacent to the tile at the given grid cell
    /// </summary>
    /// <param name="cellPos">The coordinates of the tile being checked for adjacency</param>
    /// <returns>An array of 6 adjacent HexTiles or nulls for non-existent tiles</returns>
    public HexTile?[] GetAdjacentTilesToTile(Vector2Int cellPos)
    {
        HexTile?[] adjacent = new HexTile?[6];
        adjacent[0] = GetTile(GetAdjacentCell(cellPos, EHexDirection.One));
        adjacent[1] = GetTile(GetAdjacentCell(cellPos, EHexDirection.Three));
        adjacent[2] = GetTile(GetAdjacentCell(cellPos, EHexDirection.Five));
        adjacent[3] = GetTile(GetAdjacentCell(cellPos, EHexDirection.Seven));
        adjacent[4] = GetTile(GetAdjacentCell(cellPos, EHexDirection.Nine));
        adjacent[5] = GetTile(GetAdjacentCell(cellPos, EHexDirection.Eleven));

        return adjacent;
    }
    #nullable disable

    /// <summary>
    /// Look up all VertexNodes adjacent to the tile at the given grid cell. If any nodes don't
    /// exist, creates nodes for those positions.
    /// </summary>
    /// <param name="cellPos">The coordinates of the tile being checked for adjacency</param>
    /// <returns>An array of 3 adjacent VertexNodes</returns>
    public VertexNode[] GetAdjacentNodes(Vector2Int cellPos)
    {
        return new VertexNode[]
        {
            GetNode(cellPos),
            GetNode(GetAdjacentCell(cellPos, EHexDirection.Five)),
            GetNode(GetAdjacentCell(cellPos, EHexDirection.Seven))
        };
    }

    #nullable enable
    /// <summary>
    /// Create empty HexTiles in any spaces that are undefined adjacent to the given grid cell.
    /// </summary>
    /// <param name="cellPos">The coordinates of the tile being checked for adjacency</param>
    public void PopulateAdjacentTiles(Vector2Int cellPos)
    {
        HexTile?[] adjacent = GetAdjacentTilesToTile(cellPos);
        for(int i = 0; i<6; i++)
        {
            if (adjacent[i] == null) {
                CreateEmptyTile(GetAdjacentCell(cellPos, (EHexDirection)i));
            }
        }
    }
    #nullable disable

    // NODE HANDLERS
    
    /// <summary>
    /// Return the node at requested position, or a new node if it doesn't exist.
    /// </summary>
    /// <param name="vertPos">The coordinates of the grid cell whose top vertex matches with the node</param>
    /// <returns>The VertexNode at the given position</returns>
    VertexNode GetNode(Vector2Int vertPos)
    {
        if (!nodes.ContainsKey(vertPos))
            CreateNode(vertPos);
        
        return nodes[vertPos];
    }

    /// <summary>
    /// Create a new VertexNode at vertPos and initialize its data
    /// </summary>
    /// <param name="vertPos">The coordinates of the grid cell whose top vertex matches with the node</param>
    void CreateNode(Vector2Int vertPos)
    {
        Vector3 worldPos = gridLayout.CellToWorld((Vector3Int)vertPos)+Vector3.up;
        nodes[vertPos] = Instantiate(pfVertexNode, worldPos, Quaternion.identity);
        nodes[vertPos].Initialize(gameManager, this, vertPos);
    }

    #nullable enable
    /// <summary>
    /// Look up all Tiles adjacent to the VertexNode at the top of the given grid cell.
    /// </summary>
    /// <param name="vertPos">The coordinates of the grid cell whose top vertex matches with the node</param>
    /// <returns>An array of 6 adjacent HexTiles or nulls for non-existent tiles</returns>
    public HexTile?[] GetAdjacentTilesToNode(Vector2Int vertPos)
    {
        // The order here is reversed to make indexing in the match step easier
        HexTile?[] adjacent = new HexTile?[3];
        adjacent[0] = GetTile(vertPos);
        adjacent[1] = GetTile(GetAdjacentCell(vertPos, EHexDirection.Eleven));
        adjacent[2] = GetTile(GetAdjacentCell(vertPos, EHexDirection.One));

        return adjacent;
    }
    #nullable disable

    

    /// <summary>
    /// Hexagonal directions given as clock positions pointing to the centers of adjacent hexes
    /// </summary>
    public enum EHexDirection
    {
        One,
        Three,
        Five,
        Seven,
        Nine,
        Eleven
    }

    /// <summary>
    /// Helper to find hex coordinates in a direction
    /// </summary>
    /// <param name="startCell">The coordinates of the cell at the origin of the motion</param>
    /// <param name="direction">The hexagonal direction being searched</param>
    /// <returns>A Vector2Int of the coordinates at the looked-up position</returns>
    public static Vector2Int GetAdjacentCell(Vector2Int startCell, EHexDirection direction)
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

    // ================================================================================================================================
    
}
