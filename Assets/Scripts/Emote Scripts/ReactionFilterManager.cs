using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ReactionFilterManager : MonoBehaviour
{
    public Button GestureBtn;
    public Button EmoteBtn;
    public Button EmojiBtn;
    public GameObject popUpPenal;
    public GameObject ContentPanel;
    public GameObject ListItemPrefab;
    public bool taboneclick = false;
    public bool tabtwoclick = false;
    public List<GameObject> ReactionScrolls = new List<GameObject>();
    public GameObject NoDataFound;
    public GameObject ScrollViewAnimation;
    public int Counter = 0;
    public List<Button> buttonList = new List<Button>();
    public List<Button> EmotesObjects = new List<Button>();
    public GameObject progressbar;
    private bool valueget = false;
    public static bool TouchDisable = false;
    public Color SelectedColor;
    public Color UnSelectedColor;
    public static ReactionFilterManager Instance;


    public string animationTabName;
    public string animationTabNameLang;
    private Sprite sprite;
    private int CounterValue = 0;
    private bool onceforapi = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void OnEnable()
    {
        if (Instance != null && Instance != this)
            Instance = this;
    }
    void Start()
    {
        tabtwoclick = false;
        taboneclick = true;
    }


    public void OnClickReactionTab(int tab)
    {
        for (int i = 0; i < ReactionScrolls.Count; i++)
        {
            buttonList[i].transform.GetChild(0).GetComponent<Text>().color = UnSelectedColor;
            ReactionScrolls[i].SetActive(false);
        }
        buttonList[tab].transform.GetChild(0).GetComponent<Text>().color = SelectedColor;
        ReactionScrolls[tab].SetActive(true);
    }
    void Update()
    {

    }

    public void HideReactionPanel()
    {
        gameObject.SetActive(false);
        ReactScreen.Instance.HideReactionScreen();
        //EmojiBtn.image.sprite = ReactScreen.Instance.react_disable;
    }

    public void seeAllClick()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
        if (!tabtwoclick)
        {
            tabtwoclick = true;
        }

        popUpPenal.SetActive(true);

        for (int i = 0; i < buttonList.Count; i++)
        {
            if (animationTabName.Equals(buttonList[i].gameObject.transform.GetChild(0).GetComponent<Text>().text))
            {

                buttonList[i].gameObject.transform.GetChild(0).GetComponent<Text>().color = Color.black;
                valueget = false;
            }
            else
            {
                buttonList[i].gameObject.transform.GetChild(0).GetComponent<Text>().color = new Color32(142, 142, 142, 255);
            }
        }

    }
    public void PopupTextClik(Button TextBtn)
    {
        // EmoteAnimationHandler.Instance.alreadyRuning = true;
        //Caching.ClearCache();
        Debug.Log("text btn====" + TextBtn.gameObject.transform.GetChild(0).GetComponent<Text>().text + "GestureBtn====" + GestureBtn.GetComponent<Text>().text);
        Debug.Log("text btn2====" + TextBtn.gameObject.transform.GetChild(0).GetComponent<Text>().text + "GestureBtn 2====" + EmoteBtn.GetComponent<Text>().text);

        // Commented By WaqasAhmad
        //if (!GestureBtn.GetComponent<Text>().text.Equals(TextBtn.gameObject.transform.GetChild(0).GetComponent<Text>().text) &&
        //    !EmoteBtn.GetComponent<Text>().text.Equals(TextBtn.gameObject.transform.GetChild(0).GetComponent<Text>().text))
        {
            if (taboneclick)
            {
                GestureBtn.GetComponent<Text>().text = TextBtn.gameObject.transform.GetChild(0).GetComponent<Text>().text;
                GestureBtn.transform.name = TextBtn.gameObject.transform.name;
            }
            else if (tabtwoclick)
            {
                EmoteBtn.GetComponent<Text>().text = TextBtn.gameObject.transform.GetChild(0).GetComponent<Text>().text;
                EmoteBtn.transform.name = TextBtn.gameObject.transform.name;
            }
            popUpPenal.SetActive(false);
            TextBtn.gameObject.transform.GetChild(0).GetComponent<Text>().color = Color.black;
            animationTabName = TextBtn.gameObject.transform.GetChild(0).GetComponent<Text>().text;
            animationTabNameLang = TextBtn.gameObject.transform.name;

            NoDataFound.SetActive(false);
            ScrollViewAnimation.SetActive(true);
            callobjects();
        }
    }

    public void GusterBtnClick()
    {
        Debug.Log("local manage====");
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
        //EmoteAnimationHandler.Instance.alreadyRuning = true;

        taboneclick = true;
        tabtwoclick = false;
        NoDataFound.SetActive(false);
        ScrollViewAnimation.SetActive(true);
        animationTabName = GestureBtn.GetComponent<Text>().text;
        animationTabNameLang = GestureBtn.transform.name;
        GestureBtn.GetComponent<Text>().color = Color.black;
        EmoteBtn.GetComponent<Text>().color = new Color32(142, 142, 142, 255);
        callobjects();
    }

    public void EmoteBtnClick()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
        tabtwoclick = true;
        taboneclick = false;
        NoDataFound.SetActive(false);
        ScrollViewAnimation.SetActive(true);
        animationTabName = EmoteBtn.GetComponent<Text>().text;
        animationTabNameLang = EmoteBtn.transform.name;
        EmoteBtn.GetComponent<Text>().color = Color.black;
        GestureBtn.GetComponent<Text>().color = new Color32(142, 142, 142, 255);
        callobjects();
    }

    public void EmoteBtnClickPotrait(Button tabname)
    {
        foreach (Button objects in EmotesObjects)
        {
           // Debug.Log("buttonlist value==="+buttonList.Count);
            if (!objects.transform.name.Equals(tabname.transform.name))
            {
               // Debug.Log("Objects mila===="+ objects.transform.GetChild(0).GetComponent<Text>());
                objects.GetComponent<Text>().color = new Color32(142, 142, 142, 255);
                objects.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
        //tabtwoclick = true;
        //taboneclick = false;
        NoDataFound.SetActive(false);
        ScrollViewAnimation.SetActive(true);
        animationTabName = tabname.GetComponent<Text>().text;
        animationTabNameLang = tabname.transform.name;
        tabname.GetComponent<Text>().color = new Color32(0, 143, 255, 255);
        tabname.gameObject.transform.GetChild(0).gameObject.SetActive(true);
       
        callobjects();
    }
    private void callobjects()
    {
        for (int i = 0; i < ContentPanel.transform.childCount; i++)
        {

            if (animationTabNameLang.Equals("Emotes"))
            {
                if (ReactionScrolls[i].transform.name.Contains("Emotes"))
                {
                    ReactionScrolls[i].SetActive(true);
                    NoDataFound.SetActive(false);
                }
                else
                {
                    ReactionScrolls[i].SetActive(false);
                }
            }
            else if (animationTabNameLang.Equals("Gesture"))
            {
                if (ReactionScrolls[i].transform.name.Contains("Gesture"))
                {
                    ReactionScrolls[i].SetActive(true);
                    NoDataFound.SetActive(false);
                }
                else
                {
                    ReactionScrolls[i].SetActive(false);
                }
            }
            else if (animationTabNameLang.Equals("Other"))
            {
                if (ReactionScrolls[i].transform.name.Contains("Other"))
                {
                    ReactionScrolls[i].SetActive(true);
                    NoDataFound.SetActive(false);
                }
                else
                {
                    ReactionScrolls[i].SetActive(false);
                }
            }
        }
    }
}
