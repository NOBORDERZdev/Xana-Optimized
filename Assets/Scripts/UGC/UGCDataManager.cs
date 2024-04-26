
using SuperStar.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;


public class UGCDataManager : MonoBehaviour
{
    public UGCUIManager ugcUIManager;   
    void Start()
    {

    }
    public IEnumerator GetBgKey()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
        ugcUIManager.loadingTexture.SetActive(true);
        using (UnityWebRequest www = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.UGCGetBackground))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);

            www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("Response===" + data);
                BgKey _info = JsonUtility.FromJson<BgKey>(data);
                if (!_info.Equals("") || !_info.Equals(null))
                {
                    if (_info.data.isBackgroundApplied == "true")
                    {
                       StartCoroutine(DownloadBgAddressableTexture(_info.data.backgroundInfo));
                        ugcUIManager.bgDefaultTextureKey = _info.data.backgroundInfo;
                        ugcUIManager.currentKeyInfo = _info.data;
                    }
                    else
                    {
                        ugcUIManager.ApplyDefaultTexture();
                        ugcUIManager.loadingTexture.SetActive(false);
                    }
                }
                else
                {
                    ugcUIManager.ApplyDefaultTexture();
                    ugcUIManager.loadingTexture.SetActive(false);
                }
            }
        }
    }
    public IEnumerator PostBgKey(KeyInfo _keyInfo)
    {
        WWWForm form = new WWWForm();
        form.AddField("backgroundInfo", _keyInfo.backgroundInfo);
        form.AddField("isBackgroundApplied", _keyInfo.isBackgroundApplied);

        using (UnityWebRequest www = UnityWebRequest.Post(ConstantsGod.API_BASEURL + ConstantsGod.UGCAddBackground, form))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);

            www.SendWebRequest();

            while (!www.isDone)
            {
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                ugcUIManager.bgDefaultTextureKey = _keyInfo.backgroundInfo;
                Debug.Log("Successfully Post Data");
            }
        }
    }
    public IEnumerator DownloadBgAddressableTexture(string key)
    {
        if (key != "" && Application.internetReachability != NetworkReachability.NotReachable)
        {
            AsyncOperationHandle loadOp;

            bool flag = false;
            loadOp = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(key, ref flag);
            if (!flag)
                loadOp = Addressables.LoadAssetAsync<Texture>(key);

            while (!loadOp.IsDone)
            {
                yield return null;
            }

            if (loadOp.Status == AsyncOperationStatus.Failed)
            {
                ugcUIManager.loadingTexture.SetActive(false);
                ugcUIManager.ApplyDefaultTexture();
                yield break;
            }
            else if (loadOp.Status == AsyncOperationStatus.Succeeded)
            {
                AddressableDownloader.Instance.MemoryManager.AddToReferenceList(loadOp, key);
                ugcUIManager.ApplyBgTexture(loadOp.Result as Texture, key, "true");
                ugcUIManager.loadingTexture.SetActive(false);
            }
        }
    }
    private bool alreadyInstantiated = false;
    public void GetAllBackGroundCategory()
    {
        if (!alreadyInstantiated)
            StartCoroutine(GetBgIcons());
        else
            ugcUIManager.loadingTexture.SetActive(false);
    }
    IEnumerator GetBgIcons()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
        UnityWebRequest uwr = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.BACKGROUNDFILES);
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
            BackroundInfos _info = JsonUtility.FromJson<BackroundInfos>(uwr.downloadHandler.text.ToString());
            if (!_info.Equals("") || !_info.Equals(null))
            {
                Dictionary<string, List<GameObject>> categorizedBackgrounds = new Dictionary<string, List<GameObject>>();
                string initialCategory = "";
                for (int i = 0; i < _info.data.backgroundList.Count; i++)
                {
                    GameObject animObject;
                    animObject = Instantiate(ugcUIManager.ItemPrefab, ugcUIManager.bgPrefabParent.transform);
                    //animObject.transform.localPosition = Vector3.zero;
                    //animObject.transform.localScale = Vector3.one;
                    //animObject.transform.localRotation = Quaternion.identity;
                    //animObject.transform.GetChild(1).gameObject.SetActive(false);
                    BgTagsView temp = animObject.GetComponent<BgTagsView>();
                    animObject.name = _info.data.backgroundList[i].name;
                    temp.icon.sprite = null;
                    temp.InitBg(_info.data.backgroundList[i]);
                    string url = _info.data.backgroundList[i].thumbnail;
                    if (url != "")
                    {
                        AssetCache.Instance.EnqueueOneResAndWait(url, url, (success) =>
                        {
                            if (success)
                            {
                                AssetCache.Instance.LoadSpriteIntoImage(temp.icon, url, changeAspectRatio: true);
                            }
                            else
                            {
                                Debug.Log("Download Failed");
                            }
                        });
                    }
                    animObject.GetComponent<Button>().onClick.AddListener(() => ugcUIManager.OnClickSelectBackgroundButton(temp, animObject.name));
                    ugcUIManager.tagsObjects.Add(temp);

                    // Get the category of the current background
                    string category = _info.data.backgroundList[i].category;
                    if (string.IsNullOrEmpty(initialCategory))
                    {
                        initialCategory = category;
                    }
                    if (category == initialCategory)
                    {
                        animObject.SetActive(true);
                    }
                    else
                    {
                        animObject.SetActive(false);
                    }
                    // If the category doesn't exist in the dictionary, create a new list for it
                    if (!categorizedBackgrounds.ContainsKey(category))
                    {
                        categorizedBackgrounds[category] = new List<GameObject>();
                        GameObject tags = Instantiate(ugcUIManager.tagsPrefab, ugcUIManager.tagsPrefabParent);
                        tags.name = category;
                        tags.GetComponent<BgTagsView>().InitBgTags(_info.data.backgroundList[i]);
                        ugcUIManager.tagsbuttons.Add(tags);
                        //tags.GetComponent<Button>().onClick.AddListener(() => ugcUIManager.OnClickTags(tags, category));
                    }
                    // Add the instantiated background object to the corresponding category list
                    categorizedBackgrounds[category].Add(animObject);
                }
                alreadyInstantiated = true;
                ugcUIManager.loadingTexture.SetActive(false);
            }
            else
            {
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
public class BgData
{
    public List<BackgroundList> backgroundList;
}
[System.Serializable]
public class BackroundInfos
{
    public bool success;
    public BgData data;
    public string msg;
}
[System.Serializable]
public class BgKey
{
    public bool success;
    public KeyInfo data;
    public string msg;
}
[System.Serializable]
public class KeyInfo
{
    public string isBackgroundApplied;
    public string backgroundInfo;
}