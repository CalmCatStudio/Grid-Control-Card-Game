using System;
using UnityEngine;

/// <summary>
/// Contains methods that help when working with the Direction enum.
/// </summary>
public static class DirectionHelper
{
    /// <summary>
    /// Pass a direction in, and get the opposite direction out.
    /// </summary>
    /// <param name="direction">The direction you want the opposite of.</param>
    /// <returns>The opposite of the direction passed</returns>
    public static Direction GetOppositeDirection(Direction direction)
    {
        int oppositeDirection = 0;
        int directionAsInt = (int)direction;

        // Direction is Clockwise: Up, Right, Down, Left
        // So by doing this we can easily get the opposite direction.
        if (directionAsInt == 0 || directionAsInt == 1)
        {
            oppositeDirection = directionAsInt + 2;
        }
        else if (directionAsInt == 2 || directionAsInt == 3)
        {
            oppositeDirection = directionAsInt - 2;
        }
        return (Direction)oppositeDirection;
    }

    public static Vector3 GetVector3FromDirection(Direction direction)
    {
        var directionAsVector = Vector3.zero;
        // Get the direction to shoot.
        switch (direction)
        {
            case Direction.Up:
                directionAsVector = Vector3.up;
                break;
            case Direction.Right:
                directionAsVector = Vector3.right;
                break;
            case Direction.Down:
                directionAsVector = Vector3.down;
                break;
            case Direction.Left:
                directionAsVector = Vector3.left;
                break;
            default:
                DirectionNotDefined();
                break;
        }
        return directionAsVector;
    }

    /// <summary>
    /// This should not be used if can be avoided. It makes use of Enum.Isdefined which is slow.
    /// </summary>
    public static Vector3 GetVector3FromDirection(int directionAsInt)
    {
        var directionAsVector = Vector3.zero;
        if (Enum.IsDefined(typeof(Direction), directionAsInt))
        {
            Direction direction = (Direction)directionAsInt;
            directionAsVector = GetVector3FromDirection(direction);
        }

        if (directionAsVector == Vector3.zero)
        {
            DirectionNotDefined();
        }
        return directionAsVector;
    }

    private static void DirectionNotDefined()
    {
        Debug.LogError("Direction not defined. Vector3.zero was return.");
    }
}
