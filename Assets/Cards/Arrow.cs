using UnityEngine;

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
