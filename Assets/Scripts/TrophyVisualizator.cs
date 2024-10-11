using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrophyVisualizator : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text weightText;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Image rareGradient;

    public void SetData(Sprite _icon, string _weight, string _count, Color _clr)
    {
        icon.sprite = _icon;
        weightText.text = _weight;
        countText.text = _count;
        rareGradient.color = _clr;
    }
}
