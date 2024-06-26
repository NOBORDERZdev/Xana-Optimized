using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UFE3D;
using UnityEngine;
using UnityEngine.Networking;
using VTabs.Libs;
using static CharacterHandler;
using static System.Net.WebRequestMethods;

public class SPAAIHandler : MonoBehaviour
{
    public GameObject[] AIAvatarPrefabs;
    public Transform SpawnPoint;
    public GameObject CurrentAIPerformerRef;
    public PerformerAvatarData AvatarData;
    public bool IsAIDataFetched = false;
    public bool IsPlayerTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        string finalAPIURL = "https://run.mocky.io/v3/55ddc8dd-3daf-421a-b62d-0974d1ae612a";
        StartCoroutine(GetDataFromAPI(finalAPIURL, (isSucess, response) =>
        {
            if (isSucess)
            {
                AvatarData = new PerformerAvatarData();
                AvatarData = JsonUtility.FromJson<PerformerAvatarData>(response);
                if (IsPlayerTriggered)
                {
                    SpawnAIPerformer();
                }
                IsAIDataFetched = true;
            }
        }));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>())
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                if (IsAIDataFetched)
                {
                    SpawnAIPerformer();
                }
                IsPlayerTriggered = true;
            }
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

    void SpawnAIPerformer()
    {
        if (CurrentAIPerformerRef)
        {
            CurrentAIPerformerRef.SetActive(true);
        }
        else
        {
            if (AvatarData.data.Gender == "Female")
            {
                GenderBasedPrefabSlect(0);
            }
            else
            {
                GenderBasedPrefabSlect(1);
            }
        }
    }

    void GenderBasedPrefabSlect(int _index)
    {
        CurrentAIPerformerRef = Instantiate(AIAvatarPrefabs[_index], SpawnPoint.position, Quaternion.identity);
        CurrentAIPerformerRef.GetComponent<SPAAIDresser>().AvatarJson = AvatarData.data.json;
        SPAAIEmoteController spawnAIEmoteControllerRef = CurrentAIPerformerRef.GetComponent<SPAAIEmoteController>();
        spawnAIEmoteControllerRef.AnimPlayList.Clear();
        spawnAIEmoteControllerRef.AnimPlayList.TrimExcess();
        spawnAIEmoteControllerRef.AnimPlayTimer.Clear();
        spawnAIEmoteControllerRef.AnimPlayTimer.TrimExcess();
        foreach (AnimationData animData in AvatarData.data.animations)
        {
            spawnAIEmoteControllerRef.AnimPlayList.Add(animData.animationName);
            spawnAIEmoteControllerRef.AnimPlayTimer.Add(animData.playTime);
        }
        StartCoroutine(CurrentAIPerformerRef.GetComponent<SPAAIBehvrController>().PerformAction());
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
