using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Card))]
public class CardPlaceable : MonoBehaviour, IPlaceable
{
    private Card card = null;
    public Card Card => card;
    private bool isSelected = false;

    private Transform pointerTransform = null;
    private Vector3
        pointerLocationOffset = Vector3.zero;

    private void Awake()
    {
        card = GetComponent<Card>();    
    }

    public void Clicked(PointerClickAction pointerAction, Vector3 pointerPosition)
    {
        // The offset is the localPosition on the clickable that was clicked
        if (pointerAction == PointerClickAction.ClickDown)
        {
            pointerLocationOffset = transform.position - pointerPosition;
            isSelected = true;
        }
    }

    public void OnEnterFocus(Transform pointerTransform, IPlaceable pointerHeldObject)
    {
        if (isSelected || this.pointerTransform == null)
        {
            this.pointerTransform = pointerTransform;
        }
    }

    public void OnExitFocus()
    {
        if (!isSelected)
        {
            pointerTransform = null;
        }
    }

    public void Move(Vector3 destination)
    {
        var currentPosition = destination + pointerLocationOffset;
        transform.position = currentPosition;
    }

    public void Place(Vector3? destination = null)
    {
        isSelected = false;
        Vector3? position = null;
        if (destination != null)
        {
            position = destination;
        }

        card.PlaceCard(position);
    }
}
