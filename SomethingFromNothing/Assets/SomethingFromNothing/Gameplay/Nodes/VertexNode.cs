
using System.Collections;
using System.Collections.Generic;
using SomethingFromNothing;
using UnityEngine;

// The VertexNodes are the scoring piece of the game.
// They have two states (default and grown) and little behavior
public class VertexNode : MonoBehaviour
{
    bool isGrown;
    Vector2Int vertPos;
    HexGrid grid;

#nullable enable
    HexTile?[] adjacentTiles = new HexTile?[6];        // Null members are allowed
#nullable disable

    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        isGrown = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setGrid(HexGrid _grid)
    {
        grid = _grid;
    }

    public void setVertexPosition(Vector2Int _vertPos)
    {
        vertPos = _vertPos;
    }

    public void setAdjacentTiles(HexTile?[] _adjacentTiles)
    {
        adjacentTiles = _adjacentTiles;
    }

    public void setAdjacentTile(int index, HexTile tile)
    {
        adjacentTiles[index] = tile;
    }

    // When a new tile is placed adjacent, check to see if growth
    // is allowed and grow if able
    public void AddTile()
    {

        // Three different vertices must meet to be a valid growth location
        if (adjacentTiles[0] != null && adjacentTiles[1] != null && adjacentTiles[2] != null)
        {
            if (adjacentTiles[0].hasVertices() && adjacentTiles[1].hasVertices() && adjacentTiles[2].hasVertices())
            {
                EVertexType tile0Vert = adjacentTiles[0].GetVertex(0);
                EVertexType tile1Vert = adjacentTiles[1].GetVertex(1);
                EVertexType tile2Vert = adjacentTiles[2].GetVertex(2);
            
                // If they are all different, grow!
                if (tile0Vert!=tile1Vert && tile1Vert!=tile2Vert && tile2Vert!=tile0Vert)
                {
                    Grow();
                }
            } else return;
        } else return;
    }

    // When growth is possible, change the sprite and state to match
    void Grow()
    {
        isGrown = true;
        spriteRenderer.enabled = true;

        foreach (HexTile? tile in adjacentTiles)
        {
            if (tile != null)
                tile.Grow();
        }
    }

    // When an adjacent tile burns, so does this node.
    public void Burn()
    {
        // reset state
        isGrown = false;

        // hide sprite
        spriteRenderer.enabled = false;
    }

    public bool IsGrown()
    {
        return isGrown;
    }
}
