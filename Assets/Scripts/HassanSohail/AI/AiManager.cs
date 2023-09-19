using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;
using UnityEngine.AI;

namespace XanaAi
{
    public class AiManager : MonoBehaviour
    {
        #region public 
        public static AiManager instance;
        [HideInInspector]
        public int decoratedAi = 0;
        [HideInInspector]
        public int SpwanedAiCount = 0;
        #endregion

        #region private
        [Space(5)]
        //[SerializeField] int aiCountToSpwan;
        [SerializeField] AiAppearance apperance;
        [SerializeField] List<string> aiNames;

        private CharcterBodyParts charcterBody;
        [SerializeField]
        private GameObject[] aiPrefabs;
        //private List<GameObject> spawnedNpc;
        private int typesOfAICharacter = 3;
        private int rand;
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
            //aiPrefabs = new GameObject[typesOfAICharacter];
            //spawnedNpc = new List<GameObject>();

            //for (int i =0; i< typesOfAICharacter; i++) 
            //    aiPrefabs[i] = Resources.Load("NPC/NPC_" + /*(i+1)*/) as GameObject;

            for (int i = 0; i < aiPrefabs.Length; i++)
            {
                Vector3 temp = RandomNavMeshPoint();
                //spawnedNpc.Add(Instantiate(aiPrefabs[Random.Range(0, aiPrefabs.Length)], temp, Quaternion.identity));
                //Instantiate(aiPrefabs[Random.Range(0, aiPrefabs.Length)], temp, Quaternion.identity);
                aiPrefabs[i].transform.position = temp;
                aiPrefabs[i].transform.rotation = Quaternion.identity;
                SpwanedAiCount++;

                rand = Random.Range(0, aiNames.Count);
                aiPrefabs[i].GetComponent<AiController>().SetAiName(aiNames[rand]);       // Set npc names
                apperance.StartWandering(aiPrefabs[i].GetComponent<AiController>());      // start perform action
            }

            StartCoroutine(ReactScreen.Instance.getAllReactions());

            yield return null;
            //yield return new WaitForSeconds(1f);
            //InitilizeAI();
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

        #region ClotheWearableRegion
        public void InitilizeAI()
        {
            //if (decoratedAi >= aiCountToSpwan) return;
            //StartCoroutine(apperance.GetAppearance(spawnedNpc[decoratedAi].GetComponent<AiController>()));
            apperance.DecorateAI(aiPrefabs[decoratedAi].GetComponent<AiController>());
        }

        public void DownloadAddressableWearableWearable(string key, string ObjectType, AiController ai)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
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
                      WearDefault(ObjectType, ai); // wear default cloth
                                        //apperance.CheckMoreAIDresses(ai);         // remove it later
                    apperance.CheckMoreAIDresses(ai);
                    throw new Exception("Error occur in loading addressable. Wear DefaultAvatar");
                }
            }
        }

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

            //    // Release the handle when you're done to free up resources.
                //Addressables.Release(handle);
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

        #endregion

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
