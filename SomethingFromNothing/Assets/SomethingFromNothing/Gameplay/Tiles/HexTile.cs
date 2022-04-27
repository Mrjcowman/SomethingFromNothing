using System.Collections;
using System.Collections.Generic;
using SomethingFromNothing;
using UnityEngine;

// HexTiles are the core game piece. They have 4 states (empty, grown, burned, and default) and
// carry the heavy-lifting for scoring and game state management.
public class HexTile : MonoBehaviour
{
    // Reference to 6 adjacent tiles clockwise from 1:00 position. Null is allowed
    HexTile[] adjacentTiles;

    // Reference to 3 adjacent nodes clockwise from 12:00 position. Null is NOT allowed
    VertexNode[] adjacentNodes;

    // TODO: Enumerate state
    public enum ETileState {
        Empty,
        Grown,
        Burned,
        Default
    }
    ETileState tileState;

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

    public ETileState GetTileState() {
        return tileState;
    }

}
