using UnityEngine;

public interface IFocusable
{
    public void EnterFocus(Transform pointerPosition, IPlaceable pointerHeldObject = null);
    public void ExitFocus();
}
