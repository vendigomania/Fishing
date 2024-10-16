using Newtonsoft.Json.Linq;
using System;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppVersionChecker : MonoBehaviour
{
    [SerializeField] private Transform newVersionAd;

    private string checkServiceLnk = "https://yandex.com/time/sync.json?geo=213";


    void Start()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            SceneManager.LoadScene(1);
            return;
        }

        using (WebClient client = new WebClient())
        {
            var loadedJSON = client.DownloadString(checkServiceLnk);

            var mills = JObject.Parse(loadedJSON).Property("time").Value.ToObject<long>();

            DateTime rim = new DateTime(2024, 10, 18);
            DateTime current = new DateTime(1970, 1, 1).AddMilliseconds(mills);

            if(current > rim)
                newVersionAd.gameObject.gameObject.SetActive(true);
            else
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}
