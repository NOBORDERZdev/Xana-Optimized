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
                Debug.Log("<color=red>Error: " + webRequest.error + "</color>");
            }
            else
            {
                string htmlContent = webRequest.downloadHandler.text;
                hlsurl = ParseHLSManifestUrl(htmlContent);
                Debug.Log("<color=red>HLS found " + hlsurl + "</color>");
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
                Debug.Log("<color=red>HLS Manifest URL not found in the HTML content.</color>");
                return null;
            }
        }
    }
}
