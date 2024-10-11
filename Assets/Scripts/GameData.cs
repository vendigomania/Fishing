using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [SerializeField] private bool ClearAll;

    public List<FishModel> FishModels = new List<FishModel>();

    [HideInInspector] public string ModeName;

    public int Catched => Trophies.Sum(tr => tr.Count);

    public int Escaped
    {
        get => PlayerPrefs.GetInt("Escaped", 0);
        set => PlayerPrefs.SetInt("Escaped", value);
    }
    public int MoneyEarned
    {
        get => PlayerPrefs.GetInt("Earned", 0);
        set => PlayerPrefs.SetInt("Earned", value);
    }

    public int Cash
    {
        get => PlayerPrefs.GetInt("Cash", 0);
        set
        {
            MoneyEarned += Mathf.Max(value - Cash, 0);

            PlayerPrefs.SetInt("Cash", value);
        }
    }

    #region fish types

    public int Common => GetByRarity(FishModel.FishRarity.Common);
    public int Uncommon => GetByRarity(FishModel.FishRarity.Uncommon);
    public int Rare => GetByRarity(FishModel.FishRarity.Rare);
    public int Epic => GetByRarity(FishModel.FishRarity.Epic);

    public float LargestFish
    {
        get => PlayerPrefs.GetFloat("Largest", 0f);
        set => PlayerPrefs.SetFloat("Largest", value);
    }

    public int GetByRarity(FishModel.FishRarity _rarity)
    {
        return Trophies.Where(tr => tr.Model.Rarity == _rarity).Sum(tr => tr.Count);
    }

    #endregion

    #region shop

    public int CatchDistance
    {
        get => PlayerPrefs.GetInt("CatchDistance", 1);
        set => PlayerPrefs.SetInt("CatchDistance", value);
    }

    public int DecreaseBreakSpeed
    {
        get => PlayerPrefs.GetInt("DecreaseBreakSpeed", 1);
        set => PlayerPrefs.SetInt("DecreaseBreakSpeed", value);
    }

    public int CastSpeed
    {
        get => PlayerPrefs.GetInt("CastSpeed", 1);
        set => PlayerPrefs.SetInt("CastSpeed", value);
    }

    public int CatchZoneEasy
    {
        get => PlayerPrefs.GetInt("CatchZoneEasy", 1);
        set => PlayerPrefs.SetInt("CatchZoneEasy", value);
    }

    #endregion

    public static GameData Instance;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        if (ClearAll) PlayerPrefs.DeleteAll();

        LoadTrophies();
    }

    public static int GetPrice(float _weight, FishModel _model) => Mathf.FloorToInt(_weight * 8f + (int)_model.Rarity * 14);

    #region trophies

    public class TrophyItem
    {
        public FishModel Model;
        public int Count;
        public float Weight;

        public TrophyItem(FishModel _model, int _count, float _weight)
        {
            Model = _model;
            Count = _count;
            Weight = _weight;
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}/{2}", Model.Id, Count, Weight);
        }
    }

    public List<TrophyItem> Trophies = new List<TrophyItem>();

    public void LoadTrophies()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("Trophies"))) return;

        var list = PlayerPrefs.GetString("Trophies").Split('|');
        foreach (var row in list)
        {
            string[] parts = row.Split('/');
            Trophies.Add(new TrophyItem(FishModels[int.Parse(parts[0]) - 1], int.Parse(parts[1]), float.Parse(parts[2])));
        }
    }

    public void AddToTrophies(FishModel _model, float _weight)
    {
        var trophy = Trophies.Find(tr => tr.Model.Id == _model.Id && tr.Weight == _weight);

        if (trophy != null) trophy.Count++;
        else Trophies.Add(new TrophyItem(_model, 1, _weight));

        PlayerPrefs.SetString("Trophies", string.Join('|', Trophies));
    }

    #endregion
}
