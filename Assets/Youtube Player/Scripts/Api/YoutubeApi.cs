using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ZeelKheni.YoutubePlayer.Extensions;
using ZeelKheni.YoutubePlayer.Models;

namespace ZeelKheni.YoutubePlayer.Api
{
    public class YoutubeApi
    {
        public static async Task<VideoInfo> GetVideoInfo(string YoutubeUrl, string videoId, CancellationToken cancellationToken = default, List<Models.YoutubeVideoInfo> urls = null,bool skipPrevious = false)
        {
            var requestUrl = $"{YoutubeUrl}/api/v1/videos/{videoId}";
            var videoInfo = await WebRequest.GetAsync<VideoInfo>(requestUrl, cancellationToken, urls, videoId,skipPrevious);
            if (videoInfo?.VideoThumbnails != null)
            {
                foreach (var thumbnail in videoInfo.VideoThumbnails)
                {
                    if (thumbnail.Url.StartsWith("/"))
                    {
                        thumbnail.Url = YoutubeUrl + thumbnail.Url;
                    }
                }
            }
            return videoInfo;
        }

        public static async Task<List<YoutubeVideoInfo>> GetPublicInstances(CancellationToken cancellationToken = default)
        {
            var requestUrl = "https://api.invidious.io/instances.json?pretty=1&sort_by=type,users";
            var data = await WebRequest.GetAsync<JArray>(requestUrl, cancellationToken);

            return data
                .SelectTokens("$[*][1]")
                .Where(token => IsValidInstance(token as JObject))
                .Select(token => token.ToObject<YoutubeVideoInfo>())
                .ToList();
        }

        private static bool IsValidInstance(JObject jobject)
        {
            return jobject.IsValidField("type", JTokenType.String, "https")
                && jobject.IsValidField("api", JTokenType.Boolean, true)
                && jobject.IsValidField("cors", JTokenType.Boolean, true);
        }
    }
}
