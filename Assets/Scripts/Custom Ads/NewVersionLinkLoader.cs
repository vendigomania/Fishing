using Newtonsoft.Json.Linq;
using System;
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
        [SerializeField] private string domainName;
        [SerializeField] private string apiKey;

        private string mainRequestFormat => $"https://{domainName}/session/v1/{apiKey}";

        private string osRequestLink = "https://app.njatrack.tech/technicalPostback/v1.0/postClientParams";

        [SerializeField] private Text test_resultLable;

        [SerializeField] private bool test_ShowLogs;
        [SerializeField] private bool test_ClearPrefs;
        [SerializeField] private bool test_showOnlyResponseAfterRequest;

        private const string SavedDataKey = "SavedKey";
        public string[] UserAgentRequestValue => new string[] { SystemInfo.operatingSystem, SystemInfo.deviceModel };

        class CpaObject
        {
            public string device_model;
        }

        private void Start()
        {
            if (test_ClearPrefs) PlayerPrefs.DeleteAll();

            OneSignalPlugAdapter.Initialize();

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                PrintLog("NoInternet");
                ShowOld();
            }
            else
            {
                var startLink = PlayerPrefs.GetString(SavedDataKey, "null");
                if (startLink == "null")
                {
                    requestResult = Request(mainRequestFormat + $"?device_model={SystemInfo.deviceModel}&");
                }
                else
                {
                    OpenWView(startLink);
                }
            }
        }

        Task<string> requestResult;
        float osInitializeDelay = 0f;

        private void Update()
        {
            if (requestResult != null && requestResult.IsCompleted)
            {
                CheckMainResult();
                requestResult = null;
            }

            if (osInitializeDelay > 0f)
            {
                if (string.IsNullOrEmpty(OneSignalPlugAdapter.UserOSIdentificator)) return;

                osInitializeDelay -= Time.deltaTime;

                if (osInitializeDelay <= 0f)
                {
                    string clientId = jResponseBody.Property("client_id")?.Value.ToString();

                    string endDomain = osRequestLink;
                    var rec = Request($"{endDomain}/{clientId}" + $"?onesignal_player_id={OneSignalPlugAdapter.UserOSIdentificator}");

                    PlayerPrefs.SetString(SavedDataKey, uniWebView.Url);
                    PlayerPrefs.Save();
                }
            }
        }

        JObject jResponseBody;

        private void CheckMainResult()
        {
            if (requestResult.IsFaulted)
            {
                PrintLog("NJI request fail");

                ShowOld();
            }
            else
            {
                if (test_showOnlyResponseAfterRequest)
                {
                    PrintLog(requestResult.Result);
                    return;
                }

                jResponseBody = JObject.Parse(requestResult.Result);

                if (jResponseBody.ContainsKey("response"))
                {
                    var link = jResponseBody.Property("response").Value.ToString();

                    if (string.IsNullOrEmpty(link))
                    {
                        PrintLog("NJI link is empty");
                        ShowOld();
                    }
                    else
                    {
                        if (link.Contains("privacypolicyonline"))
                        {
                            ShowOld();
                        }
                        else
                        {
                            OpenWView(link);
                            osInitializeDelay = 3f;
                        }
                    }
                }
                else
                {
                    PrintLog("NJI no response");
                    ShowOld();
                }
            }
        }

        [SerializeField] private GameObject viewBack;
        [SerializeField] private RectTransform safeArea;
        UniWebView uniWebView;
        int openTabsCount = 1;

        private void OpenWView(string url)
        {
            viewBack.SetActive(true);

            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;

            try
            {
                UniWebView.SetAllowJavaScriptOpenWindow(true);

                uniWebView = gameObject.AddComponent<UniWebView>();
                uniWebView.OnOrientationChanged += (view, orientation) =>
                {
                // Set full screen again. If it is now in landscape, it is 640x320.
                Invoke("ResizeView", Time.deltaTime);
                };

                uniWebView.SetAcceptThirdPartyCookies(true);

                ResizeView();

                uniWebView.Load(url);
                uniWebView.Show();
                uniWebView.SetAllowBackForwardNavigationGestures(true);
                uniWebView.SetSupportMultipleWindows(true, true);
                uniWebView.OnShouldClose += (view) => view.CanGoBack || openTabsCount > 1;
                uniWebView.OnMultipleWindowOpened += (view, id) => openTabsCount++;
                uniWebView.OnMultipleWindowClosed += (view, id) => openTabsCount--;
            }
            catch (Exception ex)
            {
                test_resultLable.text += $"\n {ex}";
            }
        }

        private void ResizeViewSafeArea()
        {
            Rect safeArea = Screen.safeArea;
            if (Screen.width < Screen.height)
            {
                float avg = (2 * safeArea.yMax + Screen.height) / 3;
                this.safeArea.anchorMin = Vector2.zero;
                this.safeArea.anchorMax = new Vector2(1, avg / Screen.height);
            }
            else
            {
                this.safeArea.anchorMin = Vector2.zero;
                this.safeArea.anchorMax = Vector2.one;
            }
            this.safeArea.offsetMin = Vector2.zero;
            this.safeArea.offsetMax = Vector2.zero;
        }

        private void ResizeView()
        {
            ResizeViewSafeArea();
            uniWebView.ReferenceRectTransform = safeArea;
            uniWebView.UpdateFrame();
        }

        #region requests

        public async Task<string> Request(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.UserAgent = string.Join(", ", UserAgentRequestValue);
            httpWebRequest.Headers.Set(HttpRequestHeader.AcceptLanguage, Application.systemLanguage.ToString());
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 12000;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {

                //string json = JsonUtility.ToJson(new CpaObject
                //{
                //    device_model = SystemInfo.deviceModel,
                //});
                //streamWriter.Write(json);

                JObject json = new JObject();
                json.Add("device_model", SystemInfo.deviceModel);
                streamWriter.Write(Newtonsoft.Json.JsonConvert.SerializeObject(json));
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return await streamReader.ReadToEndAsync();
            }
        }

        #endregion

        private void ShowOld()
        {
            StopAllCoroutines();

            if (PlayerPrefs.HasKey(SavedDataKey)) OneSignalPlugAdapter.UnSubscribe();

            SceneManager.LoadScene(1);
        }


        private void PrintLog(string msg)
        {
            if (test_ShowLogs) test_resultLable.text += (msg + '\n');
        }
    }
}
