using System.Collections;
using System.Collections.Generic;
using SomethingFromNothing;
using UnityEngine;

// HexTiles are the core game piece. They have 4 states (empty, grown, burned, and default) and
// carry the heavy-lifting for scoring and game state management.
public class HexTile : MonoBehaviour
{
    // TODO: Reference to 6 adjacent tiles. Null is allowed
    HexTile[] adjacentTiles;

    // TODO: Reference to 3 adjacent nodes. Null is NOT allowed
    VertexNode[] adjacentNodes;

    // TODO: Enumerate state
    EVertexType[] vertexPermutation;

    float secondsLeft;

    void Start()
    {
        secondsLeft = -1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Each second the tile is alive, add to the score for each adjacent tree
    // and update the UI timer display
    void OnTimerTick()
    {

    }

    // On placement, expand the node system and notify adjacent pieces of its existence.
    // Also start the timer
    void Place()
    {

    }

    // If a tree grows on an adjacent node, this tile becomes grown and adjacent
    // spaces become valid for new tile placement (All null references to adjacent
    // tiles are populated with new empty tiles)
    void Grow()
    {

    }

    // When the time is up, destroy adjacent trees and any spaces that might 
    // become invalid. Damage adjacent tiles and update texture to fire
    void Burn()
    {

    }

    public EVertexType GetVertex(int index) {
        return vertexPermutation[index];
    }

}
