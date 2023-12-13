using UnityEngine;
using System.Text.RegularExpressions;

public class PMY_WorldInfo : MonoBehaviour
{
    [SerializeField] int id;
    [SerializeField] PMY_Ratio NftRatio;

    float clickTime = .2f;
    float tempTimer = 0;
    public void Testing()
    {
        Debug.LogError("Hello");
    }

    private void OnMouseDown()
    {
        tempTimer = Time.time;
        Debug.LogError("onmouse down");
    }

    private void OnMouseUp()
    {
        Debug.LogError("onmouse up");
        if (CameraLook.IsPointerOverUIObject()) return;
        if ((Time.time - tempTimer) < clickTime)
        {
            if (WorldItemView.m_EnvName.Contains("XANA Lobby"))
                OpenWorldInfo();
            tempTimer = 0;
        }

    }

    public void OpenWorldInfo()
    {
        if (SelfieController.Instance.m_IsSelfieFeatureActive) return;

        if (PMY_Nft_Manager.Instance != null)
        {
            if (GameManager.currentLanguage.Contains("en") && !CustomLocalization.forceJapanese)
            {
                PMY_Nft_Manager.Instance.SetInfoForXanaLobby(NftRatio, PMY_Nft_Manager.Instance.worldInfos[id].Title[0], PMY_Nft_Manager.Instance.worldInfos[id].Aurthor[0], PMY_Nft_Manager.Instance.worldInfos[id].Des[0], PMY_Nft_Manager.Instance.worldInfos[id].Texture, PMY_Nft_Manager.Instance.worldInfos[id].Type);
            }
            else if (CustomLocalization.forceJapanese || GameManager.currentLanguage.Equals("ja"))
            {
                PMY_Nft_Manager.Instance.SetInfoForXanaLobby(NftRatio, PMY_Nft_Manager.Instance.worldInfos[id].Title[1], PMY_Nft_Manager.Instance.worldInfos[id].Aurthor[1], PMY_Nft_Manager.Instance.worldInfos[id].Des[1], PMY_Nft_Manager.Instance.worldInfos[id].Texture, PMY_Nft_Manager.Instance.worldInfos[id].Type);
            }
        }
    }

    void PublishLog()
    {
        // for firebase analytics
        int languageMode = CustomLocalization.forceJapanese ? 1 : 0;
        Debug.Log("<color=red> LanguageMode: " + languageMode + "</color>");
        if (JjInfoManager.Instance.worldInfos[id].Title[languageMode].IsNullOrEmpty())
        {
            string sceneName = FindObjectOfType<StayTimeTracker>().worldName;
            //Firebase.Analytics.FirebaseAnalytics.LogEvent(sceneName + "_NFT" + id + "_Click");
            Debug.Log("<color=red>" + sceneName + "_NFT" + id + "_Click </color>");
        }
        else
            SendFBLogs(JjInfoManager.Instance.worldInfos[id].Title[languageMode]);
    }

    private void SendFBLogs(string data)
    {
        data = Regex.Replace(data, @"\s", "");
        string trimmedString = data.Substring(0, Mathf.Min(data.Length, 18));
        //Firebase.Analytics.FirebaseAnalytics.LogEvent(trimmedString + "_NFT_Click");
        Debug.Log("<color=red>" + trimmedString + "_NFT_Click" + "</color>");
    }

}
