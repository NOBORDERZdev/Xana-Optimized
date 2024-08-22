using DG.Tweening;
using SuperStar.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EmoteFilterManager : MonoBehaviour
{
    public List<Button> EmotesObjects = new List<Button>();
    public Button GestureBtn;
    public Button PoseBtn;
    public GameObject popUpPenal;
    private GameObject animObject;
    public GameObject ContentPanel;
    public GameObject ListItemPrefab;
    public GameObject emoteAnimationHighlightButton;
    public string animationTabName;
    public string animationTabNameLang;
    private Sprite sprite;
    public bool taboneclick = false;
    public bool tabtwoclick = false;
    public GameObject NoDataFound;
    public GameObject ScrollViewAnimation;
    public int Counter = 0;
    public List<Button> buttonList = new List<Button>();
    private int CounterValue=0;
    private bool onceforapi = false;
    public GameObject progressbar;
    private bool valueget=false;
    public static bool TouchDisable = false;
    public  AnimationDetails bean;
    public Color[] emoteBGs;

    public GameObject JyosticksObject;
    public GameObject BottomObject;
   
    public GameObject JumpObject;
    private void OnEnable()
    {
        ResetHighligt();
        if(GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OnEmoteSelectionClose += OnEmoteSelectionClose;
    }

    public void OnDisable()
    {
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OnEmoteSelectionClose -= OnEmoteSelectionClose;
    }

    void Start()
    {
        animationTabName = "Dance";
        animationTabNameLang = "Dance";
        getLocalStorageEmote();
    }
    public void GusterBtnClick()
    {
        Debug.Log("local manage====");

        taboneclick = true;
        tabtwoclick = false;
        NoDataFound.SetActive(false);
        ScrollViewAnimation.SetActive(true);
        animationTabName = GestureBtn.GetComponent<Text>().text;
        animationTabNameLang = GestureBtn.transform.name;
        GestureBtn.GetComponent<Text>().color = Color.black;
        PoseBtn.GetComponent<Text>().color = new Color32(142, 142, 142, 255);
        callobjects();
    }
    public void GusterBtnClick(string tabname)
    {
        Debug.Log("local manage====");

        taboneclick = true;
        tabtwoclick = false;
        NoDataFound.SetActive(false);
        ScrollViewAnimation.SetActive(true);
        animationTabName = GestureBtn.GetComponent<Text>().text;
        animationTabNameLang = GestureBtn.transform.name;
        GestureBtn.GetComponent<Text>().color = Color.black;
        PoseBtn.GetComponent<Text>().color = new Color32(142, 142, 142, 255);
        callobjects();
    }
    public void EmotesTabClick(Button tabname)
    {
        foreach (Button objects in EmotesObjects)
        {
            if (!objects.transform.name.Equals(tabname.transform.name))
            {
                objects.GetComponent<Text>().color = new Color32(142, 142, 142, 255);
                objects.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        Debug.Log("local manage====");

        NoDataFound.SetActive(false);
        ScrollViewAnimation.SetActive(true);
        animationTabName = tabname.GetComponent<Text>().text;
        animationTabNameLang = tabname.transform.name;
        tabname.GetComponent<Text>().color = new Color32(0, 143, 255, 255);
        tabname.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        callobjects();
    }



    public void PoseBtnClick()
    {
        tabtwoclick = true;
        taboneclick = false;
        NoDataFound.SetActive(false);
        ScrollViewAnimation.SetActive(true);
        animationTabName = PoseBtn.GetComponent<Text>().text;
        animationTabNameLang = PoseBtn.transform.name;
        PoseBtn.GetComponent<Text>().color = Color.black;
        GestureBtn.GetComponent<Text>().color = new Color32(142, 142, 142, 255);
        callobjects();
    }

    public void PopupTextClik(Button TextBtn)
    {
        {
            if (taboneclick)
            {
                GestureBtn.GetComponent<Text>().text = TextBtn.gameObject.transform.GetChild(0).GetComponent<Text>().text;
                GestureBtn.transform.name = TextBtn.gameObject.transform.name;
            }
            else if (tabtwoclick)
            {
                PoseBtn.GetComponent<Text>().text = TextBtn.gameObject.transform.GetChild(0).GetComponent<Text>().text;
                PoseBtn.transform.name = TextBtn.gameObject.transform.name;
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

    public void seeAllClick()
    {
        if (!tabtwoclick)
        {
            tabtwoclick = true;
        }

        popUpPenal.SetActive(true);

        for(int i = 0; i < buttonList.Count; i++)
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
    private void HideIfClickedOutside(GameObject panel)
    {
        if (Input.GetMouseButton(0) && panel.activeSelf &&
            !RectTransformUtility.RectangleContainsScreenPoint(
                panel.GetComponent<RectTransform>(),
                Input.mousePosition,
                Camera.main))
        {
            panel.SetActive(false);
        }
    }
    public void getLocalStorageEmote()
    {
        Debug.Log("Response Emote===" + EmoteAnimationHandler.Instance.emoteAnim.Count);
            if (EmoteAnimationHandler.Instance.emoteAnim.Count > 0)
            {
                for (int i = 0; i < EmoteAnimationHandler.Instance.emoteAnim.Count; i++)
            {

                valueget = true;

                        animObject = Instantiate(ListItemPrefab);
                        animObject.transform.SetParent(ContentPanel.transform);
                        animObject.transform.localPosition = Vector3.zero;
                        animObject.transform.localScale = Vector3.one;
                        animObject.transform.localRotation = Quaternion.identity;
                        if (EmoteAnimationHandler.Instance.emoteAnim[i].name.Contains("React")){
                            EmoteAnimationHandler.Instance.emoteAnim[i].name = EmoteAnimationHandler.Instance.emoteAnim[i].name.Replace("React", "Reaction");
                        }
                        animObject.transform.name = EmoteAnimationHandler.Instance.emoteAnim[i].name;
                        animObject.transform.GetChild(1).gameObject.SetActive(false);
                int i1 = i;
              
                Image ima = animObject.transform.GetChild(0).gameObject.GetComponent<Image>();
                AssetCache.Instance.EnqueueOneResAndWait(EmoteAnimationHandler.Instance.emoteAnim[i1].thumbnail.ToString(), EmoteAnimationHandler.Instance.emoteAnim[i1].thumbnail.ToString(), (success) =>
                {
                 
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(ima, EmoteAnimationHandler.Instance.emoteAnim[i1].thumbnail, changeAspectRatio: true);
                        progressbar.SetActive(false);
                    }
                    else
                    {
                        Debug.Log("Download Failed");
                    }
                });
                        LoadButtonClick LBC = animObject.GetComponent<LoadButtonClick>();
                        if (LBC == null)
                        {
                            LBC = animObject.AddComponent<LoadButtonClick>();
                        }
#if UNITY_ANDROID

                        LBC.Initializ(EmoteAnimationHandler.Instance.emoteAnim[i1].android_file, EmoteAnimationHandler.Instance.emoteAnim[i1].name, this, ContentPanel.gameObject, EmoteAnimationHandler.Instance.emoteAnim[i1].thumbnail);
#elif UNITY_IOS
                                LBC.Initializ(EmoteAnimationHandler.Instance.emoteAnim[i1].ios_file, EmoteAnimationHandler.Instance.emoteAnim[i1].name, this, ContentPanel.gameObject, EmoteAnimationHandler.Instance.emoteAnim[i1].thumbnail);

#endif 
            }

            callobjects();
        }
            else
            {
                NoDataFound.SetActive(true);
                ScrollViewAnimation.SetActive(false);
            }
    }

    private void OnEmoteSelectionClose()
    {
        closeWindow(gameObject);
    }

    public void closeWindow(GameObject panel)
    {
        if (ScreenOrientationManager._instance.isPotrait)
        {
            BottomObject.SetActive(true);
            

            if (panel == this.gameObject)
                panel.transform.DOLocalMoveY(-1500f, 0.1f);

            JyosticksObject.transform.DOLocalMoveY(ScreenOrientationManager._instance.joystickInitPosY, 0.1f);
            JumpObject.transform.DOLocalMoveY(ScreenOrientationManager._instance.joystickInitPosY, 0.1f);
            
               BuilderEventManager.ChangeNinja_ThrowUIPosition?.Invoke(225, true);
        }
        else
        {
            BuilderEventManager.ChangeNinja_ThrowUIPosition?.Invoke(165,false);
        }
        StartCoroutine(DelayToClose(panel));
        EmoteAnimationHandler.Instance.isEmoteActive = false;
        LoadEmoteAnimations.animClick = false;
        if (animObject.transform.GetChild(3).gameObject.activeInHierarchy)
        {
            animObject.transform.GetChild(3).gameObject.SetActive(false);
        }
        if (EmoteAnimationHandler.Instance.AnimObject == null /*|| EmoteAnimationHandler.Instance.currentAnimationTab == "Sit & lying"*/)
        {
            emoteAnimationHighlightButton.SetActive(false);
            if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.AllAnimationsPanelUpdate(false);

        }
       
     
    }

    IEnumerator DelayToClose(GameObject g)
    {
        yield return new WaitForSeconds(.3f);
        g.SetActive(false);
    }
    public void ResetHighligt()
    {
        if (ContentPanel.transform.childCount > 0)
        {
            foreach (Transform child in ContentPanel.transform)
            {
                if (child.transform.GetChild(1).gameObject.activeInHierarchy)
                {
                    child.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
    }
    private string animName = null;
    private GameObject animationObject;
    private void callobjects()
    {
        for (int i = 0; i < ContentPanel.transform.childCount; i++)
        {
            animName = ContentPanel.transform.GetChild(i).transform.name;
            animationObject = ContentPanel.transform.GetChild(i).transform.gameObject;
            if (animationTabNameLang.Equals("Dance"))
            {
                if (animName.Contains("Full") || animName.Contains("Jazz") || animName.Contains("Foot"))
                {
                    animationObject.SetActive(true);
                    NoDataFound.SetActive(false);
                }
                else
                {
                    animationObject.SetActive(false);
                }
            }
            else if (animationTabNameLang.Equals("Sit & lying"))
            {
                if (animName.Contains("Laydown") || animName.Contains("Sit"))
                {
                    animationObject.SetActive(true);
                    NoDataFound.SetActive(false);
                }
                else
                {
                    animationObject.SetActive(false);
                }
            }
            else if (animationTabNameLang.Equals("Idle"))
            {
                if (animName.Contains("Idle"))
                {
                    animationObject.SetActive(true);
                    NoDataFound.SetActive(false);
                }
                else
                {
                    animationObject.SetActive(false);
                }
            }
            else if (animationTabNameLang.Equals("Jump"))
            {
                if (!animName.Contains("Jump"))
                {

                    animationObject.SetActive(false);
                    NoDataFound.SetActive(true);
                }

            }
            else if (animationTabNameLang.Equals("Kick"))
            {
                if (!animName.Contains("Kick"))
                {
                    animationObject.SetActive(false);
                    NoDataFound.SetActive(true);
                }

            }
            else if (animationTabNameLang.Equals("Reaction"))
            {
                if (animName.Contains("React"))
                {
                    animationObject.SetActive(true);
                    NoDataFound.SetActive(false);
                }
                else if (animName.Contains("Idle"))
                {
                    animationObject.SetActive(true);
                    NoDataFound.SetActive(false);
                }
                else
                {
                    animationObject.SetActive(false);
                }
            }
            else if (animationTabNameLang.Equals("Run"))
            {
                if (!animName.Contains("Run"))
                {
                    animationObject.SetActive(false);
                    NoDataFound.SetActive(true);
                }
            }
            else if (animationTabNameLang.Equals("Walk"))
            {
                if (animName.Contains("Walk"))
                {
                    animationObject.SetActive(true);
                    NoDataFound.SetActive(false);
                }
                else
                {
                    animationObject.SetActive(false);
                }
            }
            else if (animationTabNameLang.Equals("Moves"))
            {
                if (animName.Contains("Break") || animName.Contains("Clapping")
                    || animName.Contains("Hand") || animName.Contains("Club"))


                {
                    animationObject.SetActive(true);
                    NoDataFound.SetActive(false);
                }
                else
                {
                    animationObject.SetActive(false);
                }
            }

        }

        EmoteAnimationHandler.Instance.currentAnimationTab = animationTabNameLang;
    }
   
  
    IEnumerator LoadSpriteEnv(string ImageUrl, GameObject thumbnail, int i)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
        }
        else
        {
            if (ImageUrl.Equals(""))
            {
            }
            else
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(ImageUrl);
                www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return null;
                }
                Texture2D thumbnailTexture = DownloadHandlerTexture.GetContent(www);
                thumbnailTexture.Compress(true);
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {

                }
                else
                {
                    sprite = Sprite.Create(thumbnailTexture, new Rect(0, 0, thumbnailTexture.width, thumbnailTexture.height), new Vector2(0, 0));
                    if (thumbnail != null)
                    {
                        progressbar.SetActive(false);
                        thumbnail.GetComponent<Image>().sprite = sprite;
                    }
                }
                www.Dispose();
            }
        }
    }


    //GetAllAnimations
    [System.Serializable]
    public class AnimationList
    {
        public int id;
        public string name;
        public string group;
        public string thumbnail;
        public string android_file;
        public string ios_file;
        public string description;
        public DateTime createdAt;
        public DateTime updatedAt;
    }
    [System.Serializable]
    public class Data
    {
        public List<AnimationList> animationList;
    }
    [System.Serializable]
    public class AnimationDetails
    {
        public bool success;
        public Data data;
        public string msg;
    }
}
