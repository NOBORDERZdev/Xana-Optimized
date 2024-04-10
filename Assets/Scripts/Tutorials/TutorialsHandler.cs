using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialsHandler : MonoBehaviour
{
    public static TutorialsHandler instance;
    private CanvasScaler canvasScaler;
    public Button skipButton;
    public Button rightNextButton;
    public Button leftNextButton;
    private TextMeshProUGUI rightNextBtnText;
    private TextMeshProUGUI leftNextBtnText;
    public Button crossButton;
    public Button okButton;
    private GameObject xanaLobby;
    public GameObject lobbyParent;
    public GameObject tutorialCanvasBG;
    public List<GameObject> panels;
    public List<Sprite> subtractSprites;
    private int currentPanelIndex = 0;
    public Toggle toggle;
    public Sprite toggleCheckedBG;
    public Sprite toggleUnCheckedBG;
    public Image toggleBG;
    void Start()
    {
        instance = this;
        rightNextButton.onClick.AddListener(NextButtonClicked);
        leftNextButton.onClick.AddListener(NextButtonClicked);
        skipButton.onClick.AddListener(SkipTutorial);
        crossButton.onClick.AddListener(CrossButton);
        okButton.onClick.AddListener(OkButtonClicked);
        canvasScaler = this.GetComponent<CanvasScaler>();
        skipButton.gameObject.SetActive(true);
        rightNextBtnText = rightNextButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        leftNextBtnText = leftNextButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    public void ShowTutorials()
    {
       /* if (PlayerPrefs.GetInt("ShowTutorial") == 0 && !ConstantsHolder.xanaConstants.isTutorialLoaded && !XanaEventDetails.eventDetails.DataIsInitialized)
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
            DisplayPanel(currentPanelIndex);
            ConstantsHolder.xanaConstants.isTutorialLoaded = true;
        }*/
    }
    private void HandleButtons(int index)
    {
        if (index <= 4)
        {
            rightNextButton.gameObject.SetActive(true);
            leftNextButton.gameObject.SetActive(false);
            if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.Expand)
            {
                RectTransform rt = rightNextButton.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(275, 104);
                rt.anchoredPosition = new Vector2(-38,74);
                rt = skipButton.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(160, 35);
                rt.anchoredPosition = new Vector2(-74, -152);
                rightNextBtnText.fontSize = 32;
            }
        }
        else if (index == 7)
        {
            rightNextButton.gameObject.SetActive(false);
            leftNextButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
        }
        else
        {
            rightNextButton.gameObject.SetActive(false);
            leftNextButton.gameObject.SetActive(true);
            if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.Expand)
            {
                RectTransform rt = leftNextButton.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(275, 104);
                rt.anchoredPosition = new Vector2(38, 74);
                rt = skipButton.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(160, 35);
                rt.anchoredPosition = new Vector2(-74, -152);
                leftNextBtnText.fontSize = 32;
            }
        }
    }
    private void DisablePanels()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(false);
        }
    }
    private void DisplayPanel(int index)
    {
        if (index == 0 || index == 1 || index == 2)
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        else
        {
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        }
        DisablePanels();
        panels[index].SetActive(true);
        HandleButtons(index);
        if (index == 1)
        {
            xanaLobby = WorldManager.instance.EventPrefabLobby;
            ShowWorlds(index);
        }
        else if (index == 2)
        {
            tutorialCanvasBG.SetActive(false);
            ShowWorlds(index);
        }
        else
        {
            tutorialCanvasBG.SetActive(true);
        }
       
    }
    public void NextButtonClicked()
    {
        if (currentPanelIndex <= panels.Count - 1)
        {
            currentPanelIndex++;
            if (currentPanelIndex == 3)
            {
               currentPanelIndex = 7;
            }
            DisplayPanel(currentPanelIndex);
        }
    }
    private void ShowWorlds(int index)
    {
        GameObject go;
        if (index == 1) // instantiate lobby
        {
            go= Instantiate(xanaLobby);
            go.transform.SetParent(lobbyParent.transform);
            go.GetComponent<RectTransform>().Stretch();
            go.transform.GetChild(0).GetChild(3).GetComponent<Image>().sprite = subtractSprites[0];
            go.transform.localScale = Vector3.one;
            go.GetComponent<Button>().enabled = false;
        }
        else //instantiate first 6 worlds
        {
            /*List<WorldItemDetail> Worlds = WorldManager.instance.WorldItemManager.Get6WorldsForTutorial();
            for (int i = 0; i < 6; i++)
            {
                panels[2].transform.GetChild(1).GetChild(0).GetChild(i).
                    GetComponent<TutorialWorldItemView>().Init(
                    Worlds[i].EnvironmentName,
                    Worlds[i].ThumbnailDownloadURL
                    );

            }*/
        }
    }
    private void SkipTutorial()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
    }
    private void CrossButton()
    {
        this.gameObject.SetActive(false);
    }
    public void CheckToggle()
    {
        if (toggle.isOn)
        {
            toggleBG.sprite = toggleCheckedBG;
            toggleBG.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            toggleBG.sprite = toggleUnCheckedBG;
            toggleBG.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    private void OkButtonClicked()
    {
            if (toggle.isOn)
            {
                PlayerPrefs.SetInt("ShowTutorial", 1);
                this.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                PlayerPrefs.SetInt("ShowTutorial", 0);
                this.transform.GetChild(0).gameObject.SetActive(false);
            }
    }
}
