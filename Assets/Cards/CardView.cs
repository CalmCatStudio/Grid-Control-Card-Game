using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField, Tooltip("Arrows go Clockwise: Up, Right, Down, Left")]
    private SpriteRenderer[] arrowRenderers = new SpriteRenderer[4];
    [SerializeField, Tooltip("Refer to The ArrowType Enum for the order: None, Single, Double")]
    private Sprite[] arrowSprites;

    public void Setup(CardScriptableObject cardInfo)
    {
        int count = arrowRenderers.Length;
        for (int i = 0; i < count; i++)
        {
            int direction = (int)cardInfo.Arrows[i];
            arrowRenderers[i].sprite = arrowSprites[direction];
        }
    }
}
