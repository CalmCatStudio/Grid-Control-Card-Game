using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickable : IFocusable
{
    public void Clicked(PointerClickAction pointerAction, Vector3 pointerPosition);
}
