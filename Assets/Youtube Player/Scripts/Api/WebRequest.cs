using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
using ZeelKheni.YoutubePlayer.Extensions;
using UnityEngine;
using System.Collections.Generic;
using ZeelKheni.YoutubePlayer.Components;

namespace ZeelKheni.YoutubePlayer
{
    public class WebRequest
    {
        static string previousBase;
        public static async Task<T> GetAsync<T>(string requestUrl, CancellationToken cancellationToken = default, List<Models.YoutubeVideoInfo> urlslist = null, string videoid = null, bool skipPrevious = false)
        {
            var request = UnityWebRequest.Get(requestUrl);
            request.timeout = 10;
            try
            {
                await request.SendWebRequestAsync(cancellationToken);
               // Debug.Log("<color=red>Response .... " + request.result.ToString() + "  \n " + request.downloadHandler.text + "    \n    </color>");
                var text = request.downloadHandler.text;
                if (string.IsNullOrEmpty(text)) { throw new NullReferenceException(); }
               if(skipPrevious && previousBase == requestUrl) { throw new NullReferenceException(); }
                GameObject.FindObjectOfType<YoutubeInstance>().VideoJson = text;
                previousBase = requestUrl;
                return JsonConvert.DeserializeObject<T>(text, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception exception)
            {
              //  Debug.Log(exception  + "   " + requestUrl);
                if (urlslist != null)
                {
                    urlslist.RemoveAt(0);
                    return await GetAsync<T>($"{urlslist[0].Uri}/api/v1/videos/{videoid}", cancellationToken, urlslist, videoid);
                }
                return JsonConvert.DeserializeObject<T>("", new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore});
            }
            finally
            {
                request.Dispose();
            }
        }

        public static async Task<long> HeadAsync(string requestUrl, CancellationToken cancellationToken = default)
        {
            var request = UnityWebRequest.Head(requestUrl);
            try
            {
                await request.SendWebRequestAsync(cancellationToken);
                return request.responseCode;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return request.responseCode;
            }
            finally
            {
                request.Dispose();
            }
        }
    }
}
