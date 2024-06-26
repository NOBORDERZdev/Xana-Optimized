using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UFE3D;
using UnityEngine;
using UnityEngine.Networking;
using static CharacterHandler;
using static System.Net.WebRequestMethods;

public class SPAAIHandler : MonoBehaviour
{
    public GameObject[] AIAvatarPrefabs;
    public Transform SpawnPoint;
    public GameObject CurrentAIPerformerRef;
    PerformerAvatarData AvatarData;

    // Start is called before the first frame update
    void Start()
    {
        string finalAPIURL = "https://run.mocky.io/v3/0f752598-d710-4e51-ba8d-00bdb37cd1e6";
        StartCoroutine(GetDataFromAPI(finalAPIURL, (isSucess, response) =>
        {
            if (isSucess)
            {
                AvatarData = new PerformerAvatarData();
                AvatarData = JsonUtility.FromJson<PerformerAvatarData>(response);
                Debug.Log("=======API Call working" + AvatarData.data.animations[0].animationName);
            }
        }));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>())
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                SpawnAIPerformer();
            }
        }
    }

    void SpawnAIPerformer()
    {
        if (CurrentAIPerformerRef)
        {
            CurrentAIPerformerRef.SetActive(true);
        }
        else
        {
            int _aiPrefabIndex = UnityEngine.Random.Range(0, AIAvatarPrefabs.Length);
            CurrentAIPerformerRef = Instantiate(AIAvatarPrefabs[_aiPrefabIndex], SpawnPoint.position, Quaternion.identity);
            CurrentAIPerformerRef.GetComponent<SPAAIDresser>().AvatarJson = AvatarData.data.json;
        }
    }

    IEnumerator GetDataFromAPI(string apiURL, Action<bool, string> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL))
        {
            //www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return null;
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                callback(false, null);
            }
            else
            {
                callback(true, www.downloadHandler.text);
            }
            www.Dispose();
        }
    }
    [System.Serializable]
    public class PerformerAvatarData
    {
        public bool Success;
        public PerformerAvatarDetails data;
        public string msg;
    }
    [System.Serializable]
    public class PerformerAvatarDetails
    {
        public string Gender;
        public SavingCharacterDataClass json;
        public AnimationData[] animations;
    }
    [System.Serializable]
    public class AnimationData
    {
        public string animationName;
        public float playTime;
    }
}
