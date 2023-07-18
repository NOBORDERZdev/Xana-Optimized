using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class ReactDataClass
{
    public int id;
    public string iconUrl;
}

public class ReactScreen : MonoBehaviour
{


    public GameObject reactionScreenParent;
    public GameObject emoteAnimationScreenParent;
    public Transform emoteParent;
    public Transform exPressionparent;
    public Transform othersparent;
    public GameObject reactPrefab;

    public GameObject emoteAnimationHighlightButton;

    public Image reactImage;
    public Sprite react_disable;
    public Sprite react_enable;

    public List<ReactEmote> reactDataClass = new List<ReactEmote>();
    public List<ReactGestures> reactDataClassGestures = new List<ReactGestures>();
    public List<ReactOthers> reactDataClassOthers = new List<ReactOthers>();

    public bool isOpen = false;


    //Hardik changes for animation panel
    public GameObject jyostickBtn;
    public GameObject jumpBtn;
    public GameObject BottomBtnParent;
    public GameObject XanaChatObject;

    //end hardik
    //private Canvas reactScreenCanvas;//riken
    //private GraphicRaycaster graphicRaycaster;//riken


    public static ReactScreen Instance;
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
    public void OpenPanel()
    {
        Debug.Log("check value of reaction panel===" + isOpen);
        Debug.Log("check value of reaction panel 1===" + reactionScreenParent.activeInHierarchy);
        EmoteAnimationPlay.Instance.isEmoteActive = false;
        if (isOpen || reactionScreenParent.activeInHierarchy)
        {
            reactImage.sprite = react_disable;
            if (!CanvasButtonsHandler.inst.actionsContainer.activeInHierarchy)
            {
                ClosePanel();
                HideReactionScreen();
                isOpen = false;
            }
            else
            {
                reactionScreenParent.SetActive(false);
                HideReactionScreen();
                if (ChangeOrientation_waqas._instance.isPotrait)
                {
                    ChangeOrientation_waqas._instance.joystickInitPosY = jyostickBtn.transform.localPosition.y;
                    //if (ChangeOrientation_waqas._instance.isPotrait)
                    //    ChangeOrientation_waqas._instance.joystickInitPosY = jyostickBtn.transform.localPosition.y;
                    //  ReferrencesForDynamicMuseum.instance.RotateBtn.interactable = false;

                    jyostickBtn.transform.DOLocalMoveY(-50f, 0.1f);
                    jumpBtn.transform.DOLocalMoveY(-30f, 0.1f);
                    reactionScreenParent.transform.DOLocalMoveY(-108f, 0.1f);
                    BottomBtnParent.SetActive(false);
                    //  XanaChatObject.SetActive(false);
                    // ReferrencesForDynamicMuseum.instance.RotateBtn.interactable = true;
                }
            }

        }
        else
        {
            if (!PremiumUsersDetails.Instance.CheckSpecificItem("chat_reaction"))
            {
                print("Please Upgrade to Premium account");
                return;
            }
            else
            {
                print("Horayyy you have Access");
            }

            //if (!CanvasButtonsHandler.inst.actionsContainer.activeInHierarchy)
            //{
            //    reactionScreenParent.SetActive(true);
            //    //if (Input.deviceOrientation == DeviceOrientation.Portrait)
            //    //{

            if (ChangeOrientation_waqas._instance.isPotrait)
            {
                ChangeOrientation_waqas._instance.joystickInitPosY = jyostickBtn.transform.localPosition.y;
                //if (ChangeOrientation_waqas._instance.isPotrait)
                //    ChangeOrientation_waqas._instance.joystickInitPosY = jyostickBtn.transform.localPosition.y;
                //  ReferrencesForDynamicMuseum.instance.RotateBtn.interactable = false;
                reactionScreenParent.SetActive(true);
                jyostickBtn.transform.DOLocalMoveY(-50f, 0.1f);
                jumpBtn.transform.DOLocalMoveY(-30f, 0.1f);
                reactionScreenParent.transform.DOLocalMoveY(-108f, 0.1f);
                BottomBtnParent.SetActive(false);
                // XanaChatObject.SetActive(false);
                // ReferrencesForDynamicMuseum.instance.RotateBtn.interactable = true;
                BuilderEventManager.ChangeNinja_ThrowUIPosition?.Invoke(-225, true);
                CheckForInstantiation();
                reactImage.sprite = react_enable;
                isOpen = true;
                HideEmoteScreen();
            }
            //    }
            //}
            else
            {
                reactionScreenParent.SetActive(true);
                CheckForInstantiation();
                reactImage.sprite = react_enable;
                isOpen = true;
                HideEmoteScreen();
                BuilderEventManager.ChangeNinja_ThrowUIPosition?.Invoke(-165, false);
            }

        }

    }

