using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class SuccessCatchScreen : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Image icon;
        [SerializeField] private GameObject[] rarity;
        [SerializeField] private TMP_Text sizeValue;
        [SerializeField] private TMP_Text weightValue;
        [SerializeField] private TMP_Text priceValue;

        [SerializeField] private GameObject closeBtn;

        public void Show(Sprite _icon, FishModel.FishRarity _rarity, string _size, string _weight, string _price)
        {
            gameObject.SetActive(true);

            closeBtn.SetActive(false);

            icon.sprite = _icon;
            icon.SetNativeSize();

            for(int i = 0; i < rarity.Length; i++)
            {
                rarity[i].SetActive(i == (int)_rarity);
            }
            sizeValue.text = _size;
            weightValue.text = _weight + "kg";
            priceValue.text = "+"+_price;

            Invoke("ActiveCloseBtn", 1f);
        }

        public void Hide()
        {
            animator.Play("Hide");
            closeBtn.SetActive(false);

            Invoke("DeactiveteScreen", 1f);
        }

        private void ActiveCloseBtn()
        {
            closeBtn.SetActive(true);
        }

        private void DeactiveteScreen()
        {
            gameObject.SetActive(false);
        }
    }
}
