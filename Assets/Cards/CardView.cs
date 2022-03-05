using UnityEngine;
using DG.Tweening;

public class CardView : MonoBehaviour
{
    [SerializeField, Tooltip("Arrows go Clockwise: Up, Right, Down, Left")]
    private SpriteRenderer[] arrowRenderers = new SpriteRenderer[4];
    [SerializeField, Tooltip("Refer to The ArrowType Enum for the order: None, Single, Double")]
    private Sprite[] arrowSprites;

    [SerializeField]
    private float cardSnapToPositionSpeed = .5f;

    private Tween cardPlacedTween = null;

    public void MoveCard(Vector3 destination)
    {
        // This might need to be handled more carefully if more movement is added.
        cardPlacedTween = transform.DOMove(destination, .5f);
    }

    public void SetupView(CardScriptableObject cardInfo)
    {
        SetupArrows(cardInfo);
    }

    private void SetupArrows(CardScriptableObject cardInfo)
    {
        int count = arrowRenderers.Length;
        for (int i = 0; i < count; i++)
        {
            Arrow arrow = cardInfo.Arrows[i];
            arrowRenderers[i].sprite = arrowSprites[(int)arrow.Power];
        }
    }
}
