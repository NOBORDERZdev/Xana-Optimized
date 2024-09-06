using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class XLDynamicText : MonoBehaviour
{
    [Header("Xana Lobby Dynamic Text")]
    public TextMeshProUGUI XLDynamicTextBanner;
    public TextMeshProUGUI XLDynamicTextThumbnail;
    // Start is called before the first frame update
    void Start()
    {
        GetXanaLobbyDynamicText();
    }
    public async void GetXanaLobbyDynamicText()
    {
        StringBuilder apiUrl = new StringBuilder();
        apiUrl.Append(ConstantsGod.API_BASEURL + ConstantsGod.GetXanaLobbyDynamicText);

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
                XanaLobbyDynamicText json = JsonConvert.DeserializeObject<XanaLobbyDynamicText>(request.downloadHandler.text);
                if (GameManager.currentLanguage == "ja" || LocalizationManager.forceJapanese)
                {
                    XLDynamicTextBanner.text = json.data.descriptionText[0];
                    XLDynamicTextThumbnail.text = json.data.descriptionText[0];
                }
                else if (GameManager.currentLanguage == "en")
                {
                    XLDynamicTextBanner.text = json.data.descriptionText[1];
                    XLDynamicTextThumbnail.text = json.data.descriptionText[1];
                }
            }
        }
    }
}
class XanaLobbyDynamicText
{
    public bool success;
    public DynamicTextData data;
    public string msg;
}
class DynamicTextData
{
    //public int id;
    public string[] descriptionText;
    //public DateTime createdAt;
    //public DateTime updatedAt;
}