using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static CharacterHandler;
using static XANASummitDataContainer;

public class SummitDomePAAIController : MonoBehaviour
{
    public List<int> PerformerAvatarAIIndex = new List<int>();
    public List<GameObject> InstPerformerAvatarAIObjects = new List<GameObject>();
    private bool isPerformerAvtrInit;

    private void OnEnable()
    {
        BuilderEventManager.AfterPlayerInstantiated += InitPerformerAvatarNPC;
        BuilderEventManager.ResetSummit += ResetOnExit;
    }

    private void OnDisable()
    {
        BuilderEventManager.AfterPlayerInstantiated -= InitPerformerAvatarNPC;
        BuilderEventManager.ResetSummit -= ResetOnExit;
    }

    void InitPerformerAvatarNPC()
    {
        if (ConstantsHolder.isFromXANASummit && !isPerformerAvtrInit)
        {
            isPerformerAvtrInit = true;
            GetNPCDATA(ConstantsHolder.domeId);
        }
    }

    async void GetNPCDATA(int domeId)
    {
        bool domeWithAINPC = await GameplayEntityLoader.instance.XanaSummitDataContainerObject.GetAIData(domeId);
        if (domeWithAINPC)
        {
            bool conPerformerAI = CheckForPerformerAvatarAI();
            if (conPerformerAI)
            {
                InstPerformerAvatarAI();
            }
        }
    }

    bool CheckForPerformerAvatarAI()
    {
        for (int i = 0; i < GameplayEntityLoader.instance.XanaSummitDataContainerObject.aiData.npcData.Count; i++)
        {
            if (GameplayEntityLoader.instance.XanaSummitDataContainerObject.aiData.npcData[i].isAvatarPerformer)
            {
                PerformerAvatarAIIndex.Add(i);
            }
        }
        if (PerformerAvatarAIIndex.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    async void InstPerformerAvatarAI()
    {
        for (int i = 0; i < PerformerAvatarAIIndex.Count; i++)
        {
            GameObject performerAvatarAIRef = GenderBasedPrefabSlect(PerformerAvatarAIIndex[i]);
            InstPerformerAvatarAIObjects.Add(performerAvatarAIRef);
            await AssignPADataToInstAI(performerAvatarAIRef, PerformerAvatarAIIndex[i]);
        }
    }

    GameObject GenderBasedPrefabSlect(int _index)
    {
        if (GameplayEntityLoader.instance.XanaSummitDataContainerObject.aiData.npcData[_index].avatarId > 10)
        {
            return Instantiate(GameplayEntityLoader.instance.AIAvatarPrefab[0]);
        }
        else
        {
            return Instantiate(GameplayEntityLoader.instance.AIAvatarPrefab[1]);
        }
    }

    async Task AssignPADataToInstAI(GameObject _aiAvatar, int _aiDataIndex)
    {
        SetPerformerAIClothingAndPosition(_aiAvatar, _aiDataIndex);
        await SetPerformerAIAnim(_aiAvatar, _aiDataIndex);
        SetPerformerAIBehvCntrlr(_aiAvatar);
    }

    void SetPerformerAIClothingAndPosition(GameObject _aiAvatar, int _aiDataIndex)
    {
        SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
        _CharacterData = _CharacterData.CreateFromJSON(GameplayEntityLoader.instance.XanaSummitDataContainerObject.aiData.npcData[_aiDataIndex].avatarCategory);
        _aiAvatar.GetComponent<SPAAIDresser>().AvatarJson = _CharacterData;
        _aiAvatar.transform.position = new Vector3(GameplayEntityLoader.instance.XanaSummitDataContainerObject.aiData.npcData[_aiDataIndex].spawnPositionArray[0] + 2f,
            GameplayEntityLoader.instance.XanaSummitDataContainerObject.aiData.npcData[_aiDataIndex].spawnPositionArray[1],
            GameplayEntityLoader.instance.XanaSummitDataContainerObject.aiData.npcData[_aiDataIndex].spawnPositionArray[2]);
    }

    async Task SetPerformerAIAnim(GameObject _aiAvatar, int _aiDataIndex)
    {
        await Task.Run(() =>
        {
            SPAAIEmoteController _spawnAIEmoteControllerRef = _aiAvatar.GetComponent<SPAAIEmoteController>();
            _spawnAIEmoteControllerRef.AnimPlayList.Clear();
            _spawnAIEmoteControllerRef.AnimPlayList.TrimExcess();
            _spawnAIEmoteControllerRef.AnimPlayTimer.Clear();
            _spawnAIEmoteControllerRef.AnimPlayTimer.TrimExcess();
            for (int i = 0; i < GameplayEntityLoader.instance.XanaSummitDataContainerObject.aiData.npcData[_aiDataIndex].animations.Length; i++)
            {
                _spawnAIEmoteControllerRef.AnimPlayList.Add(GameplayEntityLoader.instance.XanaSummitDataContainerObject.aiData.npcData[_aiDataIndex].animations[i].name);
                _spawnAIEmoteControllerRef.AnimPlayTimer.Add(GameplayEntityLoader.instance.XanaSummitDataContainerObject.aiData.npcData[_aiDataIndex].animations[i].playTime);
            }
            _aiAvatar.SetActive(true);
        });
    }

    void SetPerformerAIBehvCntrlr(GameObject _aiAvatar)
    {
        //_aiAvatar.GetComponent<SPAAIBehvrController>().spaAIHandlerRef = this;
        StartCoroutine(_aiAvatar.GetComponent<SPAAIBehvrController>().PerformAction());
    }

    void ResetOnExit()
    {
        DestroyNPC();
    }

    void DestroyNPC()
    {
        for (int i = 0; i < InstPerformerAvatarAIObjects.Count; i++)
        {
            if (InstPerformerAvatarAIObjects[i] != null)
                Destroy(InstPerformerAvatarAIObjects[i]);
        }

        InstPerformerAvatarAIObjects.Clear();
        InstPerformerAvatarAIObjects.TrimExcess();
        PerformerAvatarAIIndex.Clear();
        PerformerAvatarAIIndex.TrimExcess();
        isPerformerAvtrInit = false;
    }
}
