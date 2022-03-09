using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointerReader : MonoBehaviour
{
    private Camera mainCam = null;
    private Vector2 position = Vector2.zero;

    private IFocusable
        currentFocus = null;
    private IPlaceable
        heldObject = null;


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
        if (!mainCam)
        {
            return;
        }

        position = mainCam.ScreenToWorldPoint(context.ReadValue<Vector2>());
        // Transform gets moved with the pointer, because it is used for positioning things later.
        transform.position = position;

        EvaluatePointerFocus();


        if (heldObject != null)
        {
            heldObject.Move(position);
        }
    }

    private void EvaluatePointerFocus()
    {
        // Shoot Raycast at position, and exit if nothing is hit.
        var hit = Physics2D.Raycast(position, Vector3.forward, 10);
        if (!hit)
        {
            ResetPointerFocus();
            return;
        }

        // Exit if hit was not focusable.
        if (!hit.collider.TryGetComponent(out IFocusable newFocusTarget))
        {
            return;
        }

        // If the focus is new.
        if (newFocusTarget != currentFocus)
        {
            ResetPointerFocus();
            currentFocus = newFocusTarget;
        }
        currentFocus.OnEnterFocus(transform, heldObject);
    }

    /// <summary>
    /// Exits focus from the current hoverTarget, and sets it to null.
    /// </summary>
    private void ResetPointerFocus()
    {
        if (currentFocus != null)
        {
            currentFocus.OnExitFocus();
            currentFocus = null;
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        // Click Down
        if (context.started)
        {
            Click(PointerClickAction.ClickDown);
        }
        /* Click Up */
        else if (context.canceled) 
        {
            Click(PointerClickAction.ClickUp);
        }
    }

    private void Click(PointerClickAction action)
    {
        // Exit if there is no focus
        if (currentFocus == null) 
        {
            return; 
        }


        // Exit the method if it isn't clickable.
        var pointerClickTarget = currentFocus as IClickable;
        if (pointerClickTarget == null) 
        { 
            return; 
        }
        pointerClickTarget.Clicked(action, position);

        HandleSelectable(action, pointerClickTarget);
    }

    private void HandleSelectable(PointerClickAction action, IClickable clickTarget)
    {
        switch (action)
        {
            case PointerClickAction.ClickDown:
                if (heldObject == null)
                {
                    // If the pointerTarget is an IPlaceable; Then select it.
                    heldObject = clickTarget as IPlaceable;
                }
                break;
            case PointerClickAction.ClickUp:
                // If an IPlaceable is currently held.
                if (heldObject != null)
                {
                    // If the pointerClickTarget is an IPlacementArea; Then place it
                    var placementAreaFocused = clickTarget as IPlacementArea;
                    if (placementAreaFocused != null)
                    {
                        placementAreaFocused.PlaceObject(heldObject);
                    }
                    else /* No placement area found. */
                    {
                        heldObject.Place();
                    }
                    heldObject = null;
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
