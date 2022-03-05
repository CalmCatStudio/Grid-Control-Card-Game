using UnityEngine;

public interface IFocusable
{
    public void OnEnterFocus(Transform pointerPosition, IPlaceable pointerHeldObject = null);
    public void OnExitFocus();
}
