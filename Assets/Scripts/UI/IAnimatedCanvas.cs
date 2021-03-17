using System.Collections;

public interface IAnimatedCanvas
{
    IEnumerator AnimateShow();
    IEnumerator AnimateHide();

    void ResetState();
}
