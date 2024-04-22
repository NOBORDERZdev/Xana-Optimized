using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.UI.Extensions;

public class SplashVideoPlay : MonoBehaviour
{
    public string video_download_link;
    public VideoPlayer player;
    float delay = 1.5f;
    float alpha = 1f;
    public GameObject button;
    public Image Image;
    public GameObject SplashvideoObj;
    
    public void RefrenceDownloafVideo()
    {
        Image = UserRegisterationManager.instance.BlackScreen.GetComponent<Image>();

#if UNITY_ANDROID
        video_download_link = SplashVideoDownloader.splashVideoDownloader.playbackDir;
#endif
#if UNITY_IPHONE
       video_download_link= SplashVideoDownloader.splashVideoDownloader.playbackDir;
#endif
        player.url = video_download_link;
        player.loopPointReached += End;
        SkipButton();
    }

    void End(VideoPlayer VP) {
        SplashvideoObj.SetActive(false);
        OnAvatarSelectionPanal();
        StartCoroutine(FadeOut());
    }
    void SkipButton()
    {
        StartCoroutine(ShowButtonAfterDelay());
    }

    IEnumerator ShowButtonAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        button.SetActive(true);
    }
    IEnumerator FadeOut()
    {
        player.targetTexture.Release();

        while (alpha > 0.0f)
        {
            yield return new WaitForSeconds(0.05f);
            alpha -= 0.05f;
            Image.color = new Color(0f, 0f, 0f, alpha);
        }
        UserRegisterationManager.instance.BlackScreen.SetActive(false);
        
    }
    public void Onskipp() {
        SplashvideoObj.SetActive(false);
        OnAvatarSelectionPanal();
        StartCoroutine(FadeOut());
    }

   public void OnAvatarSelectionPanal() 
    {
        InventoryManager.instance.StartPanel_PresetParentPanel.SetActive(true);
        //  InventoryManager.instance._CanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;        ------ Comment By Abdullah for Avatar Selection on OnBoarding
    }

}
