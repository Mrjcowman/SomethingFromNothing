using System.Collections;
using System.Collections.Generic;
using SomethingFromNothing;
using UnityEngine;
using TMPro;

// TODO: write better description
/// <summary>
/// HexTiles are the core game piece. They have 4 states (empty, grown, burned, and default) and
/// carry the heavy-lifting for scoring and game state management.
/// </summary>
public class HexTile : MonoBehaviour
{
    // CONSTANTS
    const float MAX_SECONDS_LEFT = 15;

    // RESOURCES
    public Sprite spriteEmpty, spriteGrown, spriteBurned, spriteDefault;

    // FIELDS
    GameManager gameManager;
    HexGrid grid;

    SpriteRenderer spriteRenderer;
    SpriteRenderer vertexRenderer;
    TextMeshProUGUI timerText;

    Vector2Int cellPos;
    bool isPlaced = false;
    ETileState tileState;

    // TODO: encapsulate timer as separate component
    bool timerActive;
    float secondsLeft;

    int? vertexPermutationIndex;

    VertexNode[] adjacentNodes;                     // Reference to 3 adjacent nodes clockwise from 12:00 position

    #nullable enable
    HexTile?[] adjacentTiles = new HexTile?[6];     // Reference to 6 adjacent tiles clockwise from 1:00 position
    #nullable disable

    // ================================================================================================================================
    // TILE METHODS

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = spriteEmpty;

        vertexRenderer = gameObject.transform.Find("TileNodes").GetComponent<SpriteRenderer>();
        vertexRenderer.enabled = false;

        timerText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        timerText.text = "";

