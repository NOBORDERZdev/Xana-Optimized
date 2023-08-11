using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using XanaAi;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class PhotonAIController : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public int totalAISpawn = 0;

    [SerializeField] float aiStartDelay = 2;
    [SerializeField] int AiCountToSpwaned;
    [SerializeField] List<string> aiNames;
    [SerializeField] List<Transform> SpwanPoints;
    private AiAppearance apperance;
    private int rand = 0;

    private void Awake()
    {
        apperance = GetComponent<AiAppearance>();
        ReferrencesForDynamicMuseum.instance.pai = this;
    }

    IEnumerator Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            yield return new WaitForSeconds(aiStartDelay);
            StartCoroutine(SpawnAI());
        }
    }

    IEnumerator SpawnAI()
    {
        Debug.LogError("I am master client");
        ReactScreen.Instance.StartCoroutine(ReactScreen.Instance.getAllReactions());
        for (int i = 0; i < AiCountToSpwaned; i++)
        {
            rand = Random.Range(0, SpwanPoints.Count);
            Transform temp = SpwanPoints[rand];
            GameObject bot = PhotonNetwork.InstantiateRoomObject("PhotonAI", temp.position, Quaternion.identity, 0);
            SpwanPoints.RemoveAt(rand);

            apperance.StartCoroutine(apperance.GetAppearance(bot.GetComponent<AiController>()));
            rand = Random.Range(0, aiNames.Count);
            bot.GetComponent<AiController>().SetAiName(aiNames[rand]);
            totalAISpawn++;
            yield return new WaitForSeconds(7f);
        }
    }

    public IEnumerator DownloadAddressableTexture(string key, string ObjectType, AiController ai)
    {
        Resources.UnloadUnusedAssets();
        CharcterBodyParts charcterBody = ai.GetComponent<CharcterBodyParts>();
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            AsyncOperationHandle<Texture2D> loadObj;

            try
            {
                loadObj = Addressables.LoadAssetAsync<Texture2D>(key.ToLower());
            }
            catch (System.Exception e)
            {
                // wear default 
                if (ObjectType.Contains("EyeTexture"))
                {
                    charcterBody.ApplyEyeLenTexture(charcterBody.Eye_Texture, ai.gameObject);
                }
                else if (ObjectType.Contains("EyeBrrow"))
                {
                    charcterBody.ApplyEyeBrowTexture(charcterBody.defaultEyebrow, ai.gameObject);
                }
                else if (ObjectType.Contains("Makeup"))
                {
                    charcterBody.ApplyMakeup(charcterBody.defaultMakeup, ai.gameObject);
                }
                else if (ObjectType.Contains("EyeLashes"))
                {
                    charcterBody.ApplyEyeLashes(charcterBody.defaultEyelashes, ai.gameObject);
                }
                throw;
            }

            while (!loadObj.IsDone /*|| loadTex.IsDone*/)
                yield return loadObj;

            if (loadObj.Status == AsyncOperationStatus.Failed)
            {
                // wear default 
                if (ObjectType.Contains("EyeTexture"))
                {
                    charcterBody.ApplyEyeLenTexture(charcterBody.Eye_Texture, ai.gameObject);
                }
                else if (ObjectType.Contains("EyeBrrow"))
                {
                    charcterBody.ApplyEyeBrowTexture(charcterBody.defaultEyebrow, ai.gameObject);
                }
                else if (ObjectType.Contains("Makeup"))
                {
                    charcterBody.ApplyMakeup(charcterBody.defaultMakeup, ai.gameObject);
                }
                else if (ObjectType.Contains("EyeLashes"))
                {
                    charcterBody.ApplyEyeLashes(charcterBody.defaultEyelashes, ai.gameObject);
                }
                yield break;

            }
            else if (loadObj.Status == AsyncOperationStatus.Succeeded)
            {
                if (ObjectType.Contains("EyeTexture"))
                {
                    charcterBody.ApplyEyeLenTexture(loadObj.Result, ai.gameObject);
                }
                else if (ObjectType.Contains("EyeBrrow"))
                {
                    charcterBody.ApplyEyeBrowTexture(loadObj.Result, ai.gameObject);
                }
                else if (ObjectType.Contains("Makeup"))
                {
                    charcterBody.ApplyMakeup(loadObj.Result, ai.gameObject);
                }
                else if (ObjectType.Contains("EyeLashes"))
                {
                    charcterBody.ApplyEyeLashes(loadObj.Result, ai.gameObject);
                }
            }
        }
    }

    public IEnumerator DownloadAddressableWearableWearable(string key, string ObjectType, AiController ai)
    {
        Resources.UnloadUnusedAssets();
        CharcterBodyParts charcterBody = ai.GetComponent<CharcterBodyParts>();
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            AsyncOperationHandle<GameObject> loadObj;

            try
            {
                loadObj = Addressables.LoadAssetAsync<GameObject>(key.ToLower());
            }
            catch (System.Exception)
            {
                WearDefault(ObjectType, ai); // wear default cloth
                throw;
            }
            while (!loadObj.IsDone /*|| loadTex.IsDone*/)
                yield return loadObj;

            if (loadObj.Status == AsyncOperationStatus.Failed)
            {
                WearDefault(ObjectType, ai); // wear default cloth
                yield break;
            }
            else if (loadObj.Status == AsyncOperationStatus.Succeeded)
            {
                ai.StichItem(-1, (GameObject)(object)loadObj.Result, ObjectType, ai.gameObject, false);
            }
        }
    }

    void WearDefault(string type, AiController ai)
    {
        switch (type)
        {
            case "Chest":
                ai.StichItem(-1, ItemDatabase.instance.DefaultShirt, "Chest", ai.gameObject, false);
                break;
            case "Legs":
                ai.StichItem(-1, ItemDatabase.instance.DefaultPent, "Legs", ai.gameObject, false);
                break;
            case "Feet":
                ai.StichItem(-1, ItemDatabase.instance.DefaultShoes, "Feet", ai.gameObject, false);
                break;
            case "Hair":
                ai.StichItem(-1, ItemDatabase.instance.DefaultHair, "Hair", ai.gameObject, false);
                break;
            default:
                break;
        }
    }


}
