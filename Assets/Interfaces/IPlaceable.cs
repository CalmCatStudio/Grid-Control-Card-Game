using UnityEngine;

public interface IPlaceable : IClickable
{
    public void Move(Vector3 destination);
    /// <summary>
    /// If destination is null then the placeable will return to its default location
    /// </summary>
    public void Place(Vector3? destination = null);
}
