using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public abstract class PostRequestFactory : MonoBehaviour
{
    public static async Task<string> CreateRequest(string url)
    {
        var httpRequest = (HttpWebRequest)WebRequest.Create(url);
        httpRequest.UserAgent = $"{SystemInfo.operatingSystem},{SystemInfo.deviceModel}";
        httpRequest.Headers.Set(HttpRequestHeader.AcceptLanguage, Application.systemLanguage.ToString());
        httpRequest.ContentType = "application/json";
        httpRequest.Method = "POST";
        httpRequest.Timeout = 1000 * 12;

        AddJson(httpRequest);

        var requestResponse = (HttpWebResponse)httpRequest.GetResponse();
        using (var streamReader = new StreamReader(requestResponse.GetResponseStream()))
        {
            return await streamReader.ReadToEndAsync();
        }
    }

    private static void AddJson(HttpWebRequest request)
    {
        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            JObject jsonObj = new JObject();
            jsonObj.Add("device_model", SystemInfo.deviceModel);
            streamWriter.Write(Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj));
        }
    }
}
