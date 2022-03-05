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
    private Tile neighborPushed = null;

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
    /// This is initially called by an IPlacement area. 
    /// Then recursively pushes along a path until it finds an empty tile, or a blockade.
    /// </summary>
    /// <param name="newCard">The card to be placed on the tile</param>
    /// <param name="directionPushed">The edge that was pushed on the tile</param>
    /// <returns>If every push was succesful then this will return true</returns>
    public bool OnPreviewPlacement(CardPlaceable newCard, Direction directionPushed, ArrowPower strongestArrowPushing = ArrowPower.None)
    {
        // TODO: This seems like a poor way to determine first push, but it will do for now
        // It works because an empty arrow can not push, and the First Push does have an arrow yet. Recursive Pushes all have an Arrow power greater than None.
        // The first push.
        if (strongestArrowPushing == ArrowPower.None)
        {
            // If the Tile is dead; Then we can not directly place a card on it. (Pushing into is allowed.)
            if (type == TileType.Dead)
            {
                return false;
            }
        }

        // If there is no card on the tile; Then it will accept any new card.
        if (currentCardHandler == null) 
        {
            previewCardHandler = newCard;
            return true; 
        }

        // When pushed from one direction; An object is pushing towards the opposite side.
        Direction directionPushing = (Direction)DirectionHelper.GetOppositeDirection(directionPushed);
        neighborPushed = neighbors[(int)directionPushing];
        // If there is no neigbor in the direction then we refuse the card.
        if (neighborPushed == null)
        {
            return false;
        }

        // While pushing we use the strongest arrow we have come accross for every push after that.
        // If the strongest arrow loses to this card; Then we refuse the new card.
        strongestArrowPushing = GetStrongestArrowInDirection(newCard, directionPushing, strongestArrowPushing);
        if (!CheckIfNewArrowBeatsCurrentArrow(strongestArrowPushing, directionPushed))
        {
            return false;
        }

        // This method will keep going recursively until a neighbor accepts the card, or outright refuses it.
        if (!neighborPushed.OnPreviewPlacement(currentCardHandler, directionPushed, strongestArrowPushing))
        {
            return false;
        }

        SetPreviewCard(newCard);
        return true;
    }

    /// <summary>
    /// The preview card will only get set if the push is legal.
    /// </summary>
    /// <param name="previewCard"></param>
    /// <param name="neighborPushed"></param>
    private void SetPreviewCard(CardPlaceable previewCard)
    {
        previewCardHandler = previewCard;

        if (neighborPushed != null)
        {
            neighborPushed.PreviewPushing();
        }
    }

    public void PreviewPushing()
    {
        previewCardHandler.Unselected(transform);
    }

    /// <summary>
    /// Confirms the push that was already getting previewed.
    /// </summary>
    /// <returns>Returns true if placement was succesful.</returns>
    public bool OnConfirmPlacement()
    {
        if (previewCardHandler == null)
        {
            return false;
        }

        ConfirmPlacement();
        neighborPushed?.OnConfirmPlacement();
        neighborPushed = null;

        return true;
    }

    private void ConfirmPlacement()
    {
        currentCardHandler = previewCardHandler;
        previewCardHandler = null;
        currentCardHandler.Unselected(transform);
    }

    public void OnExitPreview()
    {
        currentCardHandler?.Unselected(transform);
        previewCardHandler = null;
        neighborPushed?.OnExitPreview();
        neighborPushed = null;
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
