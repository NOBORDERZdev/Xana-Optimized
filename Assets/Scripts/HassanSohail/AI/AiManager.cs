using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.AI;

namespace XanaAi
{
    public class AiManager : MonoBehaviour
    {
        public TMP_InputField inputField;
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
        //[SerializeField] List<Transform> SpwanPoints;
        [SerializeField] List<string> aiNames;
        CharcterBodyParts charcterBody;
        List<string> EmotesLink;

        #endregion

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        IEnumerator Start()
        {
            aiPrefab = Resources.Load("NPC_1") as GameObject; // Resources.Load("Ai") as GameObject;

            inputField.text = "1";
            yield return new WaitForSeconds(2f);
            StartCoroutine(IntAis());
        }

        IEnumerator IntAis()
        {
            int rand;
            StartCoroutine(ReactScreen.Instance.getAllReactions());
            for (int i = 0; i < AiCountToSpwaned; i++)
            {
                //rand = Random.Range(0, SpwanPoints.Count);
                //Transform temp = SpwanPoints[rand];

                // Generate a random point on the NavMesh
                Vector3 temp = RandomNavMeshPoint();

                GameObject aiTemp = Instantiate(aiPrefab, temp, Quaternion.identity);
                //aiTemp.transform.position = temp.position;
                //SpwanedAi.Add(aiTemp.GetComponent<AiController>());

                //SpwanPoints.RemoveAt(rand);
                StartCoroutine(apperance.GetAppearance(aiTemp.GetComponent<AiController>()));
                rand = Random.Range(0, aiNames.Count);
                aiTemp.GetComponent<AiController>().SetAiName(aiNames[rand]);
                SpwanedAiCount++;

                yield return new WaitForSeconds(7f);

                int integerValue;
                string data = inputField.text;
                if (int.TryParse(data, out integerValue))
                {
                    // Successfully converted to an integer
                    AiCountToSpwaned = integerValue;
                }
            }
        }

        Vector3 RandomNavMeshPoint()
        {
            NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
            // Get a random triangle from the NavMesh
            int randomTriangleIndex = Random.Range(0, navMeshData.indices.Length / 3);
            int index = randomTriangleIndex * 3;
            // Get the vertices of the random triangle
            Vector3 vertex1 = navMeshData.vertices[navMeshData.indices[index]];
            Vector3 vertex2 = navMeshData.vertices[navMeshData.indices[index + 1]];
            Vector3 vertex3 = navMeshData.vertices[navMeshData.indices[index + 2]];
            // Get a random point within the triangle
            float r1 = Random.Range(0f, 1f);
            float r2 = Random.Range(0f, 1f);

            if (r1 + r2 > 1)
            {
                r1 = 1 - r1;
                r2 = 1 - r2;
            }
            Vector3 randomPoint = vertex1 + r1 * (vertex2 - vertex1) + r2 * (vertex3 - vertex1);
            return randomPoint;
        }


        public void DownloadAddressableWearableWearable(string key, string ObjectType, AiController ai)
        {
            //Resources.UnloadUnusedAssets();
            //CharcterBodyParts charcterBody = ai.GetComponent<CharcterBodyParts>();
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                #region commentedSection
                //AsyncOperationHandle<GameObject> loadObj;
                //try
                //{
                //    loadObj = Addressables.LoadAssetAsync<GameObject>(key.ToLower());
                //}
                //catch (System.Exception)
                //{
                //    WearDefault(ObjectType, ai); // wear default cloth
                //    throw;
                //}

                //while (!loadObj.IsDone /*|| loadTex.IsDone*/)
                //    yield return null; // loadObj;

                //if (loadObj.Status == AsyncOperationStatus.Failed)
                //{
                //    WearDefault(ObjectType, ai); // wear default cloth

                //    yield break;
                //}
                //else if (loadObj.Status == AsyncOperationStatus.Succeeded)
                //{
                //    ai.StichItem(-1, (GameObject)(object)loadObj.Result, ObjectType, ai.gameObject, false);
                //}
                #endregion

                try
                {
                    AsyncOperationHandle<GameObject> loadObj = Addressables.LoadAssetAsync<GameObject>(key.ToLower());
                    loadObj.Completed += operationHandle =>
                    {
                        OnLoadCompleted(operationHandle, ObjectType, ai);
                    };
                }
                catch (System.Exception)
                {
                    Handheld.Vibrate();
                    WearDefault(ObjectType, ai); // wear default cloth
                    apperance.CheckMoreAIDresses(ai);
                    throw new Exception("Error occur in loading addressable. Wear DefaultAvatar");
                }
                //yield return null;
            }
        }

