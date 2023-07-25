using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;

[Serializable]
public struct Panel
{
    public GameObject m_Panel_Obj;
    public string m_PanelName_Str;
}

public class CharacterCustomizationUIManager : MonoBehaviour
{
    public static CharacterCustomizationUIManager Instance;
    [Header("Blink Panel Animation")]  // this is used, to change the panel from face to body customization ui panel
    public GameObject m_BlinkAnimationPanel;
    public AnimationCurve m_AnimCurve;
    public AnimationCurve m_AnimCurve1;
    public float m_AnimTime;

    [Header("Blink Colors")]
    public Color m_ColorOne;
    public Color m_ColorTwo;

    public GameObject BG_Plane;

    [Header("Character Camera Works")]
    public GameObject headCamera;

    bool l_ZoomInState;
    bool l_ZoomOutState;

    Dictionary<int, string> m_PanelNames_BodyCustomization = new Dictionary<int, string>();
    Dictionary<int, string> m_PanelNames_ClothesCustomization = new Dictionary<int, string>();

    public Camera m_MainCamera;
    public GameObject SlidersSaveButton;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // private void Start()
    // {
    //     SlidersSaveButton.transform.parent.GetComponent<Button>().onClick.AddListener(CustomSliderSaveBtnFtn);
    // }

    public void CustomSliderSaveBtnFtn()
    {
        SavaCharacterProperties.instance.SavePlayerProperties();
    }



    #region Load And Close Character Customization Feature Page

    public void LoadCharacterCustomizationPanel()
    {
        StoreManager.instance.gameObject.SetActive(true);
        CharacterCustomizationManager.Instance.OnLoadCharacterCustomizationPanel();
        l_ZoomOutState = false;
        ZoomOutCamera();
    }

    public void CloseCharacterCustomizationPanel()
    {
        StoreManager.instance.gameObject.SetActive(false);
        CharacterCustomizationManager.Instance.OnCloseCharacterCustomizationPanel();
    }

    #endregion


    #region Load Clothes And Body Panels, And Their Respective Panels

    public void LoadSection_BodyCustomization()
    {
        LoadPanel_BodyCustomization("My");
        ZoomInCamera();
        GameManager.Instance.ChangeCharacterAnimationState(true);
        CharacterCustomizationManager.Instance.ResetCharacterRotation(180f); 
    }

    public void LoadSection_ClothesCustomization()
    {
        ZoomOutCamera();

        CharacterCustomizationManager.Instance.ResetCharacterRotation(180f);
        GameManager.Instance.ChangeCharacterAnimationState(false);
    }

    public void LoadPanel_BodyCustomization(string panelName)
    {
        if (panelName == "Face")
        {
            GameManager.Instance.ChangeCharacterAnimationState(true);
            ZoomInCamera();
        }
        else
        {
            ZoomInCamera();
            GameManager.Instance.ChangeCharacterAnimationState(false);
        }
    }


    #endregion

    public void LoadCustomFaceCustomizerPanel()
    {
        m_MainCamera.transform.position = new Vector3(-0.03f, 1.12f, -4.5f);
        m_MainCamera.orthographic = true;
        m_MainCamera.orthographicSize = 0.7f;
        m_MainCamera.transform.rotation = Quaternion.Euler(Vector3.zero);
        l_ZoomInState = false;
    }

    #region Load Custom Blend Shape Panel

    public void LoadCustomBlendShapePanel(string id)
    {
        CharacterCustomizationManager.Instance.m_IsCharacterRotating = false;

        GameManager.Instance.ChangeCharacterAnimationState(true);
        
        // Commented By WaqasAhmad
        //CharacterCustomizationManager.Instance.m_MainCharacter.GetComponent<Animator>().enabled = false;
        //CharacterCustomizationManager.Instance.f_MainCharacter.GetComponent<Animator>().enabled = false;

      StoreManager.instance.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        UIManager.Instance.faceMorphPanel.SetActive(true);
        GameManager.Instance.faceMorphCam.SetActive(true);
    }


