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
        SetupArrows(cardInfo);
    }

    private void SetupArrows(CardScriptableObject cardInfo)
    {
        int count = arrowRenderers.Length;
        for (int i = 0; i < count; i++)
        {
            SetArrowPowerSprite(cardInfo, i);
        }
    }

    private void SetArrowPowerSprite(CardScriptableObject cardInfo, int i)
    {
        Arrow arrow = cardInfo.Arrows[i];
        ArrowPower power = arrow.Power;
        int powerToInt = (int)power;
        arrowRenderers[i].sprite = arrowSprites[powerToInt];
    }
}
