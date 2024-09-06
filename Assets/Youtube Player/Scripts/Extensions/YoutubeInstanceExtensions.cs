using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZeelKheni.YoutubePlayer.Api;
using ZeelKheni.YoutubePlayer.Components;
using ZeelKheni.YoutubePlayer.Models;

namespace ZeelKheni.YoutubePlayer.Extensions
{
    public static class YoutubeInstanceExtensions
    {
        public static async Task<string> GetVideoUrl(this YoutubeInstance YoutubeInstance, string videoId, bool proxyVideo = false, string itag = null, CancellationToken cancellationToken = default)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            // /latest_version endpoint is not supported in WebGL (CORS)
            return await YoutubeInstance.GetVideoUrlWebGl(videoId, itag, cancellationToken);
#else
            var url = await YoutubeInstance.GetInstanceUrl(cancellationToken);

            url = $"{url}/latest_version?id={videoId}";

            if (proxyVideo)
            {
                url += "&local=true";
            }

            if (!string.IsNullOrEmpty(itag))
            {
                url += $"&itag={itag}";
            }

            return url;
#endif
        }

        static async Task<string> GetVideoUrlWebGl(this YoutubeInstance YoutubeInstance, string videoId, string itag = null, CancellationToken cancellationToken = default)
        {
            var instanceUrl = await YoutubeInstance.GetInstanceUrl(cancellationToken);
            var videoInfo = await YoutubeApi.GetVideoInfo(instanceUrl, videoId, cancellationToken);

            if (string.IsNullOrEmpty(itag))
            {
                // 720p, the highest quality available with the video and audio combined
                itag = "22";
            }

            var format = videoInfo.FormatStreams.Find(f => f.Itag == itag);
            if (format == null)
            {
                // Maybe we're looking for an adaptive format?
                format = videoInfo.AdaptiveFormats.Find(f => f.Itag == itag);
            }

            if (format == null)
            {
                // On older videos, itag 22 may not be available
                // Get any format at this point
                format = videoInfo.FormatStreams.LastOrDefault();
            }

            if (format == null)
            {
                throw new InvalidOperationException("No video format found");
            }

            var uri = new Uri(format.Url);
            var instanceUri = new Uri(instanceUrl);
            var builder = new UriBuilder(uri)
            {
                Host = instanceUri.Host,
                Scheme = instanceUri.Scheme,
                Port = instanceUri.Port
            };

            return builder.Uri.ToString();
        }

        public static async Task<VideoInfo> GetVideoInfo(this YoutubeInstance YoutubeInstance, string videoId, CancellationToken cancellationToken = default)
        {
            var instanceUrl = await YoutubeInstance.GetInstanceUrl(cancellationToken);
            return await YoutubeApi.GetVideoInfo(instanceUrl, videoId, cancellationToken);
        }

        public static string GetVideoThumbnailUrl(this YoutubeInstance YoutubeInstance, string videoId)
        {
            var instanceUrl = YoutubeInstance.InstanceUrl;
            if (string.IsNullOrEmpty(instanceUrl))
            {
                throw new InvalidOperationException("InstanceUrl is null or empty. Call YoutubeInstance.GetInstanceUrl() first.");
            }

            return $"{instanceUrl}/vi/{videoId}/mqdefault.jpg";
        }
    }
}
