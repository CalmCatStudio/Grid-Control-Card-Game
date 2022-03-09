using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardView), typeof(Collider2D))]
public class Card : MonoBehaviour
{
    private CardView view = null;
    [SerializeField]
    private CardScriptableObject cardInfo = null;
    private Vector3 defaultLocation = Vector3.zero;
    private Arrow[] arrows = null;
    public Arrow[] Arrows => arrows;
    private Collider2D cardCollider = null;

    private void Awake()
    {
        // TODO: This doesn't make sense right now, because cardInfo is already set, but this is for initial testing.
        // Card will probably be Setup by something else like the Deck once it exists.
        Setup(cardInfo);
    }

    public void Setup(CardScriptableObject cardInfo)
    {
        view = GetComponent<CardView>();
        cardCollider = GetComponent<Collider2D>();
        view.SetupView(cardInfo);

        // Set this object up based on the Scriptable Object given.
        this.cardInfo = cardInfo;
        gameObject.name = cardInfo.CardName;
        arrows = cardInfo.Arrows;
        
        // TODO: This is for testing, and will be removed
        // Set the starting locationg to its location when Play is pressed.
        defaultLocation = transform.position;
    }

    public void DrawCard(Vector3 destination)
    {
        // TODO: This is for the Deck that still needs to be created.
        defaultLocation = destination;
        view.MoveCard(destination);
    }

    /// <summary>
    /// Place the card at the given destination.
    /// If null the card will return to its original location.
    /// </summary>
    public void PlaceCard(Vector3? destination)
    {
        cardCollider.enabled = false;
        if (destination != null)
        {
            view.MoveCard((Vector3)destination);
        }
        else
        {
            view.MoveCard(defaultLocation);
            cardCollider.enabled = true;
        }
    }
}
