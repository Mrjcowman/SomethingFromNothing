using System.Collections;
using System.Collections.Generic;
using SomethingFromNothing;
using UnityEngine;


// HexTiles are the core game piece. They have 4 states (empty, grown, burned, and default) and
// carry the heavy-lifting for scoring and game state management.
public class HexTile : MonoBehaviour
{
    // Tunable variables
    const float MAX_SECONDS_LEFT = 5;

    HexGrid grid;

    #nullable enable
    HexTile?[] adjacentTiles = new HexTile?[6];     // Reference to 6 adjacent tiles clockwise from 1:00 position. Null is allowed
    #nullable disable
    VertexNode[] adjacentNodes;                     // Reference to 3 adjacent nodes clockwise from 12:00 position. Null is NOT allowed
    Vector2Int cellPosition;

    enum ETileState {
        Empty,
        Grown,
        Burned,
        Default
    }
    ETileState tileState;

    // Every possible permutation of vertices. Odd indices are mirrors of the preceding even indices
    static readonly EVertexType[][] possibleVertexPermutations = new EVertexType[][]
    {
        new EVertexType[] {EVertexType.Sun, EVertexType.Nutrients, EVertexType.Water},
        new EVertexType[] {EVertexType.Sun, EVertexType.Water, EVertexType.Nutrients},
        new EVertexType[] {EVertexType.Nutrients, EVertexType.Water, EVertexType.Sun},
        new EVertexType[] {EVertexType.Nutrients, EVertexType.Sun, EVertexType.Water},
        new EVertexType[] {EVertexType.Water, EVertexType.Sun, EVertexType.Nutrients},
        new EVertexType[] {EVertexType.Water, EVertexType.Nutrients, EVertexType.Sun}
    };
    EVertexType[] vertexPermutation;

    public Sprite spriteEmpty, spriteGrown, spriteBurned, spriteDefault;
    SpriteRenderer spriteRenderer;

    float secondsLeft;
    bool timerActive;


    void Start()
    {
        secondsLeft = -1;
        timerActive = false;
        tileState = ETileState.Empty;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = spriteEmpty;
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

            // TODO: render timer
        }
    }

    public void setGrid(HexGrid _grid)
    {
        grid = _grid;
    }

    public void setCellPosition(Vector2Int _cellPosition)
    {
        cellPosition = _cellPosition;
    }

    public void setAdjacentNodes(VertexNode[] _adjacentNodes)
    {
        adjacentNodes = _adjacentNodes;
    }

    public void setAdjacentTiles(HexTile?[] _adjacentTiles)
    {
        adjacentTiles = _adjacentTiles;
    }

    // Each second the tile is alive, add to the score for each adjacent tree
    void OnTimerTick()
    {
        if (secondsLeft <= 0) {
            secondsLeft = -1;
            timerActive = false;
            Burn();
            return;
        }

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
        timerActive = true;
    }

    // If a tree grows on an adjacent node, this tile becomes grown and adjacent
    // spaces become valid for new tile placement (All null references to adjacent
    // tiles are populated with new empty tiles)
    public void Grow()
    {
        tileState = ETileState.Grown;
        spriteRenderer.sprite = spriteGrown;
        //TODO: create empty spaces
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
