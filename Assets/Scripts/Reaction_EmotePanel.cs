using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reaction_EmotePanel : MonoBehaviour
{
    public GameObject m_ReactPanel;
    public GameObject m_EmotePanel;
    public GameObject m_EmotePanelHighlighter;
    public GameObject m_ReactPanelHighlighter;
    public Button m_reactbtn;
    public Sprite disable_sprite;
    public Sprite enable_sprite;
    public Image ActionBtn;
    public Image ReactionBtn;
    public Sprite SelectedAction_Sprite;
    public Sprite UnSelectedAction_Sprite;
    public Sprite SelectedReaction_Sprite;
    public Sprite UnSelectedReaction_Sprite;

    //Hardik Changes
    public GameObject JyosticksObject;
    public GameObject BottomObject;
    public GameObject XanaChatObject;
    public GameObject JumpObject;
    //end hardik
    private Canvas reactScreenCanvas;//riken
    private GraphicRaycaster graphicRaycaster;//riken

    public ReactScreen reactScreenInstance;

    public static Reaction_EmotePanel instance;
    private void Awake()
    {
        instance = this;
    }


    public void OnEnable()
    {
        if (instance != this)
            instance = this;
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OpenAllAnimsPanel += OpenPanel;
    }

    public void OnDisable()
    {
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OpenAllAnimsPanel -= OpenPanel;
    }


    public void ReactionOn()
    {
        if (reactScreenCanvas == null)
        {
            reactScreenCanvas = m_ReactPanel.AddComponent<Canvas>();
            graphicRaycaster = m_ReactPanel.AddComponent<GraphicRaycaster>();
            reactScreenCanvas.overrideSorting = true;
            reactScreenCanvas.sortingOrder = 6;
        }

        reactScreenInstance.CheckForInstantiation();

        //if (SoundController.Instance.name.Contains("potrait"))
        //{
        //    JyosticksObject.gameObject.transform.DOLocalMoveY(29.1f, 1);
        //    JumpObject.gameObject.transform.DOLocalMoveY(29.1f, 1);
        //    BottomObject.SetActive(false);
        //    XanaChatObject.SetActive(false);
        //}
        m_ReactPanel.SetActive(true);
        m_EmotePanel.SetActive(false);
        if (ScreenOrientationManager._instance.isPotrait)
        {

            //if (ScreenOrientationManager._instance.isPotrait)
            //    ScreenOrientationManager._instance.joystickInitPosY = JyosticksObject.transform.localPosition.y;

            //BottomObject.SetActive(false);
            //XanaChatObject.SetActive(false);
            m_ReactPanel.transform.DOLocalMoveY(-108f, 0.1f);
            //JyosticksObject.transform.DOLocalMoveY(10f, 1);
            //JumpObject.transform.DOLocalMoveY(10f, 1);
        }

       
        m_EmotePanelHighlighter.SetActive(false);
        m_ReactPanelHighlighter.SetActive(true);
        ReactionBtn.sprite = SelectedReaction_Sprite;
        ActionBtn.sprite = UnSelectedAction_Sprite;
        m_reactbtn.GetComponent<Image>().sprite = enable_sprite;
    }

    public void ReactionOff()
    {
        m_ReactPanel.SetActive(false);
        m_ReactPanelHighlighter.SetActive(false);
        m_EmotePanelHighlighter.SetActive(true);
        m_reactbtn.GetComponent<Image>().sprite = disable_sprite;
    }


    public void EmoteOn()
    {

        //if (SoundController.Instance.name.Contains("potrait"))
        //{
        //    JyosticksObject.gameObject.transform.DOLocalMoveY(29.1f, 1);
        //    JumpObject.gameObject.transform.DOLocalMoveY(29.1f, 1);
        //    BottomObject.SetActive(false);
        //    XanaChatObject.SetActive(false);
        //}
        m_EmotePanel.SetActive(true);
        m_ReactPanel.SetActive(false);
        if (ScreenOrientationManager._instance.isPotrait)
        {
            //if (ScreenOrientationManager._instance.isPotrait)
            //    ScreenOrientationManager._instance.joystickInitPosY = JyosticksObject.transform.localPosition.y;

            //BottomObject.SetActive(false);
            //XanaChatObject.SetActive(false);
            m_EmotePanel.transform.DOLocalMoveY(-108f, 0.1f);
            //JyosticksObject.transform.DOLocalMoveY(10f, 1);
            //JumpObject.transform.DOLocalMoveY(10f, 1);
        }
     
       
        m_EmotePanelHighlighter.SetActive(true);
        m_ReactPanelHighlighter.SetActive(false);
        ActionBtn.sprite = SelectedAction_Sprite;
        ReactionBtn.sprite = UnSelectedReaction_Sprite;
        m_reactbtn.GetComponent<Image>().sprite = disable_sprite;
    }

    public void OpenPanel()
    {
        Debug.Log("call hua times 4===" + GamePlayButtonEvents.inst.selectionPanelOpen);
        if (GamePlayButtonEvents.inst.selectionPanelOpen)
        {
            if (m_ReactPanel.activeInHierarchy)
            {
              //  Debug.Log("Close hua 1=== Call");
                ReactionOn();

            }
            else
              //  Debug.Log("Close hua 2=== Call");
            EmoteOn();
        }
        else
        {
            Debug.Log("Close hua=== Call");
            if (SoundController.Instance != null)
            {
                if (SoundController.Instance.name.Contains("potrait"))
                {
                    // ReferencesForGamePlay.instance.RotateBtn.interactable = false;
                    JyosticksObject.gameObject.transform.DOLocalMoveY(29.1f, 0.1f);
                    JumpObject.gameObject.transform.DOLocalMoveY(29.1f, 0.1f);
                    BottomObject.SetActive(false);
                    XanaChatObject.SetActive(false);
                    //  ReferencesForGamePlay.instance.RotateBtn.interactable = true;
                }
            }
            m_EmotePanel.SetActive(true);
            m_EmotePanelHighlighter.SetActive(true);
            
            //m_EmotePanel.transform.DOLocalMoveY(-563f, 1);
            //if (Input.deviceOrientation == DeviceOrientation.Portrait)
            //{
             
            //}
        }

        
    }
}
