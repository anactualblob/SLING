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

    [Header("Text Fields")]
    [SerializeField] private Text scoreText = null;
    [SerializeField] private Text ropesText = null;
    [SerializeField] private Text gameOverScoreText = null;

    private Dictionary<GameObject, IAnimatedCanvas> dict = new Dictionary<GameObject, IAnimatedCanvas>();

    private IAnimatedCanvas _startAnimator;
    private IAnimatedCanvas _gameAnimator;
    private IAnimatedCanvas _gameOverAnimator;

    public bool _coroutineActive = false;


    public void Awake()
    {
        _startAnimator = startCanvas.GetComponent<IAnimatedCanvas>();
        if (_startAnimator != null) dict.Add(startCanvas, _startAnimator);

        _gameAnimator = gameCanvas.GetComponent<IAnimatedCanvas>();
        if (_gameAnimator != null) dict.Add(gameCanvas, _gameAnimator);

        _gameOverAnimator = gameOverCanvas.GetComponent<IAnimatedCanvas>();
        if (_gameOverAnimator != null) dict.Add(gameOverCanvas, _gameOverAnimator);
    }


    public void DisplayScore(int score)
    {
        scoreText.text = score.ToString("00,0");
        gameOverScoreText.text = score.ToString("00,0");
    }

    public void DisplayRopes(int ropes)
    {
        ropesText.text = ropes.ToString();
    }


    public void RestartButtonCallback()
    {
        GameManager.RestartGame();
    }


    private void EnsureAllCoroutinesAreFinished()
    {
        if (_coroutineActive)
        {
            StopAllCoroutines();
            _startAnimator?.ResetState();
            _gameAnimator?.ResetState();
            _gameOverAnimator?.ResetState();
        }
    }

    public void ShowStartCanvas()
    {
        EnsureAllCoroutinesAreFinished();

        if (startCanvas.activeInHierarchy)
            return;


        if (gameCanvas.activeInHierarchy)
            StartCoroutine(CoroutineSwitchCanvas(startCanvas, gameCanvas));
        else if (gameOverCanvas.activeInHierarchy)
            StartCoroutine(CoroutineSwitchCanvas(startCanvas, gameOverCanvas));
        else
            StartCoroutine(CoroutineSwitchCanvas(startCanvas, null));
    }

    public void ShowGameOverCanvas()
    {
        EnsureAllCoroutinesAreFinished();

        if (gameOverCanvas.activeInHierarchy)
            return;

        if (gameCanvas.activeInHierarchy)
            StartCoroutine(CoroutineSwitchCanvas(gameOverCanvas, gameCanvas));
        else if (startCanvas.activeInHierarchy)
            StartCoroutine(CoroutineSwitchCanvas(gameOverCanvas, startCanvas));
        else
            StartCoroutine(CoroutineSwitchCanvas(gameOverCanvas, null));
    }

    public void ShowGameCanvas()
    {
        EnsureAllCoroutinesAreFinished();

        if (gameCanvas.activeInHierarchy)
            return;

        if (gameOverCanvas.activeInHierarchy)
            StartCoroutine(CoroutineSwitchCanvas(gameCanvas, gameOverCanvas));
        else if (startCanvas.activeInHierarchy)
            StartCoroutine(CoroutineSwitchCanvas(gameCanvas, startCanvas));
        else
            StartCoroutine(CoroutineSwitchCanvas(gameCanvas, null));
    }

    IEnumerator CoroutineSwitchCanvas(GameObject canvasToShow, GameObject canvasToHide)
    {
        _coroutineActive = true;

        if (canvasToHide != null)
        {
            if (dict.TryGetValue(canvasToHide, out IAnimatedCanvas hide))
                yield return StartCoroutine(hide.AnimateHide());
            else
                canvasToHide.SetActive(false);
        }

    
        if (dict.TryGetValue(canvasToShow, out IAnimatedCanvas show))
            yield return StartCoroutine(show.AnimateShow());
        else
            canvasToShow.SetActive(true);

        _coroutineActive = false;
    }

}