        if(isPlaced) Place(0);
    }

    void Update()
    {
        if (timerActive) {
            int previousWholeSecond = (int) secondsLeft;
            secondsLeft -= Time.deltaTime;

            // If the truncated time changes, a whole second has ticked by
            if (previousWholeSecond != (int) secondsLeft) {
                OnTimerTick();
            }

            if (secondsLeft <= 0) {
                secondsLeft = -1;
                timerActive = false;
                Burn();
                return;
            }

            timerText.text = ((int) (secondsLeft + 1)).ToString();
        }
        else timerText.text = "";
    }

    public void Initialize(GameManager _gameManager, HexGrid _grid, Vector2Int _cellPos, bool _isPlaced)
    {
        gameManager = _gameManager;
        grid = _grid;

        cellPos = _cellPos;
        isPlaced = _isPlaced;

        secondsLeft = -1;
        timerActive = false;
        tileState = ETileState.Empty;

        SetAdjacentTiles(grid.GetAdjacentTilesToTile(cellPos));
        SetAdjacentNodes(grid.GetAdjacentNodes(cellPos));
    }

    public void SetGrid(HexGrid _grid)
    {
        grid = _grid;
    }

    public void SetCellPosition(Vector2Int _cellPosition)
    {
        cellPos = _cellPosition;
    }

    // Set my adjacent nodes and let them know I exist
    void SetAdjacentNodes(VertexNode[] _adjacentNodes)
    {
        adjacentNodes = _adjacentNodes;
        for (int i = 0; i < _adjacentNodes.Length; i++)
        {
            adjacentNodes[i].setAdjacentTile(i, this);
        }
    }

    #nullable enable
    // Set my adjacent tiles and let them know I exist
    void SetAdjacentTiles(HexTile?[] _adjacentTiles)
    {
        adjacentTiles = _adjacentTiles;
        for (int i = 0; i < _adjacentTiles.Length; i++)
        {
            if (adjacentTiles[i] != null)
            {
                // (i+3)%6 rotates the index 180 degrees
                adjacentTiles[i].setAdjacentTile((i+3)%6, this);
            }
        }
    }
    #nullable disable

    public void setAdjacentTile(int index, HexTile tile)
    {
        adjacentTiles[index] = tile;
    }

    // Each second the tile is alive, add to the score for each adjacent tree
    void OnTimerTick()
    {
        if (adjacentNodes[0].IsGrown()) gameManager.AddScore(1);
        if (adjacentNodes[1].IsGrown()) gameManager.AddScore(1);
        if (adjacentNodes[2].IsGrown()) gameManager.AddScore(1);

    }

    // On placement, notify adjacent pieces of its existence.
    // Also start the timer
    public void Place(int _vertexPermutationIndex)
    {
        tileState = ETileState.Default;
        spriteRenderer.sprite = spriteDefault;
        secondsLeft = MAX_SECONDS_LEFT;
        timerActive = true;

        vertexPermutationIndex = _vertexPermutationIndex;
        grid.AddActiveTile();


        foreach (VertexNode node in adjacentNodes)
        {
            node.CheckMatch();
        }

        // Rotate/mirror the vertices based on permutation
        float angle = 120f * (_vertexPermutationIndex/2);
        float xScale = _vertexPermutationIndex % 2 == 0 ? 1f: -1f;


        vertexRenderer.gameObject.transform.rotation = Quaternion.identity;
        vertexRenderer.gameObject.transform.Rotate(new Vector3(0, 0, -angle));
        vertexRenderer.gameObject.transform.localScale = new Vector3(xScale, 1, 1);
        vertexRenderer.enabled = true;

    }

    // If a tree grows on an adjacent node, this tile becomes grown and adjacent
    // spaces become valid for new tile placement (All null references to adjacent
    // tiles are populated with new empty tiles)
    public void Grow()
    {
        tileState = ETileState.Grown;
        spriteRenderer.sprite = spriteGrown;
        grid.PopulateAdjacentTiles(cellPos);
    }

    // When the time is up, destroy adjacent trees and any spaces that might 
    // become invalid. Damage adjacent tiles and update texture to fire
    public void Burn()
    {
        grid.RemoveActiveTile();
        tileState = ETileState.Burned;
        spriteRenderer.sprite = spriteBurned;

        vertexPermutationIndex = null;
        vertexRenderer.enabled = false;
        
        foreach (HexTile? tile in adjacentTiles)
        {
            if (tile != null)
                tile.Damage();
        }

        foreach (VertexNode node in adjacentNodes)
        {
            node.Burn();
        }
    }

    // Destroy stranded empty spaces and damage active tiles
    public void Damage()
    {
        if(tileState == ETileState.Empty)
        {
            bool stillValid = false;
            foreach (HexTile? tile in adjacentTiles)
            {
                if (tile != null) {
                    if (tile.IsGrown()){
                        stillValid = true;
                        break;
                    }
                }
            }

            if (!stillValid) {
                Debug.Log("Tile no longer valid!");
                grid.RemoveActiveTile();
                grid.RemoveViableSpace();
                Destroy(gameObject);
            }

        } else if (timerActive) {
            secondsLeft --;
            gameManager.AddScore(-1);
        }
    }

    public EVertexType GetVertex(int index) {
        if (vertexPermutationIndex == null)
            throw new System.Exception("Tried to access a vertex from a null cell!");
        
        return possibleVertexPermutations[(int)vertexPermutationIndex][index];
            
    }

    public bool IsEmpty() {
        return tileState == ETileState.Empty;
    }

    public bool IsGrown() {
        return tileState == ETileState.Grown;
    }

    public bool hasVertices()
    {
        return vertexPermutationIndex != null;
    }

    public void SetGameManager(GameManager _gameManager)
    {
        gameManager = _gameManager;
    }

    public void MarkStartTile()
    {
        isPlaced = true;
    }

    // ================================================================================================================================
    // ENUMS AND HELPERS
    
    /// <summary>
    /// 
    /// </summary>
    enum ETileState {
        Empty,
        Grown,
        Burned,
        Default
    }

    /// <summary>
    /// Every possible permutation of vertices. Odd indices are x-mirrors of the preceding even indices.
    /// Advancing pairs are rotated by 120 degrees clockwise
    /// </summary>
    static readonly EVertexType[][] possibleVertexPermutations = new EVertexType[][]
    {
        new EVertexType[] {EVertexType.Sun, EVertexType.Nutrients, EVertexType.Water},
        new EVertexType[] {EVertexType.Sun, EVertexType.Water, EVertexType.Nutrients},
        new EVertexType[] {EVertexType.Water, EVertexType.Sun, EVertexType.Nutrients},
        new EVertexType[] {EVertexType.Nutrients, EVertexType.Sun, EVertexType.Water},
        new EVertexType[] {EVertexType.Nutrients, EVertexType.Water, EVertexType.Sun},
        new EVertexType[] {EVertexType.Water, EVertexType.Nutrients, EVertexType.Sun},
    };

    // ================================================================================================================================
}