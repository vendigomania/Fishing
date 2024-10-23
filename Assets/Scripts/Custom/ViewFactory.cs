using System.Threading.Tasks;
using UnityEngine;

namespace Service
{
    public class ViewFactory : MonoBehaviour
    {
        public UniWebView uniWV;
        int windowsCount = 1;

        public ViewFactory CreateView(GameObject _gm, RectTransform _safeArea)
        {
            UniWebView.SetAllowJavaScriptOpenWindow(true);

            uniWV = _gm.AddComponent<UniWebView>();
            uniWV.OnOrientationChanged += (view, orientation) => UpdateView();

            uniWV.SetAcceptThirdPartyCookies(true);

            uniWV.SetAllowBackForwardNavigationGestures(true);
            uniWV.SetSupportMultipleWindows(true, true);
            uniWV.OnShouldClose += (view) => view.CanGoBack || windowsCount > 1;
            uniWV.OnMultipleWindowOpened += (view, id) => windowsCount++;
            uniWV.OnMultipleWindowClosed += (view, id) => windowsCount--;
            uniWV.ReferenceRectTransform = _safeArea;

            return this;
        }

        public async void UpdateView()
        {
            await Task.Delay(100);

            if (Screen.width < Screen.height) Portrait();
            else Landscape();

            uniWV.ReferenceRectTransform.offsetMin = Vector2.zero;
            uniWV.ReferenceRectTransform.offsetMax = Vector2.zero;


            uniWV.UpdateFrame();
        }

        private void Portrait()
        {
            float average = (2 * Screen.safeArea.yMax + Screen.height) / 3;
            uniWV.ReferenceRectTransform.anchorMin = Vector2.zero;
            uniWV.ReferenceRectTransform.anchorMax = new Vector2(1, average / Screen.height);
        }

        private void Landscape()
        {
            uniWV.ReferenceRectTransform.anchorMin = Vector2.zero;
            uniWV.ReferenceRectTransform.anchorMax = Vector2.one;
        }
    }
}
