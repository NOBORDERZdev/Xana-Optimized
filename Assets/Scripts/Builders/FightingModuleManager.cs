using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class FightingModuleManager : MonoBehaviour
{
    public bool isUserHaveAlphaPass;
    public bool isEnvLoaded;
    public string addressableSceneName;
    public string environmentLabel;
    public int _NFTIndex;
    public string player1Icon;
    public string player2Icon;
    public AvatarController NFTAvatarController;
    public static FightingModuleManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OnClickMainMenu()
    {
        StartCoroutine(IEOnClickMainMenu());
    }

    public IEnumerator IEOnClickMainMenu()
    {
        const string UsernamePrefs = "PlayerName";
        string UserName = "";
        if (PlayerPrefs.HasKey(UsernamePrefs))
        {
            UserName = PlayerPrefs.GetString(UsernamePrefs);
        }
        Debug.LogError("UserName: " + UserName);
        Input.multiTouchEnabled = true;
        yield return new WaitForSeconds(.1f);
        //SceneManager.LoadScene("Demo_Fighter3D - Type 2");
        SceneManager.LoadScene("FightingModuleMenu");
    }
    public void EqipRandomNFT()
    {
        fighterNFTlist = UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTlistdata.list.FindAll(o => o.collection.name.StartsWith("XANA x BreakingDown"));
        _NFTIndex = Random.Range(0, fighterNFTlist.Count);
        SaveNFTAttributesInFile();
        NFTAvatarController.EquipNFT(true);
        XanaConstants.xanaConstants.isNFTEquiped = true;
        //BoxerNFTEventManager.OnNFTequip?.Invoke(true);
    }
    public List<List> fighterNFTlist = new List<List>();
    public void SaveNFTAttributesInFile()
    {
        Debug.LogError("SaveAttributesInFile: " + _NFTIndex);
        BoxerNFTDataClass nftAttributes = new BoxerNFTDataClass();
        nftAttributes.isNFTAquiped = true;
        nftAttributes.id = fighterNFTlist[_NFTIndex].attribute.id.ToString();
        nftAttributes.Gloves = fighterNFTlist[_NFTIndex].attribute.Gloves;
        nftAttributes.Glasses = fighterNFTlist[_NFTIndex].attribute.glasses;
        nftAttributes.Full_Costumes = fighterNFTlist[_NFTIndex].attribute.Full_Costumes;
        nftAttributes.Chains = fighterNFTlist[_NFTIndex].attribute.Chains;
        nftAttributes.Hairs = fighterNFTlist[_NFTIndex].attribute.hairs;
        nftAttributes.Face_Tattoo = fighterNFTlist[_NFTIndex].attribute.Face_Tattoo;
        nftAttributes.Forehead_Tattoo = fighterNFTlist[_NFTIndex].attribute.Forehead_Tattoo;
        nftAttributes.Chest_Tattoo = fighterNFTlist[_NFTIndex].attribute.Chest_Tattoo;
        nftAttributes.Arm_Tattoo = fighterNFTlist[_NFTIndex].attribute.Arm_Tattoo;
        nftAttributes.Legs_Tattoo = fighterNFTlist[_NFTIndex].attribute.legs_Tattoo;
        nftAttributes.Shoes = fighterNFTlist[_NFTIndex].attribute.Shoes;
        nftAttributes.Mustache = fighterNFTlist[_NFTIndex].attribute.Mustache;
        nftAttributes.Pants = fighterNFTlist[_NFTIndex].attribute.Pants;
        nftAttributes.Eyebrows = fighterNFTlist[_NFTIndex].attribute.Eyebrows;
        nftAttributes.Lips = fighterNFTlist[_NFTIndex].attribute.Lips;
        nftAttributes.Heads = fighterNFTlist[_NFTIndex].attribute.head;
        nftAttributes.Eye_Shapes = fighterNFTlist[_NFTIndex].attribute.Eye_Shapes;
        nftAttributes.Skin = fighterNFTlist[_NFTIndex].attribute.Skin;
        nftAttributes.Eye_Lense = fighterNFTlist[_NFTIndex].attribute.Eye_lense;
        nftAttributes.Eyelid = fighterNFTlist[_NFTIndex].attribute.Eyelid;
        nftAttributes.profile = fighterNFTlist[_NFTIndex].attribute.profile;
        nftAttributes.speed = fighterNFTlist[_NFTIndex].attribute.speed;
        nftAttributes.stamina = fighterNFTlist[_NFTIndex].attribute.stamina;
        nftAttributes.punch = fighterNFTlist[_NFTIndex].attribute.punch;
        nftAttributes.kick = fighterNFTlist[_NFTIndex].attribute.kick;
        nftAttributes.defence = fighterNFTlist[_NFTIndex].attribute.defence;
        nftAttributes.special_move = fighterNFTlist[_NFTIndex].attribute.special_move;
        string attributesJson = JsonUtility.ToJson(nftAttributes);
        Debug.LogError("NFT Json Data: " + attributesJson);
        File.WriteAllText(Application.persistentDataPath + XanaConstants.xanaConstants.NFTBoxerJson, attributesJson);
    }
}