    public void ClosePanel()
    {
        if (ChangeOrientation_waqas._instance.isPotrait)
        {
            // ReferrencesForDynamicMuseum.instance.RotateBtn.interactable = false;
            BottomBtnParent.SetActive(true);
            // XanaChatObject.SetActive(true);
            reactionScreenParent.transform.DOLocalMoveY(-1500f, 0.1f);
            jyostickBtn.transform.DOLocalMoveY(ChangeOrientation_waqas._instance.joystickInitPosY, 0.1f);
            jumpBtn.transform.DOLocalMoveY(ChangeOrientation_waqas._instance.joystickInitPosY, 0.1f);
            //ReferrencesForDynamicMuseum.instance.RotateBtn.interactable = true;
        }
        reactionScreenParent.SetActive(false);
    }
    public void HideEmoteScreen()
    {
        if (!EmoteAnimationPlay.Instance.isAnimRunning)
            emoteAnimationHighlightButton.SetActive(false);
        emoteAnimationScreenParent.SetActive(false);
    }
    public void HideReactionScreen()
    {
        isOpen = false;
        reactImage.sprite = react_disable;
        if (ChangeOrientation_waqas._instance.isPotrait)
        {
            //  ReferrencesForDynamicMuseum.instance.RotateBtn.interactable = false;
            BottomBtnParent.SetActive(true);
            //  XanaChatObject.SetActive(true);
            reactionScreenParent.transform.DOLocalMoveY(-1500f, 0.1f);
            emoteAnimationScreenParent.transform.DOLocalMoveY(-1500f, 0.1f);
            jyostickBtn.transform.DOLocalMoveY(ChangeOrientation_waqas._instance.joystickInitPosY, 0.1f);
            jumpBtn.transform.DOLocalMoveY(ChangeOrientation_waqas._instance.joystickInitPosY, 0.1f);
            // ReferrencesForDynamicMuseum.instance.RotateBtn.interactable = true;
            BuilderEventManager.ChangeNinja_ThrowUIPosition?.Invoke(225, true);
        }
        else
        {
            BuilderEventManager.ChangeNinja_ThrowUIPosition?.Invoke(165, false);
        }
        reactionScreenParent.SetActive(false);
    }



    public void ReactButtonClick()
    {


        //isOpen = !isOpen;
        //if (isOpen)
        //{
        //    reactImage.sprite = react_enable;
        //    Reaction_EmotePanel.Instance.ReactionOn();
        //    reactScreen.SetActive(true);
        //    CreatePrefab();
        //}
        //else
        //{
        //    reactImage.sprite = react_disable;
        //    reactScreen.SetActive(false);
        //}

        //if (isFromFevButton)
        //{
        //    if (isOpen && reactScreenCanvas == null)
        //    {
        //        reactScreenCanvas = reactScreen.AddComponent<Canvas>();
        //        reactScreenCanvas.overrideSorting = true;
        //        reactScreenCanvas.sortingOrder = 6;
        //        graphicRaycaster = reactScreen.AddComponent<GraphicRaycaster>();
        //    }
        //    else
        //    {
        //        Destroy(graphicRaycaster);
        //        Destroy(reactScreenCanvas);
        //        graphicRaycaster = null;
        //        reactScreenCanvas = null;
        //    }
        //}
    }

    //this method is used to show emote panel when user click on favorite button.......rik 
    public void OnShowEmotePanelFromFavorite()
    {
        //if (isOpen)
        //{
        //    if (reactScreenCanvas == null)
        //    {
        //        reactScreenCanvas = reactionScreenParent.AddComponent<Canvas>();
        //        graphicRaycaster = reactionScreenParent.AddComponent<GraphicRaycaster>();
        //        reactScreenCanvas.overrideSorting = true;
        //        reactScreenCanvas.sortingOrder = 6;
        //    }
        //}
        //else
        //{
        //    ReactButtonClick();
        //}
    }

    private void Start()
    {
        reactDataClass.Clear();
        reactDataClassGestures.Clear();
        reactDataClassOthers.Clear();
    }
    public void CheckForInstantiation()
    {
        if (reactDataClass.Count == 0 || reactDataClassGestures.Count == 0 || reactDataClassOthers.Count == 0)
        {
            ClearParent();
            StartCoroutine(getAllReactions());
        }
    }

