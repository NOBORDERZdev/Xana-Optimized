using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text.RegularExpressions;
using System.Linq;
using System;


namespace ZeelKheni.YoutubePlayer.Api
{
    public class FetchHtmlContent
    {
        private string hlsurl;


        public IEnumerator GetHtmlContent(string url, Action<string> callback)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string htmlContent = webRequest.downloadHandler.text;
               


                hlsurl = ParseHLSManifestUrl(htmlContent);
                Debug.LogError("HLS found  " + hlsurl);
            }
            callback(hlsurl);
        }

        string ParseHLSManifestUrl(string htmlContent)
        {
            string pattern = @"hlsManifestUrl"":""([^""]+)""";
            Match match = Regex.Match(htmlContent, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                Debug.LogError("HLS Manifest URL not found in the HTML content.");
                return null;
            }
        }
    }
}
