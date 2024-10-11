using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shop
{
    public class ShopScreen : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private CanvasGroup group;

        [SerializeField] private ShopGoodVis[] goods;

        private void Start()
        {
            foreach (var good in goods)
                good.OnBuy += () => UpdateAll();
        }

        public void Show()
        {
            GameAudioController.Instance.Click();

            gameObject.SetActive(true);
            group.interactable = false;

            Invoke("ActiveCloseBtn", 1f);

            UpdateAll();
        }

        public void Hide()
        {
            GameAudioController.Instance.Click();

            animator.Play("Hide");
            group.interactable = false;

            Invoke("DeactiveteScreen", 1f);
        }

        private void ActiveCloseBtn()
        {
            group.interactable = true;
        }

        private void DeactiveteScreen()
        {
            gameObject.SetActive(false);
        }

        private void UpdateAll()
        {
            foreach (var good in goods) good.UpdateInfo();
        }
    }
}
