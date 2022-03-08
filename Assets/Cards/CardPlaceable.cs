using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Card))]
public class CardPlaceable : MonoBehaviour, IPlaceable
{
    private Card card = null;
    public Card Card => card;

    private Vector3
        pointerLocationOnCard = Vector3.zero;

    private void Awake()
    {
        card = GetComponent<Card>();    
    }

    public void Clicked(PointerClickAction pointerAction, Vector3 pointerPosition)
    {
        if (pointerAction == PointerClickAction.ClickDown)
        {
            pointerLocationOnCard = transform.position - pointerPosition;
        }
    }

    public void OnEnterFocus(Transform pointerTransform, IPlaceable pointerHeldObject)
    {
        // TODO: Show Card information if pointer isn't holding something.
    }

    public void OnExitFocus()
    {
        // TODO: Hide Card information.
    }

    public void Move(Vector3 destination)
    {
        // This prevents the card from snapping to the pointer position.
        var currentPosition = destination + pointerLocationOnCard;
        transform.position = currentPosition;
    }

    public void Place(Vector3? destination = null)
    {
        card.PlaceCard(destination);
    }
}