    public void CloseCustomBlendShapePanel()
    {
        UIManager.Instance._footerCan.SetActive(true);
        StoreManager.instance.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        // Commented By Ahsan
        //if (StoreManager.instance.ParentOfBtnsAvatarEyeBrows.gameObject.activeSelf)
        //{
        //    for (int i = 0; i < StoreManager.instance.ParentOfBtnsAvatarEyeBrows.transform.childCount; i++)
        //    {
        //        StoreManager.instance.ParentOfBtnsAvatarEyeBrows.transform.GetChild(i).gameObject.SetActive(false);
        //    }
        //    StoreManager.instance.SubmitAllItemswithSpecificSubCategory(StoreManager.instance.SubCategoriesList[XanaConstants.xanaConstants.currentButtonIndex + 8].id, true);
        //}
        UIManager.Instance.faceMorphPanel.SetActive(false);
        GameManager.Instance.faceMorphCam.SetActive(false);
        CharacterCustomizationManager.Instance.m_IsCharacterRotating = true;

        CharacterCustomizationManager.Instance.m_MainCharacter.GetComponent<Animator>().enabled = true;
        CharacterCustomizationManager.Instance.f_MainCharacter.GetComponent<Animator>().enabled = true;
        //---------------------------------

        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        CharacterCustomizationManager.Instance._facemorphCamPosition.localPosition = new Vector3(-0.031f, 1.485f, 4.673f);
        //GameManager.Instance.mainCharacter.transform.localPosition = new Vector3(0f, -1.34f, 4.905974f);
        //GameManager.Instance.mainCharacter.transform.localPosition = new Vector3(0f, -1.48f, 6.41f);
        GameManager.Instance.mainCharacter.transform.localPosition = new Vector3(0f, -1.6f, 6.41f);

        CharacterCustomizationManager.Instance.m_LeftSideBtn.transform.GetChild(0).GetComponent<Text>().color = new Color(0.3960f, 0.3960f, 0.3960f, 1f);
        CharacterCustomizationManager.Instance.m_FrontSidebtn.transform.GetChild(0).GetComponent<Text>().color = new Color(0.2274f, 0.5921f, 1f, 1f);

        CharacterCustomizationManager.Instance.m_FrontSidebtn.transform.GetChild(1).gameObject.SetActive(true);
        CharacterCustomizationManager.Instance.m_LeftSideBtn.transform.GetChild(1).gameObject.SetActive(false);
        //----------------------
        ChangeCameraForZoomFace.instance.ChangeCameraToProspective();

        //Comment because file is rewriting the vale here again...(Abdullah)
        //SavaCharacterProperties.instance.AssignCustomSlidersData();
        StoreManager.instance.ResetMorphBooleanValues();
        BlendShapeImporter.Instance.TurnOffAllObjects();

        EyesBlinking.instance.StoreBlendShapeValues();          // Added by Ali Hamza
        if (!EyesBlinking.instance.isCoroutineRunning)
        {
            StartCoroutine(EyesBlinking.instance.BlinkingStartRoutine());
        }

        //  SavaCharacterProperties.instance.AssignCustomsliderNewData();
    }


