using UnityEngine;

public enum EssenceColor
{
    Red,
    Yellow,
    Blue
}

[System.Serializable]
public class Essence 
{
    public EssenceColor color;
    public int amount;
}

