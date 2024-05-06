using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
public class JJMuseumInfoManager : MonoBehaviour
{
    [NonReorderable]
    public List<GameObject> NftPlaceholder;
    [SerializeField] int JJMusuemId_test;
    [SerializeField] int JJMusuemId_main;
    [SerializeField] int JJMusuemId;

    private void Start()
    {
        if (APIBasepointManager.instance.IsXanaLive)
            JJMusuemId = JJMusuemId_main;
        else
            JJMusuemId = JJMusuemId_test;
    }

    public async void InitJJMuseumInfoManager()
    {
        StringBuilder apiUrl = new StringBuilder();
        apiUrl.Append(ConstantsGod.API_BASEURL + ConstantsGod.JJWORLDASSET + JJMusuemId);

        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl.ToString()))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            await request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                //Debug.Log("<color=red>" + request.error + " </color>");
            }
            else
            {
                StringBuilder data = new StringBuilder();
                data.Append(request.downloadHandler.text);
                JjJson json = JsonConvert.DeserializeObject<JjJson>(data.ToString());
                for(int i=0; i < JjInfoManager.Instance.worldInfos.Count; i++)
                {
                    JjInfoManager.Instance.worldInfos[i].Title = new string[0];
                    JjInfoManager.Instance.worldInfos[i].Aurthor = new string[0];
                    JjInfoManager.Instance.worldInfos[i].Des = new string[0];
                    JjInfoManager.Instance.worldInfos[i].url = null;
                    JjInfoManager.Instance.worldInfos[i].WorldImage = null;
                    JjInfoManager.Instance.worldInfos[i].VideoLink = null;
                    JjInfoManager.Instance.worldInfos[i].isAWSVideo = false;
                    JjInfoManager.Instance.worldInfos[i].isLiveVideo = false;
                }
                StartCoroutine(JjInfoManager.Instance.InitData(json, NftPlaceholder));
            }
        }
    }
}
