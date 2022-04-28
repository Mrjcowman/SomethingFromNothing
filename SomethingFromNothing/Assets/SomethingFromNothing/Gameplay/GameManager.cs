using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class GameManager : MonoBehaviour
{
    int score = 0;
    int upcomingVertexPermutationIndex;

    public HexGrid pfHexGrid;
    HexGrid grid;
    public CanvasRenderer upcomingTileDisplay;

    public TextMeshProUGUI scoreText;

    public void AddScore(int points)
    {
        score += points;
    }

    public void ResetScore()
    {
        score = 0;
    }

    public int GetUpcomingVertexPermutationIndex()
    {
        return upcomingVertexPermutationIndex;
    }

    public void NewUpcomingTile()
    {
        upcomingVertexPermutationIndex = Random.Range(0,6);
        float angle = 120f * (upcomingVertexPermutationIndex / 2);
        float xScale = upcomingVertexPermutationIndex % 2 == 0 ? 1f : -1f;
        upcomingTileDisplay.gameObject.transform.rotation = Quaternion.identity;
        upcomingTileDisplay.gameObject.transform.Rotate(new Vector3(0, 0, -angle));
        upcomingTileDisplay.gameObject.transform.localScale = new Vector3(xScale, 1, 1);
    }


    // Start is called before the first frame update
    void Start()
    {
        NewUpcomingTile();
        grid = Instantiate(pfHexGrid, Vector3.zero, Quaternion.identity);
        grid.Initialize(this);
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = string.Format("Score: {0}", score);
    }
}
