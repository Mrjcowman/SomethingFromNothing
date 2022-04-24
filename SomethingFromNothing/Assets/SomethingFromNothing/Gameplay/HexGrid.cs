using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    GridLayout gridLayout;

    // Start is called before the first frame update
    void Start()
    {
        gridLayout = transform.GetComponent<GridLayout>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get clicked coordinates
        if (Input.GetMouseButtonDown(0)) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(string.Format("Mouse Coordinates: ({0}, {1})", pos.x, pos.y));
            Vector3Int cellPos = gridLayout.WorldToCell(pos);
            Debug.Log(string.Format("Cell Coordinates: [{0}. {1}]", cellPos.x, cellPos.y, cellPos.z));
        }
    }
}
