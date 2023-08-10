using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace XanaAi
{
    public class AiManager : MonoBehaviour
    {
        #region public 
        [HideInInspector]
        public int SpwanedAiCount = 0;
        public static AiManager instance;
        #endregion
        #region private
        [SerializeField] int AiCountToSpwaned;
        /*[SerializeField]*/
        private GameObject aiPrefab;
        [SerializeField] List<AiController> SpwanedAi;
        [SerializeField] AiAppearance apperance;
        [SerializeField] List<Transform> SpwanPoints;
        [SerializeField] List<string> aiNames;
        CharcterBodyParts charcterBody;
        List<string> EmotesLink;

        #endregion

        private void Awake()
        {
            aiPrefab = Resources.Load("Ai") as GameObject;
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
            //apperance = GetComponent<AiApperance>();

            if (XanaConstants.xanaConstants.isSpawnBot)
                StartCoroutine(IntAis());
        }

        IEnumerator IntAis()
        {
            int rand;
            ReactScreen.Instance.StartCoroutine(ReactScreen.Instance.getAllReactions());
            for (int i = 0; i < AiCountToSpwaned; i++)
            {
                rand = Random.Range(0, SpwanPoints.Count);
                Transform temp = SpwanPoints[rand];
                GameObject aiTemp = Instantiate(aiPrefab, temp.position, Quaternion.identity);
                //aiTemp.transform.position = temp.position;
                //SpwanedAi.Add(aiTemp.GetComponent<AiController>());
                SpwanPoints.RemoveAt(rand);
                apperance.StartCoroutine(apperance.GetAppearance(aiTemp.GetComponent<AiController>()));
                rand = Random.Range(0, aiNames.Count);
                aiTemp.GetComponent<AiController>().SetAiName(aiNames[rand]);
                SpwanedAiCount++;
                yield return new WaitForSeconds(7f);
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

}
