using UnityEngine;

[System.Serializable]
public class FishModel
{
    public enum FishRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
    }

    public int Id = 0;
    public Sprite Icon;
    public float MinWeight;
    public float MaxWeight;
    public float ChancePercent;
    public FishRarity Rarity;
}