        //public int counter = 0;
        private void OnLoadCompleted(AsyncOperationHandle<GameObject> handle, string ObjectType, AiController ai)
        {

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Loaded Successfully");
                GameObject loadedObject = handle.Result;
                if (loadedObject != null)
                {
                    ai.StichItem(-1, (GameObject)(object)loadedObject, ObjectType, ai.gameObject, false);
                    apperance.CheckMoreAIDresses(ai);
                }
                else
                {
                    Handheld.Vibrate();
                    Debug.LogError("Loaded GameObject is null. Handle the error appropriately.");
                }
            }
            else if (handle.Status == AsyncOperationStatus.Failed)
            {
                Handheld.Vibrate();
                WearDefault(ObjectType, ai); // wear default cloth
                apperance.CheckMoreAIDresses(ai);
                Debug.LogError("Failed to load addressable: " + handle.OperationException);
            }

            //counter++;
            //if (counter >= 4)
            //{
            //    // Release the handle when you're done to free up resources.
            //    Addressables.Release(handle);
            //}
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

        #region UnusedMethod
        public IEnumerator DownloadAddressableTexture(string key, string ObjectType, AiController ai)
        {
            //Resources.UnloadUnusedAssets();
            CharcterBodyParts charcterBody = ai.GetComponent<CharcterBodyParts>();
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                try
                {
                    AsyncOperationHandle<Texture2D> loadObj = Addressables.LoadAssetAsync<Texture2D>(key.ToLower());
                    loadObj.Completed += operationHandle =>
                    {
                        OnTexLoadCompleted(operationHandle, ObjectType, ai);
                    };
                }
                catch (System.Exception)
                {
                    Handheld.Vibrate();
                    if (ObjectType.Contains("EyeTexture"))
                        charcterBody.ApplyEyeLenTexture(charcterBody.Eye_Texture, ai.gameObject);
                    else if (ObjectType.Contains("EyeBrrow"))
                        charcterBody.ApplyEyeBrowTexture(charcterBody.defaultEyebrow, ai.gameObject);
                    else if (ObjectType.Contains("Makeup"))
                        charcterBody.ApplyMakeup(charcterBody.defaultMakeup, ai.gameObject);
                    else if (ObjectType.Contains("EyeLashes"))
                        charcterBody.ApplyEyeLashes(charcterBody.defaultEyelashes, ai.gameObject);

                    throw new Exception("Error occur in loading addressable Textures. Wear DefaultTextures");
                }

                yield return null;
            }
        }

        void OnTexLoadCompleted(AsyncOperationHandle<Texture2D> handle, string ObjectType, AiController ai)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Loaded Textures Successfully");
                Texture2D loadedObject = handle.Result;
                if (loadedObject != null)
                {
                    if (ObjectType.Contains("EyeTexture"))
                        charcterBody.ApplyEyeLenTexture(loadedObject, ai.gameObject);
                    else if (ObjectType.Contains("EyeBrrow"))
                        charcterBody.ApplyEyeBrowTexture(loadedObject, ai.gameObject);
                    else if (ObjectType.Contains("Makeup"))
                        charcterBody.ApplyMakeup(loadedObject, ai.gameObject);
                    else if (ObjectType.Contains("EyeLashes"))
                        charcterBody.ApplyEyeLashes(loadedObject, ai.gameObject);
                }
                else
                {
                    Handheld.Vibrate();
                    Debug.LogError("Loaded Textures are null. Handle the error appropriately.");
                }
            }
            else if (handle.Status == AsyncOperationStatus.Failed)
            {
                // wear default 
                if (ObjectType.Contains("EyeTexture"))
                    charcterBody.ApplyEyeLenTexture(charcterBody.Eye_Texture, ai.gameObject);
                else if (ObjectType.Contains("EyeBrrow"))
                    charcterBody.ApplyEyeBrowTexture(charcterBody.defaultEyebrow, ai.gameObject);
                else if (ObjectType.Contains("Makeup"))
                    charcterBody.ApplyMakeup(charcterBody.defaultMakeup, ai.gameObject);
                else if (ObjectType.Contains("EyeLashes"))
                    charcterBody.ApplyEyeLashes(charcterBody.defaultEyelashes, ai.gameObject);

                Debug.LogError("Failed to load addressable Textures: " + handle.OperationException);
            }

            // Release the handle when you're done to free up resources.
            //Addressables.Release(handle);
        }

        #endregion

    }
}
