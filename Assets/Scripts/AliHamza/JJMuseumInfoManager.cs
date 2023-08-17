using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using Cysharp.Threading.Tasks;
public class JJMuseumInfoManager : MonoBehaviour
{
    [NonReorderable]
    public List<GameObject> NftPlaceholder;
    [SerializeField] int JJMusuemId;
    // Start is called before the first frame update


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
                Debug.Log("<color=red>" + request.error + " </color>");
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
                    JjInfoManager.Instance.worldInfos[i].WorldImage = null;
                    JjInfoManager.Instance.worldInfos[i].VideoLink = null;
                    JjInfoManager.Instance.worldInfos[i].isAWSVideo = false;
                    JjInfoManager.Instance.worldInfos[i].isLiveVideo = false;
                }
                JjInfoManager.Instance.InitData(json, NftPlaceholder);
            }
        }
    }
}
