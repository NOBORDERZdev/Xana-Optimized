using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadEmoteAnimations : MonoBehaviour
{
    public GameObject animationPanel;
    public GameObject highlightAnim;

    public GameObject animationSelectionPanel;
    public GameObject animationSelectionPanelPotrait;

    public static bool animClick = false;

    public static LoadEmoteAnimations instance;
    void Awake()
    {
        instance = this;
        EmoteAnimationHandler.Instance.AnimHighlight = highlightAnim;
        EmoteAnimationHandler.Instance.popupPenal = animationPanel;
    }

    private void Start()
    {
        StartCoroutine(EmoteAnimationHandler.Instance.getAllAnimations());
    }

    public void OnEnable()
    {
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OpenAllAnimsPanel += AnimClick;
    }

    public void OnDisable()
    {
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OpenAllAnimsPanel -= AnimClick;
    }

    public void AnimClick()
    {
        animClick = true;
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
#if UNITY_EDITOR
        EmoteAnimationHandler.Instance.animationClick();
#endif
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            EmoteAnimationHandler.Instance.animationClick();
        }
#endif
    }


    public void OpenAnimationSelectionPanel()
    {
        if (ScreenOrientationManager._instance.isPotrait)
        {
            animationSelectionPanelPotrait.SetActive(true);
        }
        else
        {
            animationSelectionPanel.SetActive(true);
        }
    }
    
    
    public void CloseAnimationSelectionPanel()
    {
        if (ScreenOrientationManager._instance.isPotrait)
        {
            animationSelectionPanelPotrait.SetActive(false);
        }
        else
        {
            animationSelectionPanel.SetActive(false);
        }
    }

}
