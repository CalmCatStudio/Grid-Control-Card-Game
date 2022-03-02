using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointerReader : MonoBehaviour
{
    private Camera mainCam = null;
    private Vector2 position = Vector2.zero;

    private IFocusable
        hoverTarget = null;
    private IPlaceable
        placeableHeld = null;


    private void Awake()
    {
        mainCam = Camera.main;
    }

    /// <summary>
    /// Called by Unity Input System.
    /// Sets pointerHoverTarget if the Pointer is over valid target
    /// </summary>
    public void OnMovePointer(InputAction.CallbackContext context)
    {
        // Checking for mainCam is needed to avoid an error on scene switching
        if (mainCam != null)
        {
            position = mainCam.ScreenToWorldPoint(context.ReadValue<Vector2>());
            // Transform gets moved with the pointer, because it is used for positioning things later.
            transform.position = position;
            var hit = Physics2D.Raycast(position, Vector3.forward, 50);
            if (hit)
            {                
                // Check if this is a hoverable Target.
                if (hit.collider.TryGetComponent(out IFocusable newHoverTarget))
                {
                    // If we already had a hover target; Then tell the old target it is no longer in focus.
                    if (newHoverTarget != hoverTarget)
                    {
                        ResetPointerFocus();
                        hoverTarget = newHoverTarget;
                    }
                    // Tell the new target it is in focus.
                    hoverTarget.EnterFocus(transform, placeableHeld);
                }
            }
            else
            {
                ResetPointerFocus();
            }
        }
    }

    /// <summary>
    /// Exits focus from the current hoverTarget, and sets it to null.
    /// </summary>
    private void ResetPointerFocus()
    {
        if (hoverTarget != null)
        {
            hoverTarget.ExitFocus();
            hoverTarget = null;
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        // Click Down
        if (context.started)
        {
            Click(PointerClickAction.ClickDown);
        }
        else if (context.canceled) /* Click Up */
        {
            Click(PointerClickAction.ClickUp);
        }
    }

    private void Click(PointerClickAction action)
    {
        if (hoverTarget == null) { return; }

        // Try to click the target. Exit the method if it isn't clickable.
        var pointerClickTarget = hoverTarget as IClickable;
        if (pointerClickTarget == null) { return; }
        pointerClickTarget.Clicked(action, position);

        HandleSelectable(action, pointerClickTarget);
    }

    private void HandleSelectable(PointerClickAction action, IClickable pointerClickTarget)
    {
        switch (action)
        {
            case PointerClickAction.ClickDown:
                if (placeableHeld == null)
                {
                    // If the pointerTarget is an IPlaceable; Then select it.
                    placeableHeld = pointerClickTarget as IPlaceable;
                    if (placeableHeld != null)
                    {
                        placeableHeld.Selected();
                    }
                }
                break;
            case PointerClickAction.ClickUp:
                // If an IPlaceable is currently held.
                if (placeableHeld != null)
                {
                    // If the pointerClickTarget is an IPlacementArea; Then place it
                    // Otherwise Unselect it
                    var placementArea = pointerClickTarget as IPlacementArea;
                    if (placementArea != null)
                    {
                        placementArea.PlaceSelectable(placeableHeld);
                    }
                    else
                    {
                        placeableHeld.Unselected();
                    }
                    placeableHeld = null;
                }
                break;
        }
    }
}

public enum PointerClickAction
{
    ClickDown,
    ClickUp
}
