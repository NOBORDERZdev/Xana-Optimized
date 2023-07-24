using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JjWorldInfo : MonoBehaviour
{
    [SerializeField] int id;
    [SerializeField] JjRatio NftRatio;
    
    //JjInfoManager infoManager;
    float clickTime = .2f;
    float tempTimer = 0;

    private void Awake()
    {
        //infoManager = JjInfoManager.Instance;
    }
    private void OnMouseDown()
    {
        tempTimer = Time.time;
       
    }

    private void OnMouseUp()
    {
        if (CameraLook.IsPointerOverUIObject()) return;
        if ((Time.time - tempTimer) < clickTime)
        {
            OpenWorldInfo();
            tempTimer = 0;
        }

    }

    public void OpenWorldInfo() {
        if (SelfieController.Instance.m_IsSelfieFeatureActive) return;

        if (JjInfoManager.Instance != null)
        {
            if (GameManager.currentLanguage.Contains("en") && !CustomLocalization.forceJapanese )
            {
                JjInfoManager.Instance.SetInfo(NftRatio,JjInfoManager.Instance.worldInfos[id].Title[0], JjInfoManager.Instance.worldInfos[id].Aurthor[0], JjInfoManager.Instance.worldInfos[id].Des[0], JjInfoManager.Instance.worldInfos[id].WorldImage, JjInfoManager.Instance.worldInfos[id].Type, JjInfoManager.Instance.worldInfos[id].VideoLink);
            }
            else if(CustomLocalization.forceJapanese || GameManager.currentLanguage.Equals("ja"))
            {
                if (!JjInfoManager.Instance.worldInfos[id].Title[1].IsNullOrEmpty() && !JjInfoManager.Instance.worldInfos[id].Aurthor[1].IsNullOrEmpty() && !JjInfoManager.Instance.worldInfos[id].Des[1].IsNullOrEmpty())
                {
                    JjInfoManager.Instance.SetInfo(NftRatio,JjInfoManager.Instance.worldInfos[id].Title[1], JjInfoManager.Instance.worldInfos[id].Aurthor[1], JjInfoManager.Instance.worldInfos[id].Des[1], JjInfoManager.Instance.worldInfos[id].WorldImage, JjInfoManager.Instance.worldInfos[id].Type, JjInfoManager.Instance.worldInfos[id].VideoLink);
                }
            }
        }
    }
}
