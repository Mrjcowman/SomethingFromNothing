using System.Collections;
using System.Collections.Generic;
using SomethingFromNothing;
using UnityEngine;


// HexTiles are the core game piece. They have 4 states (empty, grown, burned, and default) and
// carry the heavy-lifting for scoring and game state management.
public class HexTile : MonoBehaviour
{

    const float MAX_SECONDS_LEFT = 10;

    // Reference to 6 adjacent tiles clockwise from 1:00 position. Null is allowed
    HexTile[] adjacentTiles;

    // Reference to 3 adjacent nodes clockwise from 12:00 position. Null is NOT allowed
    VertexNode[] adjacentNodes;

    // TODO: Enumerate state
    enum ETileState {
        Empty,
        Grown,
        Burned,
        Default
    }
    ETileState tileState;

    EVertexType[] vertexPermutation;

    public Sprite spriteEmpty, spriteGrown, spriteBurned, spriteDefault;
    SpriteRenderer spriteRenderer;

    float secondsLeft;

    void Start()
    {
        secondsLeft = -1;
        tileState = ETileState.Empty;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = spriteEmpty;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Each second the tile is alive, add to the score for each adjacent tree
    // and update the UI timer display
    void OnTimerTick()
    {
        if (adjacentNodes[0].IsGrown()) SFNGame.AddScore(1);
        if (adjacentNodes[1].IsGrown()) SFNGame.AddScore(1);
        if (adjacentNodes[2].IsGrown()) SFNGame.AddScore(1);
    }

    // On placement, expand the node system and notify adjacent pieces of its existence.
    // Also start the timer
    public void Place()
    {
        tileState = ETileState.Default;
        spriteRenderer.sprite = spriteDefault;
        secondsLeft = MAX_SECONDS_LEFT;
        //TODO: update sprite and start timer
    }

    // If a tree grows on an adjacent node, this tile becomes grown and adjacent
    // spaces become valid for new tile placement (All null references to adjacent
    // tiles are populated with new empty tiles)
    public void Grow()
    {
        tileState = ETileState.Grown;
        spriteRenderer.sprite = spriteGrown;
        //TODO: update sprite and create empty spaces
    }

    // When the time is up, destroy adjacent trees and any spaces that might 
    // become invalid. Damage adjacent tiles and update texture to fire
    public void Burn()
    {
        tileState = ETileState.Burned;
        spriteRenderer.sprite = spriteBurned;
        //TODO: update sprite and burn adjacent tiles
    }

    public EVertexType GetVertex(int index) {
        return vertexPermutation[index];
    }

    public bool IsEmpty() {
        return tileState == ETileState.Empty;
    }

    public bool IsGrown() {
        return tileState == ETileState.Grown;
    }

}
