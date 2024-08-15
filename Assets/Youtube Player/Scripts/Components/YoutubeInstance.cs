using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using ZeelKheni.YoutubePlayer.Api;

namespace ZeelKheni.YoutubePlayer.Components
{
    public class YoutubeInstance : MonoBehaviour
    {
        public enum YoutubeInstanceType
        {
            Public,
            Custom
        }

        public YoutubeInstanceType InstanceType;

        public string CustomInstanceUrl;

        public string InstanceUrl => InstanceType == YoutubeInstanceType.Public ? m_PublicInstanceUrl : CustomInstanceUrl;
        public List<Models.YoutubeVideoInfo> YoutubeInstanceInfos;
        public string VideoJson;
        private string m_PublicInstanceUrl;

        public async Task<string> GetInstanceUrl(CancellationToken cancellationToken = default)
        {
            switch (InstanceType)
            {
                case YoutubeInstanceType.Public:
                    //if (string.IsNullOrEmpty(m_PublicInstanceUrl))
                    //{
                        Debug.LogWarning("Instance type is set to \"Public\". Fetching public instances every time is slow, and are only used for the sample to work. Please set a custom instance in the YoutubeInstance component.");
                        Debug.Log("Fetching Youtube public instances...");
                        YoutubeInstanceInfos = await YoutubeApi.GetPublicInstances(cancellationToken);

                        m_PublicInstanceUrl = YoutubeInstanceInfos[0].Uri;
                        Debug.Log($"Using Youtube public instance: {m_PublicInstanceUrl}");
                    //}

                    return m_PublicInstanceUrl;
                case YoutubeInstanceType.Custom:
                    return CustomInstanceUrl;

                default:
                    throw new System.ArgumentOutOfRangeException("InstanceType");
            }
        }
    }
}
