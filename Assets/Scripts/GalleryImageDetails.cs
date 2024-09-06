using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GalleryImageDetails : MonoBehaviour
{
    [Header("Picture Index")]
    public int m_PictureIndex;

    [Header("Special Painting")]
    public bool m_IsSpecialPainting;
    public bool m_IsVideo;

    [Header("Banner Settings")]
    public bool m_IsPortrait;
    public bool m_isBanner;

    [Header("Mayor And Judge Frames")]
    public bool m_IsMayorAndJudgeFrame;
    public int m_MayorAndJudgeFrameIndex;

    public static Context CurrentContext;

    //public bool m_isCertificate;
    //public Sprite m_ZoomImages;
    //public _w dialogue;

    //public bool _isSpecialPainting;
    //public Texture2D specialPainting_texture2D;

    float clickTime = .2f;
    float tempTimer = 0;
    private void OnMouseDown()
    {
        tempTimer = Time.time;
        //if (PlayerSelfieController.Instance.m_IsSelfieFeatureActive) return;

        //if (!GalleryImageManager.Instance.m_IsDescriptionPanelActive && GalleryImageManager.Instance.CheckCanClickOnImage())
        //{
        //    if (m_IsMayorAndJudgeFrame)
        //    {
        //        GalleryImageManager.Instance.LoadMayorAndJudgeFrame(m_MayorAndJudgeFrameIndex);
        //    }
        //    else if (m_isBanner)
        //    {
        //        GalleryImageManager.Instance.LoadBannerImagePanel(m_IsPortrait, m_PictureIndex);
        //    }
        //    else if (m_IsVideo)
        //    {
        //        GalleryImageManager.Instance.m_IsVideo = m_IsVideo;
        //        GalleryImageManager.Instance.LoadVideo(m_PictureIndex);
        //    }
        //    else
        //    {
        //        //string l_InfoText = m_NPCTrigger.GetComponent<NPC_DialogueTrigger>().info_textMesh.text;
        //        //dialogue = m_NPCTrigger.GetComponent<NPC_DialogueTrigger>().dialogue;
        //        GalleryImageManager.Instance.LoadPictureDescriptionPanel(m_PictureIndex, m_IsSpecialPainting);
        //    }
        //}
    }

    private void OnMouseUp()
    {
        if (PlayerCameraController.IsPointerOverUIObject()) return;
        if ((Time.time-tempTimer) < clickTime)
        {
            OnPictureClicked();
            tempTimer = 0;
        }

    }

    public void OnPictureClicked()
    {
        if (PlayerSelfieController.Instance.m_IsSelfieFeatureActive) return;

        if (!GalleryImageManager.Instance.m_IsDescriptionPanelActive && GalleryImageManager.Instance.CheckCanClickOnImage())
        {
            if (m_IsMayorAndJudgeFrame)
            {
                CurrentContext = Context.MayorAndJudgeFrame;
                GalleryImageManager.Instance.LoadMayorAndJudgeFrame(m_MayorAndJudgeFrameIndex);
                
            }
            else if (m_isBanner)
            {
                CurrentContext = Context.IsBanner;
                GalleryImageManager.Instance.LoadBannerImagePanel(m_IsPortrait, m_PictureIndex);
                
            }
            else if (m_IsVideo)
            {
                CurrentContext = Context.IsVideo;
                GalleryImageManager.Instance.m_IsVideo = m_IsVideo;
                GalleryImageManager.Instance.LoadVideo(m_PictureIndex);
                
            }
            else if (m_IsPortrait)
            {
                if (!ScreenOrientationManager._instance.isPotrait)
                {
                    CurrentContext = Context.IsPortrait;
                    RectTransform rt = GalleryImageManager.Instance.m_Picture.GetComponent<RectTransform>();
                    float width = rt.rect.width;
                    float height = rt.rect.height;
                    width = 1200;
                    height = 800;

                    rt.sizeDelta = new Vector2(width, height);
                }
               
                GalleryImageManager.Instance.LoadPictureDescriptionPanel(m_PictureIndex, m_IsSpecialPainting);
                
            }
            else
            {
                //string l_InfoText = m_NPCTrigger.GetComponent<NPC_DialogueTrigger>().info_textMesh.text;
                //dialogue = m_NPCTrigger.GetComponent<NPC_DialogueTrigger>().dialogue;
                CurrentContext = Context.IsLandscape;
                GalleryImageManager.Instance.LoadPictureDescriptionPanel(m_PictureIndex, m_IsSpecialPainting);
                UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(false, true, false, false);
            }

            if(ConstantsHolder.xanaConstants.EnviornmentName.Contains("JJ MUSEUM"))
            {
                DataType myType = DataType.Image;
                if (m_IsVideo)
                    myType = DataType.Video;
                JjInfoManager.Instance.SendCallAnalytics(myType, "---");
            }

        }
    }
}

public enum Context
{
    MayorAndJudgeFrame,
    IsBanner,
    IsVideo,
    IsPortrait,
    IsLandscape
}