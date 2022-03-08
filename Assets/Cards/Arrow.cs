using UnityEngine;

[System.Serializable]
public class Arrow
{
    [SerializeField]
    private ArrowStrength power = ArrowStrength.None;
    public ArrowStrength Power => power;

    [SerializeField]
    private ArrowEffect effect = ArrowEffect.None;
    public ArrowEffect Effect => effect;
}

public enum ArrowStrength
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
