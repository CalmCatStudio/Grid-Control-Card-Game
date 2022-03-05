using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tile))]
public class TilePlacementArea : MonoBehaviour, IPlacementArea
{
    private Tile tile = null;
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
        // TODO: Show Tile information

        // The offset is the localPosition on the clickable that was clicked
        //var offset = transform.position - pointerPosition;
    }

    public void OnEnterFocus(Transform pointerTransform, IPlaceable placeable = null)
    {
        ResetFocus();
        EvaluatePointerPosition(pointerTransform.position);
        HandleFocus(placeable);
    }

    public void OnExitFocus()
    {
        tile.OnExitPreview();
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
            tile.OnExitPreview();
            if (directionFocused == null)
            {
                return;
            }

            if (placeable == null)
            {
                return;
            }

            var cardPlaceable = placeable as CardPlaceable;
            if (cardPlaceable == null)
            {
                return;
            }

            tile.OnPreviewPlacement(cardPlaceable, (Direction)directionFocused);

        }
    }

    /// <summary>
    /// Loop through each collider on the Tile and see which one the pointer is inside.
    /// </summary>
    private void EvaluatePointerPosition(Vector3 pointerPosition)
    {
        int count = directionalColliders.Length;
        for (int i = 0; i < count; i++)
        {
            var collider = directionalColliders[i];

            // Escape if we have found a collider.
            if (collider.OverlapPoint(pointerPosition))
            {
                directionFocused = (Direction)i;
                colliderFocused = collider;
                break;
            }
        }
    }

    /// <summary>
    /// Try to place an IPlaceable. If it fails the IPlaceable will have its Unselected method called.
    /// </summary>
    public void Place(IPlaceable placeable)
    {
        var cardPlaceable = placeable as CardPlaceable;
        if (cardPlaceable == null)
        {
            return;
        }

        bool placed = tile.OnConfirmPlacement();

        if (!placed)
        {
            //print("Placement failed");
            placeable.Unselected();
        }
    }
}
