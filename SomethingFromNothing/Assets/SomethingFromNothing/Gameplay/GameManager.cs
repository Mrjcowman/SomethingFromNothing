using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;

public class GameManager : MonoBehaviour
{
    public HexGrid pfHexGrid;

    public CanvasRenderer upcomingTileDisplay;
    public TextMeshProUGUI scoreText;
    
    HexGrid grid;
    int score = 0;
    float timeSinceStart;
    int upcomingVertexPermutationIndex;


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
        timeSinceStart = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceStart += Time.deltaTime;
        scoreText.text = string.Format("Score: {0}", score);
    }

    // TODO: handle game end better
    public void EndGame()
    {
        SceneManager.LoadScene(0);
    }
}
