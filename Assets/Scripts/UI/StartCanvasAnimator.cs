using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StartCanvasAnimator : MonoBehaviour, IAnimatedCanvas
{
    [Header("Object References")]
    [SerializeField] private Image bg = null;
    [SerializeField] private RectTransform title = null;
    [SerializeField] private RectTransform touchToStart = null;

    [Header("Animation Parameters")]
    [SerializeField, Range(0,1)] private float backgroundAlpha = 0.5f;

    private Vector2 _titleStartPos;
    private Vector2 _touchTextStartPos;

    private float _animationTime = 0.2f;
    private float _height = 1000.0f;

    private void Awake()
    {
        gameObject.SetActive(true);
        _titleStartPos = title.anchoredPosition;
        _touchTextStartPos = touchToStart.anchoredPosition;
    }

    public IEnumerator AnimateShow()
    {
        gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();

        // material should have some alpha, not tweened
        Color clr = bg.color;
        clr.a = backgroundAlpha;
        bg.color = clr;

        // place title up and tween it down
        title.anchoredPosition = _titleStartPos + Vector2.up * _height;
        seq.Append(title.DOAnchorPos(_titleStartPos, 0.2f));

        // place touch text down and tween it up
        touchToStart.anchoredPosition = _touchTextStartPos - Vector2.up * _height;
        seq.Join(touchToStart.DOAnchorPos(_touchTextStartPos, 0.2f));

        // wait for completion
        yield return seq.WaitForCompletion();
    }

    public IEnumerator AnimateHide()
    {
        Sequence seq = DOTween.Sequence();


        // tween the title up
        seq.Append(title.DOAnchorPos(_titleStartPos + Vector2.up * _height, _animationTime));

        // tween the touch text down
        seq.Join(touchToStart.DOAnchorPos(_touchTextStartPos - Vector2.up * _height, _animationTime));


        // tween bg's alpha to 0
        Color clr = bg.color;
        clr.a = 0;
        seq.Join(bg.DOColor(clr, _animationTime));

        // wait for completion
        yield return seq.WaitForCompletion();

        // then deactivate GO, and reset positions
        gameObject.SetActive(false);
        title.anchoredPosition = _titleStartPos;
        touchToStart.anchoredPosition = _touchTextStartPos;
        clr.a = backgroundAlpha;
        bg.color = clr;

    }

    public void ResetState()
    {
        title.anchoredPosition = _titleStartPos;

        touchToStart.anchoredPosition = _touchTextStartPos;

        Color clr = bg.color;
        clr.a = backgroundAlpha;
        bg.color = clr;

        gameObject.SetActive(false);
    }


}
