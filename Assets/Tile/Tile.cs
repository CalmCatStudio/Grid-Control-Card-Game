using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private TileType type = TileType.Normal;
    private List<Tile> neighbors = new List<Tile>();
    private CardPlaceable 
        currentCardHandler = null,
        previewCardHandler = null;
    private Tile neighborToPush = null;

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

        GetTileNeighbors();
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
    public bool TryToPreviewPush(CardPlaceable cardToPlace, Direction edgePushed, ArrowPower strongestArrowPushing = ArrowPower.None)
    {
        // If currentCard is null Accept any card.
        // Exit Recursion.
        if (currentCardHandler == null)
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
        if (cardToPlace != null)
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
        neighborToPush = neighbors[(int)directionPushing];
        if (neighborToPush == null)
        {
            return false;
        }

        // This method will keep going recursively until a neighbor accepts the card, or outright refuses it.
        if (!neighborToPush.TryToPreviewPush(currentCardHandler, edgePushed, strongestArrowPushing))
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
        if (neighborToPush != null)
        {
            neighborToPush.SetNeighborCardInPreview();
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
        if (previewCardHandler == null)
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

            // Exit if there is no neigbor in the direction.
            var neighbor = neighbors[i];
            if (neighbor == null)
            {                
                continue;
            }

            // Exit if push fails.
            var directionPushing = (Direction)i;
            var directionPushed = DirectionHelper.GetDirectionPushing(directionPushing);
            if (!neighbor.TryToPreviewPush(null, directionPushed, arrow.Power))
            {
                continue;
            }

            neighbor.ConfirmPush();
        }

        return true;
    }

    public void ConfirmPush()
    {
        // Tell neighbor about the push.
        if (neighborToPush != null)
        {
            neighborToPush.ConfirmPush();
            neighborToPush = null;
        }

        // Handle new card placement.
        currentCardHandler = previewCardHandler;
        previewCardHandler = null;
        
        // Place the new card if it exists.
        if (currentCardHandler != null)
        {
            currentCardHandler.Place(transform.position);
        }
    }

    public void OnExitPreview()
    {
        // Return the current card to its proper position if it exists.
        if (currentCardHandler != null)
        {
            currentCardHandler.Place(transform.position);
            previewCardHandler = null;
        }

        // Tell neighbor to exit the preview.
        if (neighborToPush != null)
        {
            neighborToPush.OnExitPreview();
            neighborToPush = null;
        }
    }

    /// <summary>
    /// Loop through all directions, and get any neighbors from those directions.
    /// </summary>
    private void GetTileNeighbors()
    {  
        for (int i = 0; i < 4; i++)
        {
            var direction = DirectionHelper.GetVector3FromDirection((Direction)i);
            AddNeighborInDirection(direction);
        }
    }

    /// <summary>
    /// Adds the neigbor, or null to the list of neigbors.
    /// </summary>
    private void AddNeighborInDirection(Vector3 direction)
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

    // TODO: Make an Arrow Utility Class!!!
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

    // TODO: This might belong in an Arrow Utilty class.
    /// <summary>
    /// Returns the stronger arrow between the given card, and the given strongest arrow.
    /// </summary>
    /// <returns>The strongest arrow provided</returns>
    private ArrowPower GetStrongestArrowInDirection(CardPlaceable newCard, Direction directionToPush, ArrowPower strongestArrowInPush)
    {
        if (newCard == null)
        {
            return strongestArrowInPush;
        }

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
}

public enum TileType
{
    Normal,
    Dead
}
