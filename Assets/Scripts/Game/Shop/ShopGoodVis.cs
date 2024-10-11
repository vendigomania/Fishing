using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Shop
{
    public class ShopGoodVis : MonoBehaviour
    {
        public enum GoodType
        {
            CatchDistance,
            DecreaseBreakSpeed,
            CastSpeed,
            CatchZoneEasy,
        }

        [SerializeField] private TMP_Text lvl;
        [SerializeField] private TMP_Text cost;
        [SerializeField] private Button buyBtn;
        [SerializeField] private GoodType type;

        public UnityAction OnBuy;

        public int Cost
        {
            get
            {
                switch(type)
                {
                    case GoodType.CatchDistance:
                        return 100 + GameData.Instance.CatchDistance * 20;
                    case GoodType.DecreaseBreakSpeed:
                        return 100 + GameData.Instance.DecreaseBreakSpeed * 20;
                    case GoodType.CastSpeed:
                        return 100 + GameData.Instance.CastSpeed * 20;
                    default:
                        return 100 + GameData.Instance.CatchZoneEasy * 20;
                }
            }
        }

        void Start()
        {
            buyBtn.onClick.AddListener(Buy);
        }

        public void UpdateInfo()
        {
            buyBtn.interactable = GameData.Instance.Cash >= Cost;
            cost.text = Cost.ToString();

            switch(type)
            {
                case GoodType.CatchDistance:
                    lvl.text = GameData.Instance.CatchDistance.ToString();
                    break;
                case GoodType.DecreaseBreakSpeed:
                    lvl.text = GameData.Instance.DecreaseBreakSpeed.ToString();
                    break;
                case GoodType.CastSpeed:
                    lvl.text = GameData.Instance.CastSpeed.ToString();
                    break;
                default:
                    lvl.text = GameData.Instance.CatchZoneEasy.ToString();
                    break;
            }
        }

        private void Buy()
        {
            GameAudioController.Instance.Win();

            GameData.Instance.Cash -= Cost;

            switch (type)
            {
                case GoodType.CatchDistance:
                    GameData.Instance.CatchDistance++;
                    break;
                case GoodType.DecreaseBreakSpeed:
                    GameData.Instance.DecreaseBreakSpeed++;
                    break;
                case GoodType.CastSpeed:
                    GameData.Instance.CastSpeed++;
                    break;
                default:
                    GameData.Instance.CatchZoneEasy++;
                    break;
            }

            OnBuy?.Invoke();
        }

    }
}
