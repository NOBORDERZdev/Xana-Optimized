using Newtonsoft.Json;

namespace ZeelKheni.YoutubePlayer.Models
{
    public class FormatInfo
    {
        [JsonProperty("url")]
        public string Url { get; set;}

        [JsonProperty("itag")]
        public string Itag { get; set;}

        public override string ToString()
        {
            return $"Itag: {Itag}, Url: {Url}";
        }
    }
}
