using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Race
{
    public class RaceTableRow : MonoBehaviour
    {
        [SerializeField] private TMP_Text placeL;
        [SerializeField] private TMP_Text nameL;
        [SerializeField] private TMP_Text valueL;

        public void SetData(string _place, string _name, string _result, bool _isPlayer = false)
        {
            placeL.text = _place;
            nameL.text = _name;
            valueL.text = _result;

            nameL.color = _isPlayer ? Color.yellow : Color.white;
        }
    }
}
