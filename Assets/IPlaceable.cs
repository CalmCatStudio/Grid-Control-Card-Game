using UnityEngine;

public interface IPlaceable : IClickable
{
    public void Selected();
    /// <summary>
    /// If placement area is null then the selectable will return to its default location
    /// </summary>
    /// <param name="placementArea">A Transform is used rather than a vector3 because Transform can be null</param>
    public void Unselected(Transform placementArea = null);
}