    private void ClearParent()
    {
        foreach (Transform child in emoteParent)
        {
            Destroy(child.gameObject);
        }
    }

    public IEnumerator getAllReactions()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
        UnityWebRequest uwr = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.GetAllReactions + "/" + APIBaseUrlChange.instance.apiversion);
        try
        {
            if (UserRegisterationManager.instance.LoggedInAsGuest)
            {
                uwr.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            }
            else
            {
                uwr.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            }
        }
        catch (Exception e1)
        {
        }

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            try
            {
                ReactionDetails bean = JsonUtility.FromJson<ReactionDetails>(uwr.downloadHandler.text.ToString().Trim());
                Debug.Log("ReactionClass : " + uwr.downloadHandler.text.ToString().Trim());
                if (bean.success)
                {
                    reactDataClass.Clear();
                    reactDataClassGestures.Clear();
                    reactDataClassOthers.Clear();

                    for (int i = 0; i < bean.data.reactionList.Count; i++)
                    {
                        if (bean.data.reactionList[i].group.Equals("Emote"))
                        {

                            ReactEmote bean1 = new ReactEmote();
                            bean1.thumb = bean.data.reactionList[i].thumbnail;
                            bean1.mainImage = bean.data.reactionList[i].icon3d;
                            bean1.imageName = bean.data.reactionList[i].name;
                            reactDataClass.Add(bean1);
                        }
                        else if (bean.data.reactionList[i].group.Equals("Gestures"))
                        {
                            ReactGestures bean1 = new ReactGestures();
                            bean1.thumb = bean.data.reactionList[i].thumbnail;
                            bean1.mainImage = bean.data.reactionList[i].icon3d;
                            bean1.imageName = bean.data.reactionList[i].name;
                            reactDataClassGestures.Add(bean1);
                        }
                        else if (bean.data.reactionList[i].group.Equals("Others"))
                        {
                            ReactOthers bean1 = new ReactOthers();
                            bean1.thumb = bean.data.reactionList[i].thumbnail;
                            bean1.mainImage = bean.data.reactionList[i].icon3d;
                            bean1.imageName = bean.data.reactionList[i].name;
                            reactDataClassOthers.Add(bean1);
                        }
                    }
                }

                for (int i = 0; i < reactDataClass.Count; i++)
                {
                    GameObject newItem = Instantiate(reactPrefab, Vector3.zero, Quaternion.identity, emoteParent);
                    newItem.GetComponent<ReactItem>().SetData(reactDataClass[i].thumb + "?width=50&height=50", reactDataClass[i].mainImage, i, reactDataClass[i].imageName);
                }

                for (int j = 0; j < reactDataClassGestures.Count; j++)
                {
                    GameObject newItem = Instantiate(reactPrefab, Vector3.zero, Quaternion.identity, exPressionparent);
                    newItem.GetComponent<ReactItem>().SetData(reactDataClassGestures[j].thumb + "?width=50&height=50", reactDataClassGestures[j].mainImage, j, reactDataClassGestures[j].imageName);
                }
                for (int j = 0; j < reactDataClassOthers.Count; j++)
                {
                    GameObject newItem = Instantiate(reactPrefab, Vector3.zero, Quaternion.identity, othersparent);
                    newItem.GetComponent<ReactItem>().SetData(reactDataClassOthers[j].thumb + "?width=50&height=50", reactDataClassOthers[j].mainImage, j, reactDataClassOthers[j].imageName);
                }
            }
            catch
            {

            }
        }
    }
    #region DATA

    public class ReactEmote
    {
        public string imageName;
        public string thumb;
        public string mainImage;
    }

    public class ReactGestures
    {
        public string imageName;
        public string thumb;
        public string mainImage;
    }
    public class ReactOthers
    {
        public string imageName;
        public string thumb;
        public string mainImage;
    }

    public class ReactionList
    {
        public int id;
        public string name;
        public object android_bundle;
        public object ios_bundle;
        public string thumbnail;
        public int version;
        public string group;
        public string icon3d;
        public DateTime createdAt;
        public DateTime updatedAt;
    }

    public class Data
    {
        public List<ReactionList> reactionList;
    }

    public class ReactionDetails
    {
        public bool success;
        public Data data;
        public string msg;
    }
    #endregion

}
