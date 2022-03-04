using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tile))]
public class TilePlacementArea : MonoBehaviour, IPlacementArea
{
    private Tile tile = null;
    [SerializeField]
    private Collider2D backGroundCollider = null;
    [SerializeField, Tooltip("Place the colliders Clockwise starting at Up")]
    private Collider2D[] directionalColliders = new Collider2D[4];

    private Collider2D
        colliderFocused = null,
        previousColliderFocused = null;
    private Direction? directionFocused = null;

    private void Awake()
    {
        tile = GetComponent<Tile>();
    }

    public void Clicked(PointerClickAction pointerAction, Vector3 pointerPosition)
    {
        // TODO: Clicked might be obsolete, but I have to see.
        // The offset is the localPosition on the clickable that was clicked
        //var offset = transform.position - pointerPosition;
        //print($"Pointer Action: {pointerAction}");
        //print($" Clickable Target Name: {gameObject.name} Collider Clicked: {colliderFocused.name}");
    }

    public void EnterFocus(Transform pointerTransform, IPlaceable placeable = null)
    {
        ResetFocus();
        EvaluatePointerFocus(pointerTransform.position);
        HandleFocus(placeable);
    }

    public void ExitFocus()
    {
        tile.UndoPreview();
        ResetFocus();
    }
    private void ResetFocus()
    {
        previousColliderFocused = colliderFocused;
        directionFocused = null;
        colliderFocused = null;
    }

    private void HandleFocus(IPlaceable placeable)
    {
        if (colliderFocused == null)
        {
            return;
        }

        // Prevent the same action from looping if we are on the same collider
        if (colliderFocused != previousColliderFocused)
        {
            tile.UndoPreview();
            if (placeable != null)
            {
                var cardPlaceable = placeable as CardPlaceable;
                if (cardPlaceable == null)
                {
                    return;
                }
                
                tile.ViewPreviewPlaceHoldable(cardPlaceable, directionFocused);
            }
        }
    }

    private void EvaluatePointerFocus(Vector3 pointerPosition)
    {
        int count = directionalColliders.Length;
        for (int i = 0; i < count; i++)
        {
            var collider = directionalColliders[i];
            if (collider.OverlapPoint(pointerPosition))
            {
                directionFocused = (Direction)i;
                colliderFocused = collider;
            }
        }
    }

    public void PlaceSelectable(IPlaceable placeable)
    {
        var cardPlaceable = placeable as CardPlaceable;
        if (cardPlaceable == null)
        {
            return;
        }

        bool placed = tile.ConfirmPush();

        if (!placed)
        {
            //print("Placement failed");
            placeable.Unselected();
        }
    }
}
