using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private List<Tile> neighbors = new List<Tile>();
    private CardHandler cardHandler = null;
    private Card cardOnTile = null;

    private void Awake()
    {
        GetTileNeighbors();
    }

    public void PreviewPlaceHoldable(CardHandler cardHandler)
    {

    }

    /// <summary>
    /// Attempt to place the Card on the tile. It will fail if there is already a card.
    /// </summary>
    /// <returns>Returns false if the placement fails</returns>
    public bool PlaceHoldable(CardHandler newCardHandler)
    {
        if (cardHandler != null)
        {
            return false;
        }

        SetCardOnTile(newCardHandler);
        return true;        
    }

    /// <summary>
    /// Attempts to place a card on the tile. If there is a card already; Then it will try to push that card off the tile.
    /// </summary>
    /// <returns>Returns true if placement was succesful.</returns>
    public bool PlaceHoldable(CardHandler newCardHandler, Direction directionPlaced, ArrowType strongestArrowInPush = ArrowType.None)
    {
        // If no card is on the slot we don't need to do anything else.
        if (PlaceHoldable(newCardHandler))
        {
            return true;
        }

        int directionToInt = (int)GetOppositeDirection(directionPlaced);
        var newArrow = newCardHandler.Card.Arrows[directionToInt];
        if (newArrow > strongestArrowInPush)
        {
            strongestArrowInPush = newArrow;
        }

        if (CheckIfNewCardBeatsCard(strongestArrowInPush, directionPlaced))
        {
            print("New card beats Current card");
            var neighborThatCurrentCardMovesTo = CheckIfNeighborIsValid(directionPlaced, strongestArrowInPush);
            if (neighborThatCurrentCardMovesTo != null)
            {
                cardHandler.Unselected(neighborThatCurrentCardMovesTo.transform);
                SetCardOnTile(newCardHandler);
                return true;
            }
        }
        print("New card loses to Current card");
        return false;
    }

    private Tile CheckIfNeighborIsValid(Direction directionPlaced, ArrowType strongestArrowInPush)
    {
        int directionToPush = (int)GetOppositeDirection(directionPlaced);

        print("New card arrow stronger");
        print(directionToPush);
        var neighbor = neighbors[directionToPush];
        if (neighbor != null)
        {
            print(neighbor.name);
            if (neighbor.PlaceHoldable(cardHandler, directionPlaced, strongestArrowInPush))
            {
                return neighbor;
            }
        }
        return null;
    }

    private bool CheckIfNewCardBeatsCard(ArrowType strongestArrowInPush, Direction directionPlaced)
    {
        var directionToInt = (int)directionPlaced;

        print($"New Card arrow: {strongestArrowInPush} Current Card Arrow: {cardOnTile.Arrows[directionToInt]}");
        if (strongestArrowInPush > cardOnTile.Arrows[directionToInt])
        {
            return true;
        }
        return false;
    }

    private void SetCardOnTile(CardHandler cardInputReceiver)
    {
        cardHandler = cardInputReceiver;
        cardOnTile = cardInputReceiver.Card;
    }

    /// <summary>
    /// Loop through all directions, and get any neighbors from those directions.
    /// </summary>
    private void GetTileNeighbors()
    {  
        for (int i = 0; i < 4; i++)
        {
            var direction = GetDirectionFromInt(i);
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
    private Direction GetOppositeDirection(Direction direction)
    {
        int oppositeDirection = 0;
        int directionToInt = (int)direction;

        if (directionToInt == 0 || directionToInt == 1)
        {
            oppositeDirection = directionToInt + 2;
        }
        else if (directionToInt == 2 || directionToInt == 3)
        {
            oppositeDirection = directionToInt - 2;
        }
        return (Direction)oppositeDirection;
    }

    /// <summary>
    /// Gets the Vector3 direction based on the Direction Enum.
    /// </summary>
    private Vector3 GetDirectionFromInt(int i)
    {
        var direction = Vector3.zero;
        // Get the direction to shoot.
        switch (i)
        {
            case (int)Direction.Up:
                direction = Vector3.up;
                break;
            case (int)Direction.Right:
                direction = Vector3.right;
                break;
            case (int)Direction.Down:
                direction = Vector3.down;
                break;
            case (int)Direction.Left:
                direction = Vector3.left;
                break;
            default:
                Debug.LogError("An unknown direction has been queried", gameObject);
                break;
        }
        return direction;
    }
}
