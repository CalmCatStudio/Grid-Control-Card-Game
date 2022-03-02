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
    private ArrowType[] arrows = null;
    public ArrowType[] Arrows => arrows;
    private Collider2D cardCollider = null;

    private void Awake()
    {
        view = GetComponent<CardView>();
        cardCollider = GetComponent<Collider2D>();
        // This doesn't make sense right now, because cardInfo is already set,
        // but Card will probably be Setup by something else like the Deck.
        Setup(cardInfo);
    }

    public void Setup(CardScriptableObject cardInfo)
    {
        this.cardInfo = cardInfo;
        gameObject.name = cardInfo.CardName;
        arrows = cardInfo.Arrows;

        startingLocation = transform.position;

        view.Setup(cardInfo);
    }

    public void PlaceCard(Vector3 position)
    {
        transform.position = position;
        cardCollider.enabled = false;
    }

    public void ReturnCardToStartingLocation()
    {
        transform.position = startingLocation;
    }
}
