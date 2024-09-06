using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICustomizer : MonoBehaviour
{
    public Slider sizeSlider;
    public TextMeshProUGUI sizeSliderText;
    public Slider transperancySlider;
    public TextMeshProUGUI transparencyText;
    public UIDrag SelectButton;
    public UIDrag[] buttons;
    public Button openPanel;
    public GameObject dummyPanel;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(4f);
        GameObject go = GameObject.Find("ButtonSettings");
        openPanel = go.GetComponent<Button>();
        openPanel.onClick.AddListener(OpenDummyPanel);
        //sizeSlider.onValueChanged.AddListener(delegate { UpdateSizeText(); });
        //transperancySlider.onValueChanged.AddListener(delegate { UpdateTransparencyText(); });
        //sizeSliderText.text = (int)sizeSlider.value * 100 + "%";
        //transparencyText.text = (int)transperancySlider.value * 100 + "%";
    }

    void OpenDummyPanel() {
        dummyPanel.SetActive(true);
    }
    private void Update()
    {
        sizeSliderText.text = (int)sizeSlider.value * 100 + "%";
        transparencyText.text = (int)transperancySlider.value * 100 + "%";
        if (SelectButton) {
            SelectButton.SetSizeandTransparency(sizeSlider.value,transperancySlider.value);
        }
    }
    void UpdateSizeText() {
        sizeSliderText.text = (int)sizeSlider.value*100+"%";
    }
    void UpdateTransparencyText() {
        transparencyText.text = (int)transperancySlider.value * 100 + "%";
    }

    public void SetButtonData(float size,float transparency) {
        sizeSlider.value = size;
        transperancySlider.value = transparency;
    }

    public void SaveData() {
        foreach (var b in buttons) {
            b.SaveData();
        }
    }
    public void ResetData() {
        foreach (var b in buttons) {
            b.RestUI();
        }
    }
    public void LoadUI()
    {
        foreach (var b in buttons)
        {
            b.LoadUI();
        }
    }

}
