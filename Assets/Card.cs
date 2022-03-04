using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardView), typeof(Collider2D))]
public class Card : MonoBehaviour
{
    private CardView view = null;
    [SerializeField]
    private CardScriptableObject cardInfo = null;
    private Vector3 startingLocation = Vector3.zero;
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
        view.Setup(cardInfo);

        this.cardInfo = cardInfo;
        gameObject.name = cardInfo.CardName;
        arrows = cardInfo.Arrows;
        startingLocation = transform.position;
    }

    public void PlaceCard(Vector3 position)
    {
        // TODO: Tell the view to tween our position to the new position rather than do Position work here.
        transform.position = position;
        cardCollider.enabled = false;
    }

    public void ReturnCardToStartingLocation()
    {
        transform.position = startingLocation;
    }
}
