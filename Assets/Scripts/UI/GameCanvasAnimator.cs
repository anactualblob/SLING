using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameCanvasAnimator : MonoBehaviour, IAnimatedCanvas
{
    [SerializeField] private RectTransform ropes = null;
    [SerializeField] private RectTransform score = null;

    private Vector2 _ropesPos;
    private Vector2 _scorePos;

    private float _height = 1000;

    public void Init()
    {
        //gameObject.SetActive(true);
        _ropesPos = ropes.anchoredPosition;
        _scorePos = score.anchoredPosition;
    }

    public IEnumerator AnimateShow()
    {
        gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();

        // put both up outside the screen
        score.anchoredPosition += Vector2.up * _height;
        ropes.anchoredPosition += Vector2.up * _height;

        // tween score down
        seq.Append(score.DOAnchorPos(_scorePos, 0.1f));

        // tween ropes down
        seq.Append(ropes.DOAnchorPos(_ropesPos, 0.1f));


        yield return seq.WaitForCompletion();

    }

    public IEnumerator AnimateHide()
    {
        gameObject.SetActive(false);
        yield return null;
    }

    public void ResetState()
    {
        ropes.anchoredPosition = _ropesPos;
        score.anchoredPosition = _scorePos;

        gameObject.SetActive(false);
    }

}
