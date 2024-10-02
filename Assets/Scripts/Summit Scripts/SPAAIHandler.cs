using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UFE3D;
using UnityEngine;
using UnityEngine.Networking;
using static CharacterHandler;
using static InventoryManager;
using static System.Net.WebRequestMethods;

public class SPAAIHandler : MonoBehaviour
{
    public int AreaID = 0;
    public int AvatarID = 0;
    public Transform SpawnPoint;
    public GameObject CurrentAIPerformerRef;
    public PerformerAvatarData AvatarData;
    public bool IsAIDataFetched = false;
    public bool IsPlayerTriggered = false;
    string prfrmrAvtrAPIURL = "/domes/getDomePerfomerAvatarsInfoByAreaIdIndex/";
    public delegate void SoundEnabler(bool _soundEnable);
    public SoundEnabler LiveVideoSoundEnabler;

    // Start is called before the first frame update
    void Start()
    {
        CallPrfrmrAvtrAPI();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
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
                LiveVideoSoundEnabler?.Invoke(true);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
         
                if (CurrentAIPerformerRef)
                {
                    CurrentAIPerformerRef.SetActive(false);
                }
                LiveVideoSoundEnabler?.Invoke(false);
            IsPlayerTriggered = false;
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
            CurrentAIPerformerRef.GetComponent<SPAAIBehvrController>().isPerformingAction = false;
            StartCoroutine(CurrentAIPerformerRef.GetComponent<SPAAIBehvrController>().PerformAction());
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
        if (!CurrentAIPerformerRef)
        {
            CurrentAIPerformerRef = Instantiate(GameplayEntityLoader.instance.AIAvatarPrefab[_index], SpawnPoint.position, SpawnPoint.localRotation);
            if (CurrentAIPerformerRef.GetComponent<Rigidbody>())
            {
                
                //CurrentAIPerformerRef.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                //CurrentAIPerformerRef.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
                //CurrentAIPerformerRef.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
                CurrentAIPerformerRef.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                //CurrentAIPerformerRef.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            }
            AssignFtchDataToAIAvtr();
        }
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
        CurrentAIPerformerRef.GetComponent<SPAAIBehvrController>().spaAIHandlerRef = this;
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
