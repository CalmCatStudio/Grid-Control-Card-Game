using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tile))]
public class TileHandler : MonoBehaviour, IPlacementArea
{
    private Tile tile = null;
    [SerializeField]
    private BoxCollider2D backGroundCollider = null;
    [SerializeField, Tooltip("Place the colliders Clockwise starting at Up")]
    private BoxCollider2D[] directionalColliders = new BoxCollider2D[4];

    private BoxCollider2D
        colliderFocused = null,
        previousColliderFocused = null;
    private bool edgeFocused = false;
    private Direction directionFocused = Direction.Up;

    private void Awake()
    {
        tile = GetComponent<Tile>();
    }

    public void Clicked(PointerClickAction pointerAction, Vector3 pointerPosition)
    {
        // The offset is the localPosition on the clickable that was clicked
        //var offset = transform.position - pointerPosition;
        //print($"Pointer Action: {pointerAction}");
        //print($" Clickable Target Name: {gameObject.name} Collider Clicked: {colliderFocused.name}");
    }

    public void EnterFocus(Transform pointerTransform, IPlaceable pointerHeldObject = null)
    {
        ResetFocus();
        EvaluatePointerFocus(pointerTransform.position);
        HandleFocus();
    }

    public void ExitFocus()
    {
        ResetFocus();
    }
    private void ResetFocus()
    {
        previousColliderFocused = colliderFocused;
        colliderFocused = null;
        edgeFocused = false;
    }

    private void HandleFocus()
    {
        if (colliderFocused == null)
        {
            return;
        }

        // Prevent the same action from looping if we are on the same collider
        if (colliderFocused != previousColliderFocused)
        {
            //print($"Current Focused Collider: {colliderFocused.name}");
        }
    }

    private void EvaluatePointerFocus(Vector3 pointerPosition)
    {
        // We check the background first; So the edges will overwrite it if they are in focus.
        if (backGroundCollider.OverlapPoint(pointerPosition))
        {
            colliderFocused = backGroundCollider;
        }

        int count = directionalColliders.Length;
        for (int i = 0; i < count; i++)
        {
            var collider = directionalColliders[i];
            if (collider.OverlapPoint(pointerPosition))
            {
                directionFocused = (Direction)i;
                edgeFocused = true;
                colliderFocused = collider;
            }
        }
    }

    public void PlaceSelectable(IPlaceable holdableToPlace)
    {
        var cardReceiver = holdableToPlace as CardHandler;
        if (cardReceiver == null)
        {
            return;
        }
        bool holdablePlaced = false;
        if (edgeFocused)
        {
            //print("Edge Focused during placement");
            if (tile.PlaceHoldable(cardReceiver, directionFocused))
            {
                //print("Edge placement succesful");
                holdablePlaced = true;
                holdableToPlace.Unselected(transform);
            }
        }
        else
        {
            // If the tile accepts the holdable
            if (tile.PlaceHoldable(cardReceiver))
            {
                //print("Non Edge Placement succesfull");
                holdablePlaced = true;
                holdableToPlace.Unselected(transform);
            }
        }

        if (!holdablePlaced)
        {
            //print("Placement failed");
            holdableToPlace.Unselected();
        }
    }
}
