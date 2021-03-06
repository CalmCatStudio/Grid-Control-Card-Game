using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private TileType type = TileType.Normal;

    /// <summary>
    /// Contains tiles from the surrounding Directions. It will contain null if nothing is in the direction.
    /// </summary>
    private List<Tile> surroundingTiles = new List<Tile>();
    private CardPlaceable 
        currentCardHandler = null,
        previewCardHandler = null;
    private Tile tileToPush = null;

    // TODO: This is view work.
    [SerializeField]
    private SpriteRenderer backgroundRenderer = null;

    private void Awake()
    {
        // TODO: Make a TileView for this work.
        if (type == TileType.Dead)
        {
            backgroundRenderer.color = Color.grey;
        }

        GetSurroundingTiles();
    }

    /// <summary>
    /// This is called by an IPlacement area. 
    /// It recursively pushes along a path until it finds an empty tile, or a blockade.
    /// </summary>
    /// <param name="cardToPlace">The card to be placed on the tile</param>
    /// <param name="edgePushed">The edge that was pushed on the tile</param>
    /// <returns>If every push was succesful then this will return true</returns>
    public bool OnPreviewCardPlacement(CardPlaceable cardToPlace, Direction edgePushed)
    {
        // If the Tile is dead; Then we can not directly place a card on it.
        if (type == TileType.Dead)
        {
            return false;
        }

        if (!TryToPreviewPush(cardToPlace, edgePushed))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Attempts to Push the newCard onto the Tile. 
    /// It is recursive, and will check each neighbor until either an empty space, or an obstacle it found.
    /// </summary>
    /// <returns>True if an empty space is found</returns>
    public bool TryToPreviewPush(CardPlaceable cardToPlace, Direction edgePushed, ArrowStrength strongestArrowPushing = ArrowStrength.None)
    {
        // If currentCard is null Accept any card.
        // Exit Recursion.
        if (!currentCardHandler)
        {
            previewCardHandler = cardToPlace;
            return true;
        }

        // When pushed from one direction; An object is pushing towards the opposite side.
        // IE: If pushed from the top; Then the cardToPlace wants to use its bottom arrow to try to push with.
        Direction directionPushing = (Direction)DirectionHelper.GetDirectionPushing(edgePushed);

        // Exit if the push cannot beat this arrow.
        strongestArrowPushing = GetStrongestArrowInDirection(cardToPlace, directionPushing, strongestArrowPushing);
        if (!CheckIfNewArrowBeatsCurrentArrow(strongestArrowPushing, edgePushed))
        {
            return false;
        }

        // Check for relevant card effects.
        if (cardToPlace)
        {
            // Bomb arrows destroy cards they beat
            // End recursion here because there is nothing to push.
            if (cardToPlace.Card.Arrows[(int)directionPushing].Effect == ArrowEffect.Bomb)
            {
                // This is where the card on the tile gets destroyed by the bomb.
                currentCardHandler.Place();
                SetupPreview(cardToPlace);
                return true;
            }
        }

        // Exit if neighbor is null.
        tileToPush = surroundingTiles[(int)directionPushing];
        if (!tileToPush)
        {
            return false;
        }

        // This method will keep going recursively until a neighbor accepts the card, or outright refuses it.
        if (!tileToPush.TryToPreviewPush(currentCardHandler, edgePushed, strongestArrowPushing))
        {
            return false;
        }

        // The push was succesful.
        SetupPreview(cardToPlace);
        return true;
    }

    /// <summary>
    /// This Sets up the newCard as the preview card, and shows every placement after the first one. The first one does not show the preview, because the player is holding it.
    /// </summary>
    /// <param name="cardToPlace"></param>
    /// <param name="neighborPushed"></param>
    private void SetupPreview(CardPlaceable cardToPlace)
    {
        previewCardHandler = cardToPlace;

        // This makes it so we skip the initial card getting pulled from the players hand when previewing a move.
        if (tileToPush)
        {
            tileToPush.SetNeighborCardInPreview();
        }
    }

    /// <summary>
    /// This is called by a neighbor during a preview of a push.
    /// </summary>
    public void SetNeighborCardInPreview()
    {
        previewCardHandler.Place(transform.position);
    }

    /// <summary>
    /// Called by an IPlacementArea.
    /// Confirms the push that was already getting previewed.
    /// </summary>
    /// <returns>Returns true if placement was succesful.</returns>
    public bool OnConfirmPlacement()
    {
        if (!previewCardHandler)
        {
            return false;
        }

        ConfirmPush();

        // When a card is first place on the board Check all of its arrows for relevant effects.
        for (int i = 0; i < 4; i++)
        {
            // Exit if we don't have a push arrow.
            var arrow = currentCardHandler.Card.Arrows[i];
            if (arrow.Effect != ArrowEffect.Push)
            {
                continue;
            }

            // Exit if there is no neighbor in the direction.
            var tileInPushEffect = surroundingTiles[i];
            if (!tileInPushEffect)
            {                
                continue;
            }

            // Exit if push fails.
            var directionPushing = (Direction)i;
            var directionPushed = DirectionHelper.GetDirectionPushing(directionPushing);
            if (!tileInPushEffect.TryToPreviewPush(null, directionPushed, arrow.Power))
            {
                continue;
            }

            tileInPushEffect.ConfirmPush();
        }

        return true;
    }

    public void ConfirmPush()
    {
        // Tell neighbor about the push.
        if (tileToPush)
        {
            tileToPush.ConfirmPush();
            tileToPush = null;
        }

        // Handle new card placement.
        currentCardHandler = previewCardHandler;
        previewCardHandler = null;
        
        // Place the new card if it exists.
        if (currentCardHandler)
        {
            currentCardHandler.Place(transform.position);
        }
    }

    public void OnExitPreview()
    {
        // Return the current card to its proper position if it exists.
        if (currentCardHandler)
        {
            currentCardHandler.Place(transform.position);
            previewCardHandler = null;
        }

        // Tell neighbor to exit the preview.
        if (tileToPush)
        {
            tileToPush.OnExitPreview();
            tileToPush = null;
        }
    }

    /// <summary>
    /// Loop through all directions, and get any neighbors from those directions.
    /// </summary>
    private void GetSurroundingTiles()
    {  
        for (int i = 0; i < 4; i++)
        {
            var direction = DirectionHelper.GetVector3FromDirection((Direction)i);
            AddTileInDirection(direction);
        }
    }

    /// <summary>
    /// Adds the neighbor, or null to the list of neigbors.
    /// </summary>
    private void AddTileInDirection(Vector3 direction)
    {
        var hit = Physics2D.Raycast(transform.position + direction, direction, 1);
        if (hit)
        {
            if (hit.collider.TryGetComponent(out Tile neighbor))
            {
                surroundingTiles.Add(neighbor);
            }
            else
            {
                surroundingTiles.Add(null);
            }
        }
        else
        {
            surroundingTiles.Add(null);
        }
    }

    // TODO: Make an Arrow Utility Class!!!
    private bool CheckIfNewArrowBeatsCurrentArrow(ArrowStrength strongestArrowPushing, Direction currentCardArrowPushed)
    {
        Arrow currentArrow = currentCardHandler.Card.Arrows[(int)currentCardArrowPushed];
        ArrowStrength currentArrowPower = currentArrow.Power;
        //print($"New Card arrow: {strongestArrowPushing} Current Card Arrow: {currentArrowPower}");

        if (strongestArrowPushing > currentArrowPower)
        {
            return true;
        }
        return false;
    }

    // TODO: This might belong in an Arrow Utilty class.
    /// <summary>
    /// Returns the stronger arrow between the given card, and the given strongest arrow.
    /// </summary>
    /// <returns>The strongest arrow provided</returns>
    private ArrowStrength GetStrongestArrowInDirection(CardPlaceable newCard, Direction directionToPush, ArrowStrength strongestArrowInPush)
    {
        if (!newCard)
        {
            return strongestArrowInPush;
        }

        // When a card is placed it needs to use the opposite edge in the battle.
        // IE: If the card was placed on the left of this tile; Then it will use its right arrow to push this left arrow.
        Arrow newArrow = newCard.Card.Arrows[(int)directionToPush];
        ArrowStrength newArrowPower = newArrow.Power;
        if (newArrowPower > strongestArrowInPush)
        {
            strongestArrowInPush = newArrowPower;
        }

        return strongestArrowInPush;
    }
}

public enum TileType
{
    Normal,
    Dead
}
