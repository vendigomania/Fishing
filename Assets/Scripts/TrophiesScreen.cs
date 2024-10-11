using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophiesScreen : MonoBehaviour
{
    [SerializeField] private List<TrophyVisualizator> visualizators = new List<TrophyVisualizator>();

    public void Show()
    {
        GameAudioController.Instance.Click();
        gameObject.SetActive(true);

        for(var i = 0; i < GameData.Instance.Trophies.Count; i++)
        {
            if(visualizators.Count <= i || i == 0)
            {
                visualizators.Add(Instantiate(visualizators[0], visualizators[0].transform.parent));
            }

            var data = GameData.Instance.Trophies[i];
            Color rareClr = rareClr = Color.clear;

            switch(data.Model.Rarity)
            {
                case FishModel.FishRarity.Uncommon:
                    rareClr = new Color(0f, 1f / 255 * 196, 1f / 255 * 243);
                    break;
                case FishModel.FishRarity.Rare:
                    rareClr = new Color(1f / 255 * 215, 1f / 255 * 71, 1f / 255 * 226);
                    break;
                case FishModel.FishRarity.Epic:
                    rareClr = new Color(1f, 1f / 255 * 157, 0f);
                    break;
            }
            visualizators[i].SetData(data.Model.Icon, data.Weight + "kg", data.Count.ToString(), rareClr);

            visualizators[i].gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        GameAudioController.Instance.Click();
        gameObject.SetActive(false);
    }
}
