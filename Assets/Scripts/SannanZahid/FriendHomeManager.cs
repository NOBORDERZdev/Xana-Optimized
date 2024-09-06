using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using static InventoryManager;

public class FriendHomeManager : MonoBehaviour
{
    public Transform maleFriendAvatarPrefab, femaleFriendAvatarPrefab, NameTagFriendAvatarPrefab, PostBubbleFriendAvatarPrefab;
    [NonReorderable]
    [SerializeField]
    BestFriendData _friendsDataFetched;
    [SerializeField]
    OnlineFriends _onlineFriendsDataFetched;
    public bool SpawnFriendsAgain;
    public List<FriendSpawnData> SpawnFriendsObj = new List<FriendSpawnData>();

    public GameObject menuLightObj;
    public GameObject profileLightingObj;

    private int _randAvatarGender;
    private void OnEnable()
    {
        StartCoroutine(BuildMoodDialog());
        HomeScoketHandler.instance.updateFriendPostDelegate += UpdateFriendPost;

        MainSceneEventHandler.OnSucessFullLogin += SpawnFriends;
    }
    private void OnDisable()
    {
        if (HomeScoketHandler.instance != null)
            HomeScoketHandler.instance.updateFriendPostDelegate -= UpdateFriendPost;

        MainSceneEventHandler.OnSucessFullLogin -= SpawnFriends;
    }
    public void GetOnlineFriends()
    {
        //print("online api called...");
        StartCoroutine(IEGetOnlineFriends());
    }
    IEnumerator IEGetOnlineFriends()
    {
        string apiURL = ConstantsGod.API_BASEURL + ConstantsGod.r_url_OnlineFriends;
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.result);
            }
            else
            {
                _onlineFriendsDataFetched = JsonUtility.FromJson<OnlineFriends>(www.downloadHandler.text);
                for (int i = 0; i < SpawnFriendsObj.Count; i++)
                {
                    for (int j = 0; j < _onlineFriendsDataFetched.data.Count; j++)
                    {
                        if (_onlineFriendsDataFetched.data[j].userId.Equals(SpawnFriendsObj[i].id))
                        {
                            if (_onlineFriendsDataFetched.data[j].isOnline && _onlineFriendsDataFetched.data[j].isWorldJoin)
                            {
                                SpawnFriendsObj[i].friendNameObj.GetComponent<CheckOnlineFriend>().ToggleOnlineStatus(true);
                                WorldManager.instance.SetFriendsJoinedWorldInfo(_onlineFriendsDataFetched.data[j].worldDetails, SpawnFriendsObj[i].friendNameObj.GetComponent<WorldItemView>());
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
    
    public void SpawnFriends()
    {
        if(SpawnFriendsAgain)
        {
            Debug.Log("Spawning Friends Again");
            SpawnFriendsAgain = false;
            StartCoroutine(BuildMoodDialog());
        }
        GetOnlineFriends();
    }
    string PrepareApiURL()
    {
        return ConstantsGod.API_BASEURL + "/social/get-close-friends/" + ConstantsHolder.userId;
    }
   IEnumerator BuildMoodDialog()
    {
        while (ConstantsGod.AUTH_TOKEN == "AUTH_TOKEN")
            yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(1f);
        string finalAPIURL = PrepareApiURL();
        StartCoroutine(FetchUserMapFromServer(finalAPIURL, (isSucess) =>
        {
            if (isSucess)
            {
                var friendIds = new HashSet<int>(SpawnFriendsObj.Select(x => x.id));
                foreach (FriendsDetail friend in _friendsDataFetched.data.rows)
                {
                    if (!friendIds.Contains(friend.id))
                    {
                        StartCoroutine(CreateFriend(friend));
                    }
                }
                GetOnlineFriends();
            }
            else
            {
                StartCoroutine(BuildMoodDialog());
            }
        }));
    }

    IEnumerator CreateFriend(FriendsDetail friend)
    {
        FriendSpawnData FriendSpawn = new FriendSpawnData();
        Transform CreatedFriend;
        GameObject avatarPrefab;
        if (friend.userOccupiedAssets.Count > 0 && friend.userOccupiedAssets[0].json != null)
        {
            if (friend.userOccupiedAssets[0].json.gender == "Male")
                avatarPrefab= maleFriendAvatarPrefab.gameObject;
            else
                avatarPrefab = femaleFriendAvatarPrefab.gameObject;
        }
        else
        {
            _randAvatarGender = UnityEngine.Random.Range(0, 2);
            avatarPrefab = (_randAvatarGender == 0 ? femaleFriendAvatarPrefab : maleFriendAvatarPrefab).gameObject;
        }
        CreatedFriend = Instantiate(avatarPrefab, avatarPrefab.transform.parent).transform;
        //Transform CreatedFriend = Instantiate(FriendAvatarPrefab, FriendAvatarPrefab.parent).transform;
        yield return null; // Wait for the next frame to continue execution

        Transform CreatedFriendPostBubble = Instantiate(PostBubbleFriendAvatarPrefab, PostBubbleFriendAvatarPrefab.parent).transform;
        Transform CreatedNameTag = Instantiate(NameTagFriendAvatarPrefab, NameTagFriendAvatarPrefab.parent).transform;
        CreatedNameTag.GetComponent<FollowUser>().targ = CreatedFriend;
        CreatedNameTag.gameObject.SetActive(true);
        CreatedNameTag.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = friend.name;
        CreatedNameTag.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMPro.TMP_Text>().text = friend.name;
        CreatedNameTag.GetComponent<CheckOnlineFriend>().friendId = friend.id;
        CreatedFriend.GetComponent<Actor>().NameTagHolderObj = CreatedNameTag;
        CreatedFriend.gameObject.SetActive(true);
        CreatedFriend.GetComponent<Actor>().Init(GameManager.Instance.ActorManager.actorBehaviour[GetPostRandomDefaultAnim()]);
        if (friend.userOccupiedAssets.Count > 0 && friend.userOccupiedAssets[0].json != null)
        {
            CreatedFriend.GetComponent<AvatarController>().InitializeFrndAvatar(friend.userOccupiedAssets[0].json,CreatedFriend.gameObject);
            CreatedNameTag.GetComponent<CheckOnlineFriend>().json = friend.userOccupiedAssets[0].json;
        }
        else
        {
            int _rand = UnityEngine.Random.Range(0, CreatedFriend.GetComponent<CharacterBodyParts>().randomPresetData.Length);
            CreatedFriend.GetComponent<AvatarController>().DownloadRandomFrndPresets(_rand);
            CreatedNameTag.GetComponent<CheckOnlineFriend>().randomPreset = _rand;
            CreatedNameTag.GetComponent<CheckOnlineFriend>().randomPresetGender = (_randAvatarGender == 0) ? "Female":"Male";
        }
        CreatedFriend.GetComponent<PlayerPostBubbleHandler>().InitObj(CreatedFriendPostBubble,
            CreatedFriendPostBubble.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>());

        if (CreatedFriend.GetComponent<EyesBlinking>() != null)
        {
            CreatedFriend.GetComponent<EyesBlinking>().StoreBlendShapeValues();
            StartCoroutine(CreatedFriend.GetComponent<EyesBlinking>().BlinkingStartRoutine());
        }

        FriendSpawn.id = friend.id;
        FriendSpawn.friendObj = CreatedFriend;
        FriendSpawn.friendNameObj = CreatedNameTag;
        FriendSpawn.friendPostBubbleObj = CreatedFriendPostBubble;
        SpawnFriendsObj.Add(FriendSpawn);
        GameManager.Instance.PostManager.GetComponent<UserPostFeature>().GetLatestPostOfFriend(
            friend.id,
            CreatedFriend.GetComponent<PlayerPostBubbleHandler>(),
            CreatedFriend.GetComponent<Actor>()
            );
        yield return null;
    }

    IEnumerator FetchUserMapFromServer(string apiURL, Action<bool> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                callback(false);
            }
            else
            {
                _friendsDataFetched = JsonUtility.FromJson<BestFriendData>(www.downloadHandler.text);
                callback(true);
            }
        }
    }
    public void EnableFriendsView(bool flag)
    {
        foreach (FriendSpawnData SpawnFriendsObjref in SpawnFriendsObj)
        {
            SpawnFriendsObjref.friendNameObj.gameObject.SetActive(flag);
            SpawnFriendsObjref.friendObj.gameObject.SetActive(flag);
            SpawnFriendsObjref.friendPostBubbleObj.gameObject.SetActive(flag);
            if (SpawnFriendsObjref.friendObj.GetComponent<EyesBlinking>() && !SpawnFriendsObjref.friendObj.GetComponent<EyesBlinking>().isCoroutineRunning)   // Added by Ali Hamza
                StartCoroutine(SpawnFriendsObjref.friendObj.GetComponent<EyesBlinking>().BlinkingStartRoutine());
        }
    }
    FriendSpawnData _friendtoRemove;
    public void RemoveFriendFromHome(int friendId)
    {
        _friendtoRemove = null;
        foreach (FriendSpawnData SpawnFriendsObjref in SpawnFriendsObj)
        {
            if (SpawnFriendsObjref.id == friendId)
            {
                _friendtoRemove = SpawnFriendsObjref;
                break;
            }
        }
        if (_friendtoRemove != null)
        {
            SpawnFriendsObj.Remove(_friendtoRemove);
            Destroy(_friendtoRemove.friendNameObj.gameObject);
            Destroy(_friendtoRemove.friendObj.gameObject);
            Destroy(_friendtoRemove.friendPostBubbleObj.gameObject);
        }
    }
    public void RemoveAllFriends()
    {
        foreach (FriendSpawnData SpawnFriendsObjref in SpawnFriendsObj)
        {
            Destroy(SpawnFriendsObjref.friendNameObj.gameObject);
            Destroy(SpawnFriendsObjref.friendObj.gameObject);
            Destroy(SpawnFriendsObjref.friendPostBubbleObj.gameObject);
        }
        SpawnFriendsObj.Clear();
        SpawnFriendsAgain = true;
    }
    public void AddFriendToHome()
    {
        StartCoroutine(BuildMoodDialog());
    }


    private void UpdateFriendPost(ReceivedFriendPostData data)
    {
      //  print("________________________ " + data.creatorId);
     //   print("________________________ " + data.text_mood);
       // print("________________________ " + data.text_post);
        foreach (var frds in SpawnFriendsObj)
        {
            if (frds.id == int.Parse(data.creatorId))
            {
                if (!string.IsNullOrEmpty(data.text_post) && !data.text_post.Equals("null"))
                {
                    frds.friendPostBubbleObj.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = data.text_post;
                    frds.friendPostBubbleObj.transform.GetChild(0).gameObject.SetActive(true);
                }

                if (!string.IsNullOrEmpty(data.text_mood) && !data.text_mood.Equals("null"))
                {
                  //  print("________________________ " + data.text_mood);
                    GameManager.Instance.PostManager.GetComponent<UserAnimationPostFeature>().SetMood(data.text_mood, frds.friendObj.GetComponent<Actor>());
                    // Update Animation Here
                }
                if(string.IsNullOrEmpty(data.text_post))
                {
                    frds.friendPostBubbleObj.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }


    }

    public void UpdateFrendAvatar(int id, SavingCharacterDataClass json)
    {
        foreach (var frnd in SpawnFriendsObj)
        {
            if ( frnd.id == id && json!=null && frnd.friendObj.gameObject.activeInHierarchy)
            {
                frnd.friendObj.GetComponent<AvatarController>().InitializeFrndAvatar(json,frnd.friendObj.gameObject);
            }

        }
    }


    int GetPostRandomDefaultAnim(){ 
        float _rand = UnityEngine.Random.Range(0.1f, 3.0f);
        int value; 
        if (_rand <= 1.0f)
        {
            value= 0;
        }
        else if (_rand >= 1.0f && _rand <= 2.0f)
        {
            value= 1;
        }
        else
        {
            value= 2;
        }
        return value;
    } 

}

[Serializable]
public class BestFriendData
{
    public bool success;
    public FriendData data;
    public string msg;
}
[Serializable]
public class FriendData
{
    public int count;
    [NonReorderable]
    [SerializeField]
    public List<FriendsDetail> rows;
}
[Serializable]
public class FriendsDetail
{
    public int id;
    public string name;
    [NonReorderable]
    [SerializeField]
    public List<tempclassfordatafeed> userOccupiedAssets;
}
[Serializable]
public class tempclassfordatafeed
{
    [NonReorderable]
    [SerializeField]
    public SavingCharacterDataClass json;
    public string description;
    public bool isDeleted;
    public int createdBy;
    public DateTime createdAt;
    public DateTime updatedAt;

}
[Serializable]
public class FriendSpawnData
{
    public int id;
    public Transform friendObj;
    public Transform friendNameObj;
    public Transform friendPostBubbleObj;

}
[Serializable]
class OnlineFriends
{
    public bool success;
    public List<OnlineriendsData> data;
    public string msg;
}
[Serializable]
class OnlineriendsData
{
    public int id;
    public int userId;
    public bool isOnline;
    public bool isWorldJoin;
    public OnlineFriendsUser user;
    public RowList worldDetails;
}
[Serializable]
class OnlineFriendsUser
{
    public int id;
    public string name;
    public string avatar;
}