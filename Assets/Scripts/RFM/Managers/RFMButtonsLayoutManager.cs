using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RFMButtonsLayoutManager : MonoBehaviour
{
    public Button saveLayoutBtn;
    public Button closePanelBtn;

    public GameObject currentSelectedObject;

    public RectTransform joyStick, runButton, jumpButton, slideButton;

    public Slider sizeSlider;

    public static RFMButtonsLayoutManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        sizeSlider.onValueChanged.AddListener(ResizeUI);
        RFM.EventsManager.onToggleSetLayoutPanel += ActivatePanel;

    }
    private void OnDisable()
    {
        sizeSlider.onValueChanged.RemoveAllListeners();
        RFM.EventsManager.onToggleSetLayoutPanel -= ActivatePanel;
    }

    public void ActivatePanel() 
    {
        transform.GetChild(0).gameObject.SetActive(true);
        sizeSlider.value = 0;
        LoadLayout();
    }

    public void SaveLayout()
    {
        PlayerPrefs.SetFloat("joyX", joyStick.anchoredPosition.x);
        PlayerPrefs.SetFloat("joyY", joyStick.anchoredPosition.y);
        PlayerPrefs.SetFloat("joyScaleX", joyStick.transform.localScale.x);
        PlayerPrefs.SetFloat("joyScaleY", joyStick.transform.localScale.y);

        PlayerPrefs.SetFloat("runX", runButton.anchoredPosition.x);
        PlayerPrefs.SetFloat("runY", runButton.anchoredPosition.y);
        PlayerPrefs.SetFloat("runScaleX", runButton.transform.localScale.x);
        PlayerPrefs.SetFloat("runScaleY", runButton.transform.localScale.y);

        PlayerPrefs.SetFloat("jumpX", jumpButton.anchoredPosition.x);
        PlayerPrefs.SetFloat("jumpY", jumpButton.anchoredPosition.y);
        PlayerPrefs.SetFloat("jumpScaleX", jumpButton.transform.localScale.x);
        PlayerPrefs.SetFloat("jumpScaleY", jumpButton.transform.localScale.y);

        PlayerPrefs.SetFloat("slideX", slideButton.anchoredPosition.x);
        PlayerPrefs.SetFloat("slideY", slideButton.anchoredPosition.y);
        PlayerPrefs.SetFloat("slideScaleX", slideButton.transform.localScale.x);
        PlayerPrefs.SetFloat("slideScaleY", slideButton.transform.localScale.y);



        CanvasButtonsHandler.inst.joyStickRMF.anchoredPosition = new Vector3(joyStick.anchoredPosition.x, joyStick.anchoredPosition.y, 0);
        CanvasButtonsHandler.inst.joyStickRFM.transform.localScale = new Vector3(joyStick.transform.localScale.x, joyStick.transform.localScale.y, 0);

        CanvasButtonsHandler.inst.slideButtonRMF.anchoredPosition = new Vector3(slideButton.anchoredPosition.x, slideButton.anchoredPosition.y, 0);
        CanvasButtonsHandler.inst.slideBtn.transform.localScale = new Vector3(slideButton.transform.localScale.x, slideButton.transform.localScale.y, 0);

        CanvasButtonsHandler.inst.runButtonRMF.anchoredPosition = new Vector3(runButton.anchoredPosition.x, runButton.anchoredPosition.y, 0);
        CanvasButtonsHandler.inst.runBtn.transform.localScale = new Vector3(runButton.transform.localScale.x, runButton.transform.localScale.y, 0);

        CanvasButtonsHandler.inst.jumpButtonRMF.anchoredPosition = new Vector3(jumpButton.anchoredPosition.x, jumpButton.anchoredPosition.y, 0);
        CanvasButtonsHandler.inst.jumpBtnRFM.transform.localScale = new Vector3(jumpButton.transform.localScale.x, jumpButton.transform.localScale.y, 0);

        PlayerPrefs.Save();

    }

    public void LoadLayout()
    {

        joyStick.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("joyX", 74.99997f), PlayerPrefs.GetFloat("joyY", 36), 0);
        joyStick.transform.localScale = new Vector3(PlayerPrefs.GetFloat("joyScaleX", 1), PlayerPrefs.GetFloat("joyScaleY", 1), 0);

        runButton.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("runX", -52.5f), PlayerPrefs.GetFloat("runY", 143.29f), 0);
        runButton.transform.localScale = new Vector3(PlayerPrefs.GetFloat("runScaleX", 1), PlayerPrefs.GetFloat("runScaleY", 1), 0);

        jumpButton.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("jumpX", -91), PlayerPrefs.GetFloat("jumpY", 73), 0);
        jumpButton.transform.localScale = new Vector3(PlayerPrefs.GetFloat("jumpScaleX", 1), PlayerPrefs.GetFloat("jumpScaleY", 1), 0);

        slideButton.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("slideX", -133.73f), PlayerPrefs.GetFloat("slideY", 143.29f), 0);
        slideButton.transform.localScale = new Vector3(PlayerPrefs.GetFloat("slideScaleX", 1), PlayerPrefs.GetFloat("slideScaleY", 1), 0);

        CanvasButtonsHandler.inst.joyStickRMF.anchoredPosition = new Vector3(joyStick.anchoredPosition.x, joyStick.anchoredPosition.y, 0);
        CanvasButtonsHandler.inst.joyStickRFM.transform.localScale = new Vector3(joyStick.transform.localScale.x, joyStick.transform.localScale.y, 0);

        CanvasButtonsHandler.inst.slideButtonRMF.anchoredPosition = new Vector3(slideButton.anchoredPosition.x, slideButton.anchoredPosition.y, 0);
        CanvasButtonsHandler.inst.slideBtn.transform.localScale = new Vector3(slideButton.transform.localScale.x, slideButton.transform.localScale.y, 0);

        CanvasButtonsHandler.inst.runButtonRMF.anchoredPosition = new Vector3(runButton.anchoredPosition.x, runButton.anchoredPosition.y, 0);
        CanvasButtonsHandler.inst.runBtn.transform.localScale = new Vector3(runButton.transform.localScale.x, runButton.transform.localScale.y, 0);

        CanvasButtonsHandler.inst.jumpButtonRMF.anchoredPosition = new Vector3(jumpButton.anchoredPosition.x, jumpButton.anchoredPosition.y, 0);
        CanvasButtonsHandler.inst.jumpBtnRFM.transform.localScale = new Vector3(jumpButton.transform.localScale.x, jumpButton.transform.localScale.y, 0);


    }

    public void ResetLayout()
    {
        
        PlayerPrefs.DeleteKey("joyX");
        PlayerPrefs.DeleteKey("joyScaleX");
        PlayerPrefs.DeleteKey("joyY");
        PlayerPrefs.DeleteKey("joyScaleY");
        PlayerPrefs.DeleteKey("runX");
        PlayerPrefs.DeleteKey("runScaleX");
        PlayerPrefs.DeleteKey("runY");
        PlayerPrefs.DeleteKey("runScaleY");
        PlayerPrefs.DeleteKey("jumpX");
        PlayerPrefs.DeleteKey("jumpScaleX");
        PlayerPrefs.DeleteKey("jumpY");
        PlayerPrefs.DeleteKey("jumpScaleY");
        PlayerPrefs.DeleteKey("slideX");
        PlayerPrefs.DeleteKey("slideScaleX");
        PlayerPrefs.DeleteKey("slideY");
        PlayerPrefs.DeleteKey("slideScaleY");
        PlayerPrefs.Save();
        

        joyStick.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("joyX", 74.99997f), PlayerPrefs.GetFloat("joyY", 36), 0);
        joyStick.transform.localScale = new Vector3(PlayerPrefs.GetFloat("joyScaleX", 1), PlayerPrefs.GetFloat("joyScaleY", 1), 0);

        runButton.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("runX", -52.5f), PlayerPrefs.GetFloat("runY", 143.29f), 0);
        runButton.transform.localScale = new Vector3(PlayerPrefs.GetFloat("runScaleX", 1), PlayerPrefs.GetFloat("runScaleY", 1), 0);

        jumpButton.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("jumpX", -91), PlayerPrefs.GetFloat("jumpY", 73), 0);
        jumpButton.transform.localScale = new Vector3(PlayerPrefs.GetFloat("jumpScaleX", 1), PlayerPrefs.GetFloat("jumpScaleY", 1), 0);

        slideButton.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("slideX", -133.73f), PlayerPrefs.GetFloat("slideY", 143.29f), 0);
        slideButton.transform.localScale = new Vector3(PlayerPrefs.GetFloat("slideScaleX", 1), PlayerPrefs.GetFloat("slideScaleY", 1), 0);

    }


    public void ResizeUI(float size)
    {
        if (currentSelectedObject != null)
        {
            currentSelectedObject.transform.localScale = new Vector3(1f + size, 1f + size, 0);
        }
    }
}