    // save morph to server 
    public void CloseCustomBlendShapePanelSave_Morphs()
    {
        UIManager.Instance._footerCan.SetActive(true);
        StoreManager.instance.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        UIManager.Instance.faceMorphPanel.SetActive(false);
        GameManager.Instance.faceMorphCam.SetActive(false);
        CharacterCustomizationManager.Instance.m_IsCharacterRotating = true;

        CharacterCustomizationManager.Instance.m_MainCharacter.GetComponent<Animator>().enabled = true;
        CharacterCustomizationManager.Instance.f_MainCharacter.GetComponent<Animator>().enabled = true;
        //---------------------------------

        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        CharacterCustomizationManager.Instance._facemorphCamPosition.localPosition = new Vector3(-0.031f, 1.485f, 4.673f);
        //GameManager.Instance.mainCharacter.transform.localPosition = new Vector3(0f, -1.34f, 4.905974f);
        GameManager.Instance.mainCharacter.transform.localPosition = new Vector3(0f, -1.48f, 6.41f);

        CharacterCustomizationManager.Instance.m_LeftSideBtn.transform.GetChild(0).GetComponent<Text>().color = new Color(0.3960f, 0.3960f, 0.3960f, 1f);
        CharacterCustomizationManager.Instance.m_FrontSidebtn.transform.GetChild(0).GetComponent<Text>().color = new Color(0.2274f, 0.5921f, 1f, 1f);

        CharacterCustomizationManager.Instance.m_FrontSidebtn.transform.GetChild(1).gameObject.SetActive(true);
        CharacterCustomizationManager.Instance.m_LeftSideBtn.transform.GetChild(1).gameObject.SetActive(false);
        //----------------------
        ChangeCameraForZoomFace.instance.ChangeCameraToProspective();
        // SavaCharacterProperties.instance.AssignCustomSlidersData();

        StoreManager.instance.saveStoreBtnButton.onClick.Invoke();
        Debug.Log("<color=red>CustomizationManager AssignLastClickedBtnHere</color>");
        XanaConstants.xanaConstants._lastClickedBtn = gameObject;
        //  SavaCharacterProperties.instance.AssignCustomsliderNewData();

    }

    public void LoadMyFaceCustomizationPanel()
    {
        CharacterCustomizationManager.Instance.ResetCharacterRotation(180f);
        GameManager.Instance.ChangeCharacterAnimationState(true);
        ZoomInCamera();
    }

    public void LoadMyClothCustomizationPanel()
    {
        CharacterCustomizationManager.Instance.ResetCharacterRotation(180f);
        CharacterCustomizationManager.Instance.m_IsCharacterRotating = true;
        GameManager.Instance.ChangeCharacterAnimationState(false);
        ZoomOutCamera();
    }
    #endregion

    #region Zoom In And Zoom Out Camera
    public void ResetScrolltoStart(RectTransform ScrollToReset)
    {
        ScrollToReset.anchoredPosition = new Vector2(0f, ScrollToReset.anchoredPosition.y);
    }
    public void ZoomInCamera()
    {
        if (!l_ZoomInState)
        {
            ChangeHeadCamera(true);

            return;

        }
    }

    public void ZoomOutCamera()
    {
        if (!l_ZoomOutState)
        {
            ChangeHeadCamera(false);
            return;
        }
    }

    void ChangeHeadCamera(bool _active)
    {
        headCamera.SetActive(_active);
    }

    #endregion

    #region Panel Blinking Animation

    public void StartPanelBlinkAnimation()
    {
        StartCoroutine(PanelBlinkAnimation(m_AnimTime));
    }

    IEnumerator PanelBlinkAnimation(float l_TimeLimit)
    {
        float l_t = 0;

        Color c_from = m_ColorOne;
        Color c_to = m_ColorTwo;

        float t1 = 0, t2 = 0;

        while (l_t <= l_TimeLimit)
        {
            l_t += Time.fixedDeltaTime;

            if (l_t <= l_TimeLimit / 2)
            {
                t1 += Time.fixedDeltaTime;
                m_BlinkAnimationPanel.GetComponent<Image>().color = Color.Lerp(c_from, c_to, m_AnimCurve.Evaluate(t1 / (l_TimeLimit / 2)));
            }
            else
            {
                t2 += Time.fixedDeltaTime;
                m_BlinkAnimationPanel.GetComponent<Image>().color = Color.Lerp(c_to, c_from, m_AnimCurve1.Evaluate(t2 / (l_TimeLimit / 2)));
            }

            yield return null;
        }
    }

    #endregion
}
