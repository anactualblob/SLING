using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject startCanvas = null;
    [SerializeField] private GameObject gameCanvas = null;
    [SerializeField] private GameObject gameOverCanvas = null;

    [Header("Text")]
    [SerializeField] private Text scoreText = null;
    [SerializeField] private Text ropesText = null;


    


    public void DisplayScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void DisplayRopes(int ropes)
    {
        ropesText.text = ropes.ToString();
    }


    public void RestartButtonCallback()
    {
        GameManager.RestartGame();
    }




    public void ShowStartCanvas()
    {
        if (startCanvas.activeInHierarchy)
            return;

        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
    }

    public void ShowGameCanvas()
    {
        if (gameCanvas.activeInHierarchy)
            return;

        startCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
    }

    public void ShowGameOverCanvas()
    {
        if (gameOverCanvas.activeInHierarchy)
            return;

        startCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);
    }
}
