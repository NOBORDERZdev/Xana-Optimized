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
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        EmoteAnimationPlay.Instance.AnimHighlight = highlightAnim;
        EmoteAnimationPlay.Instance.popupPenal = animationPanel;
    }

    private void Start()
    {
        StartCoroutine(EmoteAnimationPlay.Instance.getAllAnimations());
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
        EmoteAnimationPlay.Instance.animationClick();
#endif
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            EmoteAnimationPlay.Instance.animationClick();
        }
#endif
    }


    public void OpenAnimationSelectionPanel()
    {
        if (ChangeOrientation_waqas._instance.isPotrait)
        {
            animationSelectionPanelPotrait.SetActive(true);
        }
        else
        {
            animationSelectionPanel.SetActive(true);
        }
       
       
        //Reaction_EmotePanel.instance.ReactionOff();
    }
    
    
    public void CloseAnimationSelectionPanel()
    {
        if (ChangeOrientation_waqas._instance.isPotrait)
        {
            animationSelectionPanelPotrait.SetActive(false);
        }
        else
        {
            animationSelectionPanel.SetActive(false);
        }
        //Reaction_EmotePanel.instance.ReactionOff();
    }

}
