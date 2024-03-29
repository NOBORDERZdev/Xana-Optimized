using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class FriendHomeManager : MonoBehaviour
{
    public Transform FriendAvatarPrefab, NameTagFriendAvatarPrefab, PostBubbleFriendAvatarPrefab;
    [NonReorderable]
    [SerializeField]
    BestFriendData _friendsDataFetched;
    public bool SpawnFriendsAgain;
    public List<FriendSpawnData> SpawnFriendsObj = new List<FriendSpawnData>();

    public GameObject menuLightObj;
    public GameObject profileLightingObj;
    private void OnEnable()
    {
        StartCoroutine(BuildMoodDialog());
        SocketController.instance.updateFriendPostDelegate += UpdateFriendPost;

        MainSceneEventHandler.OnSucessFullLogin += SpawnFriends;
    }
    private void OnDisable()
    {
        if (SocketController.instance != null)
            SocketController.instance.updateFriendPostDelegate -= UpdateFriendPost;

        MainSceneEventHandler.OnSucessFullLogin -= SpawnFriends;
    }

    public void SpawnFriends()
    {
        if(SpawnFriendsAgain)
        {
            Debug.Log("Spawning Friends Again");
            SpawnFriendsAgain = false;
            StartCoroutine(BuildMoodDialog());
        }
    }
    string PrepareApiURL()
    {
        return ConstantsGod.API_BASEURL + "/social/get-close-friends/" + XanaConstants.userId;
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
        Transform CreatedFriend = Instantiate(FriendAvatarPrefab, FriendAvatarPrefab.parent).transform;
        yield return null; // Wait for the next frame to continue execution

        Transform CreatedFriendPostBubble = Instantiate(PostBubbleFriendAvatarPrefab, PostBubbleFriendAvatarPrefab.parent).transform;
        Transform CreatedNameTag = Instantiate(NameTagFriendAvatarPrefab, NameTagFriendAvatarPrefab.parent).transform;
        CreatedNameTag.GetComponent<FollowUser>().targ = CreatedFriend;
        CreatedNameTag.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = friend.name;
        CreatedFriend.GetComponent<Actor>().NameTagHolderObj = CreatedNameTag;
        CreatedFriend.gameObject.SetActive(true);
        CreatedFriend.GetComponent<Actor>().Init(GameManager.Instance.ActorManager.actorBehaviour[GetPostRandomDefaultAnim()]);
        if (friend.userOccupiedAssets.Count > 0 && friend.userOccupiedAssets[0].json != null)
        {
            CreatedFriend.GetComponent<AvatarController>().InitializeFrndAvatar(friend.userOccupiedAssets[0].json,CreatedFriend.gameObject);
        }
        else
        {
            int _rand = UnityEngine.Random.Range(0, 13);
            CreatedFriend.GetComponent<AvatarController>().DownloadRandomFrndPresets(_rand);
        }
        CreatedFriend.GetComponent<PlayerPostBubbleHandler>().InitObj(CreatedFriendPostBubble,
            CreatedFriendPostBubble.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>());
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
                    frds.friendPostBubbleObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = data.text_post;
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