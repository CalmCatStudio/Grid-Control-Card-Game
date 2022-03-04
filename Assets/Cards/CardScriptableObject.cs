using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Card", menuName ="Card", order = 1)]
public class CardScriptableObject : ScriptableObject
{
    [SerializeField]
    private string cardName = "Default";
    public string CardName => cardName;

    [SerializeField]
    private Sprite fieldImage = null;
    public Sprite FieldImage => fieldImage;

    [SerializeField, Tooltip("The arrows go Clockwise: Up, Right, Down, Left")]
    private Arrow[] arrows = new Arrow[4];
    public Arrow[] Arrows => arrows;
}

[System.Serializable]
public class Arrow
{
    [SerializeField]
    private ArrowPower power = ArrowPower.None;
    public ArrowPower Power => power;

    [SerializeField]
    private ArrowEffect effect = ArrowEffect.None;
    public ArrowEffect Effect => effect;
}

public enum ArrowPower
{
    None,
    Single,
    Double
}

public enum ArrowEffect
{
    None,
    Bomb,
    Push
}