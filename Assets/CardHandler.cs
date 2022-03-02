using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Card))]
public class CardHandler : MonoBehaviour, IPlaceable
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

    private void Update()
    {
        if (isSelected)
        {
            var currentPosition = pointerTransform.position + pointerLocationOffset;
            transform.position = currentPosition;
        }
    }

    public void Clicked(PointerClickAction pointerAction, Vector3 pointerPosition)
    {
        // The offset is the localPosition on the clickable that was clicked
        pointerLocationOffset = transform.position - pointerPosition;
    }

    public void EnterFocus(Transform pointerTransform, IPlaceable pointerHeldObject)
    {
        if (isSelected || this.pointerTransform == null)
        {
            this.pointerTransform = pointerTransform;
        }
    }

    public void ExitFocus()
    {
        if (!isSelected)
        {
            pointerTransform = null;
        }
        else if (pointerTransform != null)
        {
            transform.position = pointerTransform.position + pointerLocationOffset;
        }
    }

    public void Selected()
    {
        isSelected = true;
    }

    public void Unselected(Transform placementArea = null)
    {
        isSelected = false;
        if (placementArea == null)
        {
            card.ReturnCardToStartingLocation();
        }
        else
        {
            card.PlaceCard(placementArea.position);
        }
    }
}
