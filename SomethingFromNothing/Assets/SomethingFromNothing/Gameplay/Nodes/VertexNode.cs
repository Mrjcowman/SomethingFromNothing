using System.Collections;
using System.Collections.Generic;
using SomethingFromNothing;
using UnityEngine;

// The VertexNodes are the scoring piece of the game.
// They have two states (default and grown) and little behavior
public class VertexNode : MonoBehaviour
{
    bool isGrown;
    HexTile[] adjacentTiles;        // Null members are allowed

    // Start is called before the first frame update
    void Start()
    {
        isGrown = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // When a new tile is placed adjacent, check to see if growth
    // is allowed and grow if able
    void AddTile()
    {
        // Three different vertices must meet to be a valid growth location
        if (adjacentTiles[0] && adjacentTiles[1] && adjacentTiles[2])
        {
            EVertexType tile0Vert = adjacentTiles[0].GetVertex(0);
            EVertexType tile1Vert = adjacentTiles[1].GetVertex(1);
            EVertexType tile2Vert = adjacentTiles[2].GetVertex(2);
            
            // If they are all different, grow!
            if (tile0Vert!=tile1Vert && tile1Vert!=tile2Vert && tile2Vert!=tile0Vert)
            {
                Grow();
            }
        } else
            return;
    }

    // When growth is possible, change the sprite and state to match
    void Grow()
    {
        isGrown = true;
        // TODO: show sprite
    }

    // When an adjacent tile burns, so does this node.
    void Burn()
    {
        // reset state
        isGrown = false;

        // hide sprite
    }

    public bool IsGrown()
    {
        return isGrown;
    }
}
