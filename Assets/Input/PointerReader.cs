using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointerReader : MonoBehaviour
{
    private Camera mainCam = null;
    private Vector2 position = Vector2.zero;

    private IFocusable
        currentFocusTarget = null;
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
        if (mainCam == null)
        {
            return;
        }

        position = mainCam.ScreenToWorldPoint(context.ReadValue<Vector2>());
        // Transform gets moved with the pointer, because it is used for positioning things later.
        transform.position = position;

        EvaluatePointerFocus();

        if (placeableHeld != null)
        {
            placeableHeld.Move(position);
        }
    }

    private void EvaluatePointerFocus()
    {
        var hit = Physics2D.Raycast(position, Vector3.forward, 50);
        if (!hit)
        {
            ResetPointerFocus();
            return;
        }

        if (!hit.collider.TryGetComponent(out IFocusable newFocusTarget))
        {
            return;
        }

        if (newFocusTarget != currentFocusTarget)
        {
            ResetPointerFocus();
            currentFocusTarget = newFocusTarget;
        }
        currentFocusTarget.OnEnterFocus(transform, placeableHeld);
    }

    /// <summary>
    /// Exits focus from the current hoverTarget, and sets it to null.
    /// </summary>
    private void ResetPointerFocus()
    {
        if (currentFocusTarget != null)
        {
            currentFocusTarget.OnExitFocus();
            currentFocusTarget = null;
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
        if (currentFocusTarget == null) { return; }

        // Try to click the target. Exit the method if it isn't clickable.
        var pointerClickTarget = currentFocusTarget as IClickable;
        if (pointerClickTarget == null) { return; }
        pointerClickTarget.Clicked(action, position);

        HandleSelectable(action, pointerClickTarget);
    }

    private void HandleSelectable(PointerClickAction action, IClickable clickTarget)
    {
        switch (action)
        {
            case PointerClickAction.ClickDown:
                if (placeableHeld == null)
                {
                    // If the pointerTarget is an IPlaceable; Then select it.
                    placeableHeld = clickTarget as IPlaceable;
                }
                break;
            case PointerClickAction.ClickUp:
                // If an IPlaceable is currently held.
                if (placeableHeld != null)
                {
                    // If the pointerClickTarget is an IPlacementArea; Then place it
                    // Otherwise Unselect it
                    var placeCurrentlyFocused = clickTarget as IPlacementArea;
                    if (placeCurrentlyFocused != null)
                    {
                        placeCurrentlyFocused.Place(placeableHeld);
                    }
                    else
                    {
                        placeableHeld.Place();
                    }
                    placeableHeld = null;
                    ResetPointerFocus();
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
