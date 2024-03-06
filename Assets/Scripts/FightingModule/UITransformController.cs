using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITransformController : MonoBehaviour
{
    //[SerializeField] GameObject BottomRightContainer;
    //[SerializeField] GameObject TopRightContainer;
    //[SerializeField] GameObject ModifierButtonConainer;
    //[SerializeField] GameObject JoyStickContainer;
    [SerializeField] GameObject DummyUIPanel;
    [SerializeField] Button openButton;
    [SerializeField] Button saveButton;
    [SerializeField] Button resetButton;
    [SerializeField] Button resetAllButton;
    [SerializeField] Slider sizeSlider;
    [SerializeField] Button editButton;
    [SerializeField] GameObject tipText;

    public delegate void OpenModificationPanelDelegate(Color c, RectTransform range);
    public static OpenModificationPanelDelegate openModificationEvent;

    public delegate void UIEventsDelegate();
    public static UIEventsDelegate uiClickEvent;
    public static UIEventsDelegate saveAndCloseEvent;
    public static UIEventsDelegate resetEvent;
    public static UIEventsDelegate resetAllEvent;

    public delegate void UISizeChangeDelegate(float value);
    public static UISizeChangeDelegate sliderChangeEvent;
    public static UISizeChangeDelegate SetSliderValueEvent;

    public delegate void EditMode(bool edit);
    public static EditMode OnEditModeChanged;

    public RectTransform rangeRectTransform; //Range with in the UI can be moved
    public Color highlightColor;
    

    private void OnEnable()
    {
        SetSliderValueEvent += SetSliderValue;
    }

    private void OnDisable()
    {
        SetSliderValueEvent -= SetSliderValue;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(4f);
        if (!openButton)
        {
            // openButton= FightingGameManager.instance.OpenButton;
            GameObject button = GameObject.Find("ButtonSettings");
            openButton = button.GetComponent<Button>();
        }
        openButton.onClick.AddListener(() =>
        {
            //BottomRightContainer.SetActive(true);
            //TopRightContainer.SetActive(true);
            //ModifierButtonConainer.SetActive(true);
            //JoyStickContainer.SetActive(true);
            DummyUIPanel.SetActive(true);
            openModificationEvent?.Invoke(highlightColor, rangeRectTransform);
            OnEditModeChanged?.Invoke(false);
            editButton.gameObject.SetActive(true);
            sizeSlider.interactable = false;
            tipText.SetActive(false);

        });
    }

    void EnableEditMode()
    {
        OnEditModeChanged?.Invoke(true);
        editButton.gameObject.SetActive(false);
        sizeSlider.interactable = true;
        tipText.SetActive(true);
    }
    private void Awake()
    {
       
        saveButton.onClick.AddListener(() =>
        {
            saveAndCloseEvent();
            //BottomRightContainer.SetActive(false);
            //TopRightContainer.SetActive(false);
            //ModifierButtonConainer.SetActive(false);
            DummyUIPanel.SetActive(false);
            saveAndCloseEvent(); //call again so the applier script tries again to get the latest values
            //the UI transformer saves data late
        });
        resetButton.onClick.AddListener(() =>
        {
            resetEvent();
        });
        resetAllButton.onClick.AddListener(() =>
        {
            resetAllEvent();
        });

        sizeSlider.onValueChanged.AddListener(OnSliderMoved);
        editButton.onClick.AddListener(EnableEditMode);
    }
    void OnSliderMoved(float val)
    {
        sliderChangeEvent(val);
    }
    public void SetSliderValue(float val)
    {
        sizeSlider.value = val;
    }

}
