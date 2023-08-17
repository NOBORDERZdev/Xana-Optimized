using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialsManager : MonoBehaviour
{
    public static TutorialsManager instance;
    private CanvasScaler canvasScaler;
    public Button skipButton;
    public Button rightNextButton;
    public Button leftNextButton;
    public Button crossButton;
    public Button okButton;
    private GameObject worldsParent;
    private GameObject xanaLobby;
    public GameObject tutorialsParent;
    public GameObject lobbyParent;
    public GameObject tutorialCanvasBG;
    public GameObject thirdPanel;
    public List<GameObject> panels;
    public List<Sprite> subtractSprites;
    private int currentPanelIndex = 0;
    public Toggle toggle;
    public Sprite toggleCheckedBG;
    public Sprite toggleUnCheckedBG;
    public Image toggleBG;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        rightNextButton.onClick.AddListener(NextButtonClicked);
        leftNextButton.onClick.AddListener(NextButtonClicked);
        skipButton.onClick.AddListener(SkipTutorial);
        crossButton.onClick.AddListener(CrossButton);
        okButton.onClick.AddListener(OkButtonClicked);
        canvasScaler = this.GetComponent<CanvasScaler>();
       // PlayerPrefs.SetInt("ShowTutorial", 0);
        skipButton.gameObject.SetActive(true);
    }
    public void ShowTutorials()
    {
        if (PlayerPrefs.GetInt("ShowTutorial") == 0)
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
            DisplayPanel(currentPanelIndex);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void HandleButtons(int index)
    {
        if (index <= 4)
        {
            rightNextButton.gameObject.SetActive(true);
            leftNextButton.gameObject.SetActive(false);
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
        DisablePanels();
        panels[index].SetActive(true);
        HandleButtons(index);
        if (index == 1)
        {
            xanaLobby = WorldManager.instance.eventPrefabLobby;
            ShowWorlds(index);
        }
        else if (index == 2)
        {
            worldsParent = WorldManager.instance.listParentHotSection.gameObject;
            thirdPanel.SetActive(true);
            tutorialCanvasBG.SetActive(false);
            ShowWorlds(index);
        }
        else
        {
            thirdPanel.SetActive(false);
            tutorialCanvasBG.SetActive(true);
        }
        if (index==0 || index == 1)
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        else
        {
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        }
    }
    public void NextButtonClicked()
    {
        if (currentPanelIndex <= panels.Count - 1)
        {
            currentPanelIndex++;
            if (currentPanelIndex == 3 && UserRegisterationManager.instance.LoggedInAsGuest)
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
            //go.transform.GetChild(0).GetChild(3).GetComponent<Image>().color = new Color(80, 80, 80,255);
            go.transform.GetChild(0).GetChild(3).GetComponent<Image>().sprite = subtractSprites[0];
            go.transform.localScale = Vector3.one;
            go.GetComponent<Button>().enabled = false;
        }
        else //instantiate first 6 worlds
        {
            for (int i = 0; i < 6; i++)
            {
                go = Instantiate(worldsParent.transform.GetChild(i).gameObject);
                go.transform.SetParent(tutorialsParent.transform);
                go.transform.localScale = Vector3.one;
                go.transform.GetChild(0).GetChild(2).GetComponent<Image>().sprite = subtractSprites[1];
                // go.transform.GetChild(0).GetChild(2).GetComponent<Image>().color = new Color(80, 80, 80,255);
                go.GetComponent<Button>().enabled = false;
            }
        }
    }
    private void SkipTutorial()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        //PlayerPrefs.SetInt("ShowTutorial", 0);
    }
    private void CrossButton()
    {
        thirdPanel.SetActive(false);
        this.gameObject.SetActive(false);
    }
    public void CheckToggle()
    {
        if (toggle.isOn)
        {
            //toggle.SetIsOnWithoutNotify(false);
            toggleBG.sprite = toggleCheckedBG;
            toggleBG.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            //toggle.SetIsOnWithoutNotify(true);
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
