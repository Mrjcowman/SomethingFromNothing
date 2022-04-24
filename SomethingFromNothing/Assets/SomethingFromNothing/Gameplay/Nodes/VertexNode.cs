using System.Collections;
using System.Collections.Generic;
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

    }

    // When an adjacent tile burns, so does this node.
    void Burn()
    {

    }
}
