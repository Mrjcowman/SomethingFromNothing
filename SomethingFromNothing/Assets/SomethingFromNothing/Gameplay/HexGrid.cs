using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SomethingFromNothing;

public class HexGrid : MonoBehaviour
{
    public HexTile pfHexTile;
    GridLayout gridLayout;
    Dictionary<Vector2Int, HexTile> tiles;

    // Start is called before the first frame update
    void Start()
    {
        gridLayout = transform.GetComponent<GridLayout>();
        tiles = new Dictionary<Vector2Int, HexTile>();

        // TODO: populate the game start state
        CreateEmptyTile(new Vector2Int(0,0));

        SFNGame.ResetScore();
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
                tiles[cellPos].Place();
                Debug.Log("Valid position!");
            } else {
                Debug.Log("Invalid position!");
            }
        }
    }

    void CreateEmptyTile(Vector2Int cellPos) {
        Vector3 worldPos = gridLayout.CellToWorld((Vector3Int)cellPos);
        tiles[cellPos] = Instantiate(pfHexTile, worldPos, Quaternion.identity);
    }
}
