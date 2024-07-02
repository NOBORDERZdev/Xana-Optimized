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
using static InventoryManager;
using static System.Net.WebRequestMethods;

public class SPAAIHandler : MonoBehaviour
{
    public int AreaID = 0;
    public int AvatarID = 0;
    public GameObject[] AIAvatarPrefabs;
    public Transform SpawnPoint;
    public GameObject CurrentAIPerformerRef;
    public PerformerAvatarData AvatarData;
    public bool IsAIDataFetched = false;
    public bool IsPlayerTriggered = false;
    string prfrmrAvtrAPIURL = "/domes/getDomePerfomerAvatarsInfoByAreaIdIndex/";

    // Start is called before the first frame update
    void Start()
    {
        CallPrfrmrAvtrAPI();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>())
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                if (IsAIDataFetched)
                {
                    SpawnAIPerformer();
                }
                else if (!IsAIDataFetched && IsPlayerTriggered)
                {
                    CallPrfrmrAvtrAPI();
                }
                IsPlayerTriggered = true;
            }
        }
    }

    void CallPrfrmrAvtrAPI()
    {
        string _finalAPIURL = ConstantsGod.API_BASEURL + prfrmrAvtrAPIURL + AreaID + "/" + AvatarID;
        StartCoroutine(GetDataFromAPI(_finalAPIURL, (isSucess, response) =>
        {
            if (isSucess)
            {
                AvatarData = new PerformerAvatarData();
                AvatarData = JsonUtility.FromJson<PerformerAvatarData>(response);
                if (!string.IsNullOrEmpty(AvatarData.performerAvatar.gender))
                {
                    if (IsPlayerTriggered)
                    {
                        SpawnAIPerformer();
                    }
                    IsAIDataFetched = true;
                }
                else
                {
                    Debug.LogError($"No data found agaist specified avatar id{AvatarID} and area id{AreaID}");
                }
            }
            else
            {
                Debug.LogError($"No data found agaist specified avatar id{AvatarID} and area id{AreaID}/Encountered possible network error");
            }
        }));
    }

    IEnumerator GetDataFromAPI(string apiURL, Action<bool, string> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
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
            if (AvatarData.performerAvatar.gender == "Female")
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
        CurrentAIPerformerRef = Instantiate(AIAvatarPrefabs[_index], SpawnPoint.position, SpawnPoint.localRotation);
        AssignFtchDataToAIAvtr();
    }

    void AssignFtchDataToAIAvtr()
    {
        CurrentAIPerformerRef.GetComponent<SPAAIDresser>().AvatarJson = AvatarData.performerAvatar.clothingJson;
        SPAAIEmoteController _spawnAIEmoteControllerRef = CurrentAIPerformerRef.GetComponent<SPAAIEmoteController>();
        _spawnAIEmoteControllerRef.AnimPlayList.Clear();
        _spawnAIEmoteControllerRef.AnimPlayList.TrimExcess();
        _spawnAIEmoteControllerRef.AnimPlayTimer.Clear();
        _spawnAIEmoteControllerRef.AnimPlayTimer.TrimExcess();
        foreach (AnimationData animData in AvatarData.performerAvatar.animations)
        {
            _spawnAIEmoteControllerRef.AnimPlayList.Add(animData.name);
            _spawnAIEmoteControllerRef.AnimPlayTimer.Add(animData.playTime);
        }
        StartCoroutine(CurrentAIPerformerRef.GetComponent<SPAAIBehvrController>().PerformAction());
    }

    [System.Serializable]
    public class PerformerAvatarData
    {
        public bool Success;
        public PerformerAvatarDetails performerAvatar;
        public string msg;
    }
    [System.Serializable]
    public class PerformerAvatarDetails
    {
        public int id;
        public int areaId;
        public int index;
        public string gender;
        public SavingCharacterDataClass clothingJson;
        public AnimationData[] animations;
        public string areaName;
        public DateTime createdAt;
        public DateTime updatedAt;
    }
    [System.Serializable]
    public class AnimationData
    {
        public string name;
        public float playTime;
    }
}
