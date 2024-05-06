using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class FeedAnimationBGChange : MonoBehaviour
{
    public GameObject ContentPanel;
    public GameObject ListItemPrefab;
    public GameObject contentCategoryPanel;
    public GameObject categoryPrefab;

    public GameObject pickImageObject;
    public GameObject noneObject;
    public List<GameObject> GameObjectBG = new List<GameObject>();
    public List<Button> buttonList = new List<Button>();

    public List<string> animationGroup = new List<string>();
    // Start is called before the first frame update
    private bool alreadyInstantiated = false;

    public static FeedAnimationBGChange instance;

    void Start()
    {
        instance = this;

        //StartCoroutine(GetAllBG());
    }

    // Update is called once per frame
    public void SetCategoryButtonHighLight(int index)
    {
        for (int i = 0; i < buttonList.Count; i++)
        {
            if (index == i)
            {
                // if (buttonList[i].transform.GetChild(0).GetChild(1))
                buttonList[i].transform.GetChild(0).GetChild(buttonList[i].transform.GetChild(0).childCount - 1).gameObject.SetActive(true);
                buttonList[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
                string groupName = buttonList[i].name;
                for (int j = 0; j < ContentPanel.transform.childCount; j++)
                {
                    if (groupName == "All")
                    {
                        ContentPanel.transform.GetChild(j).gameObject.SetActive(true);
                    }
                    else
                    {
                        if (ContentPanel.transform.GetChild(j).name == groupName)
                        {
                            ContentPanel.transform.GetChild(j).gameObject.SetActive(true);
                        }
                        else
                        {
                            ContentPanel.transform.GetChild(j).gameObject.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                //if (buttonList[i].transform.GetChild(0).GetChild(1))
                buttonList[i].transform.GetChild(0).GetChild(buttonList[i].transform.GetChild(0).childCount - 1).gameObject.SetActive(false);
                buttonList[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(115, 115, 115, 255);
            }
        }
    }

    public void SetAnimationHighlight(GameObject currentAnimButton)
    {
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
        for (int i = 0; i < ContentPanel.transform.childCount; i++)
        {
            ContentPanel.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
        }
        currentAnimButton.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void GetAllBackGroundImages()
    {
        if (!alreadyInstantiated)
            StartCoroutine(GetAllBG());
    }

    IEnumerator GetAllBG()
    {
        Debug.Log("All BG Base url:" + ConstantsGod.API_BASEURL);
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
        UnityWebRequest uwr = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.BACKGROUNDFILES + "/" + APIBasepointManager.instance.apiversion);
        try
        {
            uwr.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        }
        catch (Exception e1)
        {
            Debug.Log(e1);
        }

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Response===" + uwr.downloadHandler.text.ToString());
            BackroundDetails bean = JsonUtility.FromJson<BackroundDetails>(uwr.downloadHandler.text.ToString());
            if (!bean.Equals("") || !bean.Equals(null))
            {
                Debug.Log("Count===" + bean.data.backgroundList[0]);
                for (int i = 0; i < bean.data.backgroundList.Count; i++)
                {
                    GameObject animObject;
                    animObject = Instantiate(ListItemPrefab);
                    animObject.transform.SetParent(ContentPanel.transform);
                    animObject.transform.localPosition = Vector3.zero;
                    animObject.transform.localScale = Vector3.one;
                    animObject.transform.localRotation = Quaternion.identity;
                    animObject.transform.GetChild(1).gameObject.SetActive(false);
                    animObject.name = bean.data.backgroundList[i].category;
                    animObject.transform.GetChild(0).GetComponent<Image>().sprite = null;
                    if (!animationGroup.Contains(bean.data.backgroundList[i].category))
                        animationGroup.Add(bean.data.backgroundList[i].category);

                    StartCoroutine(LoadSpriteEnv(bean.data.backgroundList[i].thumbnail, animObject.transform.GetChild(0).gameObject, i));
                    animObject.GetComponent<Button>().onClick.AddListener(() => SetAnimationHighlight(animObject));

                    string url = bean.data.backgroundList[i].thumbnail;

                    animObject.GetComponent<Button>().onClick.AddListener(() => Load(animObject));
                }

                for (int i = 0; i < animationGroup.Count; i++)
                {
                    GameObject categoryObject;
                    categoryObject = Instantiate(categoryPrefab);
                    categoryObject.transform.SetParent(contentCategoryPanel.transform);
                    categoryObject.transform.localPosition = Vector3.zero;
                    categoryObject.transform.localScale = Vector3.one;
                    categoryObject.transform.localRotation = Quaternion.identity;
                    categoryObject.name = animationGroup[i];
                    categoryObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = TextLocalization.GetLocaliseTextByKey(animationGroup[i]);
                    categoryObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(115, 115, 115, 255);
                    categoryObject.transform.GetChild(0).gameObject.SetActive(true);
                    int x = i + 1;
                    categoryObject.transform.GetComponent<Button>().onClick.AddListener(() => SetCategoryButtonHighLight(x));
                    buttonList.Add(categoryObject.GetComponent<Button>());
                }
                alreadyInstantiated = true;
            }
            else
            {
            }
        }
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
                // Loader.SetActive(false);
            }
            else
            {
                //WWW www = new WWW(ImageUrl);
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
                    Sprite sprite = Sprite.Create(thumbnailTexture, new Rect(0, 0, thumbnailTexture.width, thumbnailTexture.height), new Vector2(0, 0));
                    if (thumbnail != null)
                    {
                        thumbnail.GetComponent<Image>().sprite = sprite;
                        //Loader.SetActive(false);
                    }
                    else
                    {
                        // Loader.SetActive(false);
                    }
                }
                www.Dispose();
            }
        }
    }

    public void Load(GameObject _gameObject)
    {
        LiveVideoRoomManager.Instance.BackgroundImage.gameObject.SetActive(true);
        LiveVideoRoomManager.Instance.BackgroundImage.sprite = _gameObject.transform.GetChild(0).GetComponent<Image>().sprite;
        if (_gameObject.name == "None")
        {
            LiveVideoRoomManager.Instance.BackgroundImage.color = new Color(243f, 243f, 243f);
        }
        else
        {
            LiveVideoRoomManager.Instance.BackgroundImage.color = Color.white;
        }
    }

    public void checkBGButton(GameObject button)
    {
        if (button.gameObject.transform.name.Contains("All"))
        {
            if (!pickImageObject.activeInHierarchy && !noneObject.activeInHierarchy)
            {
                pickImageObject.SetActive(true);
                noneObject.SetActive(true);
            }
            for (int i = 0; i < GameObjectBG.Count; i++)
            {
                GameObjectBG[i].SetActive(true);
            }
        }
        else if (button.gameObject.transform.name.Contains("Hot"))
        {
            if (pickImageObject.transform.name.Equals("PickImage") && noneObject.transform.name.Equals("None"))
            {
                if (pickImageObject.activeInHierarchy && noneObject.activeInHierarchy)
                {
                    pickImageObject.SetActive(false);
                    noneObject.SetActive(false);
                }
            }

            for (int i = 0; i < GameObjectBG.Count; i++)
            {
                if (GameObjectBG[i].transform.name.Contains("HotImage"))
                {
                    GameObjectBG[i].SetActive(true);
                }
                else
                {
                    GameObjectBG[i].SetActive(false);
                }
            }
        }
        else if (button.gameObject.transform.name.Contains("Space"))
        {
            if (pickImageObject.transform.name.Equals("PickImage") && noneObject.transform.name.Equals("None"))
            {
                if (pickImageObject.activeInHierarchy && noneObject.activeInHierarchy)
                {
                    pickImageObject.SetActive(false);
                    noneObject.SetActive(false);
                }
                for (int i = 0; i < GameObjectBG.Count; i++)
                {
                    if (GameObjectBG[i].transform.name.Contains("SpaceImage"))
                    {
                        GameObjectBG[i].SetActive(true);
                    }
                    else
                    {
                        GameObjectBG[i].SetActive(false);
                    }
                }
            }
        }
    }
    [System.Serializable]
    public class BackgroundList
    {
        public int id;
        public string name;
        public string category;
        public string thumbnail;
        public DateTime createdAt;
        public DateTime updatedAt;
    }
    [System.Serializable]
    public class BGData
    {
        public List<BackgroundList> backgroundList;
    }
    [System.Serializable]
    public class BackroundDetails
    {
        public bool success;
        public BGData data;
        public string msg;
    }
}