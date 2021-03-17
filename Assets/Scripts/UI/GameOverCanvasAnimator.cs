using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameOverCanvasAnimator : MonoBehaviour, IAnimatedCanvas
{
    [Header("Object References")]
    [SerializeField] private RectTransform button = null;
    [SerializeField] private RectTransform textContainer = null;
    [SerializeField] private Image bg = null;

    private Vector2 _buttonPos;
    private Vector2 _textContainerPos;

    private float _animDuration = 0.2f;
    private float _baseAlpha = 0.8f;

    private void Awake()
    {
        _buttonPos = button.anchoredPosition;
        _textContainerPos = textContainer.anchoredPosition;
    }

    public IEnumerator AnimateShow()
    {
        gameObject.SetActive(true);
        yield return null;
    }

    public IEnumerator AnimateHide()
    {
        Sequence seq = DOTween.Sequence();

        // move button down
        seq.Append(button.DOAnchorPos(_buttonPos - Vector2.up * 1000, _animDuration));

        // move text up
        seq.Join(textContainer.DOAnchorPos(_textContainerPos + Vector2.up * 1000, _animDuration));

        // fade bg ?
        //Color clr = bg.color;
        //clr.a = 0;
        //seq.Join(bg.DOColor(clr, _animDuration));


        yield return seq.WaitForCompletion();

        gameObject.SetActive(false);

        // reset positions and color
        button.anchoredPosition = _buttonPos;
        textContainer.anchoredPosition = _textContainerPos;
        //clr.a = _baseAlpha;
        //bg.color = clr;
    }

    public void ResetState()
    {
        button.anchoredPosition = _buttonPos;
        textContainer.anchoredPosition = _textContainerPos;

        gameObject.SetActive(false);
    }


}
