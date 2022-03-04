using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private List<Tile> neighbors = new List<Tile>();
    private CardPlaceable 
        currentCardHandler = null,
        previewCardHandler = null;
    private Tile neighborInPush = null;

    private void Awake()
    {
        GetTileNeighbors();
    }

    /// <summary>
    /// This is a recursive method that will keep pushing along a path until it finds an empty tile, or a blockade.
    /// </summary>
    /// <param name="newCardHandler">The card to be placed on the tile</param>
    /// <param name="currentDirectionPushed">The edge that was pushed on the tile</param>
    /// <returns>If every push was succesful then this will return true</returns>
    public bool ViewPreviewPlaceHoldable(CardPlaceable newCardHandler, Direction? currentDirectionPushed, ArrowPower strongestArrowPushing = ArrowPower.None)
    {
        // If there is no card on the tile; Then it will accept any new card.
        if (currentCardHandler == null) 
        {
            previewCardHandler = newCardHandler;
            return true; 
        }
        if (currentDirectionPushed == null)
        {
            return false;
        }

        // Get non nullable Directions so we don't have to worry about null anymore.
        Direction directionPushed = (Direction)currentDirectionPushed;
        Direction directionPushing = (Direction)DirectionHelper.GetOppositeDirection(directionPushed);
        Tile neighborToPush = neighbors[(int)directionPushing];

        // While pushing we use the strongest arrow we have come accross for every push after that.
        strongestArrowPushing = GetArrowForPush(newCardHandler, directionPushing, strongestArrowPushing);

        if (CheckIfNewArrowBeatsCurrentArrow(strongestArrowPushing, directionPushed))
        {
            print("New card beats Current card");
            
            if (neighborToPush != null)
            {
                Debug.Log($"Neighbor agrees{neighborToPush.gameObject.name}", neighborToPush.gameObject);
                if (neighborToPush.ViewPreviewPlaceHoldable(currentCardHandler, currentDirectionPushed, strongestArrowPushing))
                {
                    neighborInPush = neighborToPush;
                    SetPreviewCard(newCardHandler);
                    return true;
                }
                Debug.Log($"New card loses to Current card on {neighborToPush.gameObject.name}", neighborToPush.gameObject);
            }
        }

        return false;
    }


    /// <summary>
    /// Attempts to place a card on the tile. If there is a card already; Then it will try to push that card off the tile.
    /// </summary>
    /// <returns>Returns true if placement was succesful.</returns>
    public bool ConfirmPush()
    {
        if (previewCardHandler != null)
        {
            SetCardOnTile();
            
            neighborInPush?.ConfirmPush();
            neighborInPush = null;

            return true;
        }

        return false;
    }

    private ArrowPower GetArrowForPush(CardPlaceable newCard, Direction directionToPush, ArrowPower strongestArrowInPush)
    {
        // When a card is placed it needs to use the opposite edge in the battle.
        // IE: If the card was placed on the left of this tile; Then it will use its right arrow to push this left arrow.
        Arrow newArrow = newCard.Card.Arrows[(int)directionToPush];
        ArrowPower newArrowPower = newArrow.Power; 
        if (newArrowPower > strongestArrowInPush)
        {
            strongestArrowInPush = newArrowPower;
        }

        return strongestArrowInPush;
    }

    private bool CheckIfNewArrowBeatsCurrentArrow(ArrowPower strongestArrowPushing, Direction currentCardArrowPushed)
    {
        Arrow currentArrow = currentCardHandler.Card.Arrows[(int)currentCardArrowPushed];
        ArrowPower currentArrowPower = currentArrow.Power;
        //print($"New Card arrow: {strongestArrowPushing} Current Card Arrow: {currentArrowPower}");

        if (strongestArrowPushing > currentArrowPower)
        {
            return true;
        }
        return false;
    }

    public void UndoPreview()
    {
        previewCardHandler = null;
        neighborInPush?.UndoPreview();
        neighborInPush = null;
    }

    private void SetPreviewCard(CardPlaceable previewCard)
    {
        previewCardHandler = previewCard;
    }

    private void SetCardOnTile(CardPlaceable newCardHandler = null)
    {
        if (newCardHandler == null)
        {
            currentCardHandler = previewCardHandler;
            previewCardHandler = null;
        }
        else
        {
            currentCardHandler = newCardHandler;
        }
        currentCardHandler.Unselected(transform);
    }

    /// <summary>
    /// Loop through all directions, and get any neighbors from those directions.
    /// </summary>
    private void GetTileNeighbors()
    {  
        for (int i = 0; i < 4; i++)
        {
            var direction = DirectionHelper.GetVector3FromDirection((Direction)i);
            TryToAddNeighborInDirection(direction);
        }
    }

    /// <summary>
    /// Fires a raycast in the given direction, and adds the tile if it hits one.
    /// </summary>
    private void TryToAddNeighborInDirection(Vector3 direction)
    {
        var hit = Physics2D.Raycast(transform.position + direction, direction, 1);
        if (hit)
        {
            if (hit.collider.TryGetComponent(out Tile neighbor))
            {
                neighbors.Add(neighbor);
            }
            else
            {
                neighbors.Add(null);
            }
        }
        else
        {
            neighbors.Add(null);
        }
    }
}
