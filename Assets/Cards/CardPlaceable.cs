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
        else if (pointerTransform != null)
        {
            transform.position = pointerTransform.position + pointerLocationOffset;
        }
    }

    public void Selected()
    {
        // Selected could be replaced by Clicked. ISelectable could just add Unselected to the interface.
        isSelected = true;
    }

    public void Unselected(Transform placementArea = null)
    {
        isSelected = false;
        Vector3? position = null;
        if (placementArea != null)
        {
            position = placementArea.position;
        }

        card.PlaceCard(position);
    }
}
