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
            //OpenWorldInfo();
            PublishLog();
            tempTimer = 0;
        }

    }

    //public void OpenWorldInfo() {
    //    if (SelfieController.Instance.m_IsSelfieFeatureActive) return;

    //    if (JjInfoManager.Instance != null)
    //    {
    //        if (GameManager.currentLanguage.Contains("en") && !CustomLocalization.forceJapanese )
    //        {
    //            JjInfoManager.Instance.SetInfo(NftRatio,JjInfoManager.Instance.worldInfos[id].Title[0], JjInfoManager.Instance.worldInfos[id].Aurthor[0], JjInfoManager.Instance.worldInfos[id].Des[0], JjInfoManager.Instance.worldInfos[id].WorldImage, JjInfoManager.Instance.worldInfos[id].Type, JjInfoManager.Instance.worldInfos[id].VideoLink, JjInfoManager.Instance.worldInfos[id].videoType);
    //        }
    //        else if(CustomLocalization.forceJapanese || GameManager.currentLanguage.Equals("ja"))
    //        {
    //            if (!JjInfoManager.Instance.worldInfos[id].Title[1].IsNullOrEmpty() && !JjInfoManager.Instance.worldInfos[id].Aurthor[1].IsNullOrEmpty() && !JjInfoManager.Instance.worldInfos[id].Des[1].IsNullOrEmpty())
    //            {
    //                JjInfoManager.Instance.SetInfo(NftRatio,JjInfoManager.Instance.worldInfos[id].Title[1], JjInfoManager.Instance.worldInfos[id].Aurthor[1], JjInfoManager.Instance.worldInfos[id].Des[1], JjInfoManager.Instance.worldInfos[id].WorldImage, JjInfoManager.Instance.worldInfos[id].Type, JjInfoManager.Instance.worldInfos[id].VideoLink,JjInfoManager.Instance.worldInfos[id].videoType);
    //            }
    //        }
    //    }
    //}

    void PublishLog()
    {
        // for firebase analytics
        int languageMode = CustomLocalization.forceJapanese ? 1 : 0;
        Debug.Log("<color=red> LanguageMode: " + languageMode + "</color>");
        if (JjInfoManager.Instance.worldInfos[id].Title[languageMode].IsNullOrEmpty())
        {
            string sceneName = FindObjectOfType<StayTimeTracker>().worldName;
            Firebase.Analytics.FirebaseAnalytics.LogEvent(sceneName + "_NFT" + id + "_Click");
            Debug.Log("<color=red>" + sceneName + "_NFT" + id + "_Click </color>");
        }
        else
            SendFBLogs(JjInfoManager.Instance.worldInfos[id].Title[languageMode]);
    }

    private void SendFBLogs(string data)
    {
        data = Regex.Replace(data, @"\s", "");
        string trimmedString = data.Substring(0, Mathf.Min(data.Length, 18));
        Firebase.Analytics.FirebaseAnalytics.LogEvent(trimmedString + "_NFT_Click");
        Debug.Log("<color=red>" + trimmedString + "_NFT_Click" + "</color>");
    }

}

