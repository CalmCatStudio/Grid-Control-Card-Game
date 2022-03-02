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
    private ArrowType[] arrows = new ArrowType[4];
    public ArrowType[] Arrows => arrows;
}

public enum ArrowType
{
    None,
    Single,
    Double
}