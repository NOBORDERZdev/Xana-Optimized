using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class JjWorldInfo : MonoBehaviour
{
    [SerializeField] int id;
    [SerializeField] JjRatio NftRatio;
    
    //JjInfoManager infoManager;
    float clickTime = .2f;
    float tempTimer = 0;
    public void Testing()
    {
        //Debug.LogError("Hello");
    }

    private void OnMouseDown()
    {
        tempTimer = Time.time;
        //Debug.LogError("onmouse down");
    }

    private void OnMouseUp()
    {
        //Debug.LogError("onmouse up");
        if (PlayerCameraController.IsPointerOverUIObject()) return;
        if ((Time.time - tempTimer) < clickTime)
        {
            //OpenWorldInfo();
            //  PublishLog();
            if (WorldItemView.m_EnvName.Contains("XANA Lobby"))
                OpenWorldInfo();
            tempTimer = 0;
        }

    }

    public void OpenWorldInfo()
    {
        if (PlayerSelfieController.Instance.m_IsSelfieFeatureActive) return;

        if (JjInfoManager.Instance != null)
        {
            if (GameManager.currentLanguage.Contains("en") && !LocalizationManager.forceJapanese)
            {
                JjInfoManager.Instance.SetInfoForXanaLobby(NftRatio, JjInfoManager.Instance.worldInfos[id].Title[0], JjInfoManager.Instance.worldInfos[id].Aurthor[0], JjInfoManager.Instance.worldInfos[id].Des[0], JjInfoManager.Instance.worldInfos[id].Texture, JjInfoManager.Instance.worldInfos[id].Type);
            }
            else if (LocalizationManager.forceJapanese || GameManager.currentLanguage.Equals("ja"))
            {
                JjInfoManager.Instance.SetInfoForXanaLobby(NftRatio, JjInfoManager.Instance.worldInfos[id].Title[1], JjInfoManager.Instance.worldInfos[id].Aurthor[1], JjInfoManager.Instance.worldInfos[id].Des[1], JjInfoManager.Instance.worldInfos[id].Texture, JjInfoManager.Instance.worldInfos[id].Type);
            }
        }
    }

    void PublishLog()
    {
        // for firebase analytics
        int languageMode = LocalizationManager.forceJapanese ? 1 : 0;
        //Debug.Log("<color=red> LanguageMode: " + languageMode + "</color>");
        if (JjInfoManager.Instance.worldInfos[id].Title[languageMode].IsNullOrEmpty())
        {
            string sceneName = FindObjectOfType<StayTimeTracker>().worldName;
            //Firebase.Analytics.FirebaseAnalytics.LogEvent(sceneName + "_NFT" + id + "_Click");
            //Debug.Log("<color=red>" + sceneName + "_NFT" + id + "_Click </color>");
        }
        else
            SendFBLogs(JjInfoManager.Instance.worldInfos[id].Title[languageMode]);
    }

    private void SendFBLogs(string data)
    {
        data = Regex.Replace(data, @"\s", "");
        string trimmedString = data.Substring(0, Mathf.Min(data.Length, 18));
        //Firebase.Analytics.FirebaseAnalytics.LogEvent(trimmedString + "_NFT_Click");
        //Debug.Log("<color=red>" + trimmedString + "_NFT_Click" + "</color>");
    }

}

