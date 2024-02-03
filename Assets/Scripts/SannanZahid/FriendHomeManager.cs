using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FriendHomeManager : MonoBehaviour
{
    public Transform FriendAvatarPrefab, NameTagFriendAvatarPrefab, PostBubbleFriendAvatarPrefab;
    [NonReorderable]
    [SerializeField]
    BestFriendData _friendsDataFetched;

    public List<FriendSpawnData> SpawnFriendsObj = new List<FriendSpawnData>();

    private void OnDisable()
    {
        if (SocketController.instance != null)
            SocketController.instance.updateFriendPostDelegate -= UpdateFriendPost;
    }



    void Start()
    {
        StartCoroutine(BuildMoodDialog());
        SocketController.instance.updateFriendPostDelegate += UpdateFriendPost;
    }
    string PrepareApiURL()
    {
        return ConstantsGod.API_BASEURL + "/social/get-close-friends/" + XanaConstants.xanaConstants.userId;
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
                foreach (FriendsDetail friend in _friendsDataFetched.data.rows)
                {
                    if (SpawnFriendsObj.Find(x => x.id == friend.id) == null)
                    {
                        FriendSpawnData FriendSpawn = new FriendSpawnData();
                        Transform CreatedFriend = Instantiate(FriendAvatarPrefab, FriendAvatarPrefab.parent).transform;
                        Transform CreatedFriendPostBubble = Instantiate(PostBubbleFriendAvatarPrefab, PostBubbleFriendAvatarPrefab.parent).transform;
                        Transform CreatedNameTag = Instantiate(NameTagFriendAvatarPrefab, NameTagFriendAvatarPrefab.parent).transform;
                        CreatedNameTag.GetComponent<FollowUser>().targ = CreatedFriend;
                        CreatedNameTag.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = friend.name;
                        CreatedFriend.GetComponent<Actor>().NameTagHolderObj = CreatedNameTag;
                        CreatedFriend.gameObject.SetActive(true);
                        CreatedFriend.GetComponent<Actor>().Init(GameManager.Instance.ActorManager.actorBehaviour[0]);
                        if (friend.userOccupiedAssets.Count > 0 && friend.userOccupiedAssets[0].json != null)
                        {
                            CreatedFriend.GetComponent<FriendAvatarController>().IntializeAvatar(friend.userOccupiedAssets[0].json);
                        }
                        else
                        {
                            CreatedFriend.GetComponent<FriendAvatarController>().SetAvatarClothDefault(CreatedFriend.gameObject);
                        }
                        CreatedFriend.GetComponent<PlayerPostBubbleHandler>().InitObj(CreatedFriendPostBubble,
                            CreatedFriendPostBubble.GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>());
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
                    }
                }

            }
            else
            {
                StartCoroutine(BuildMoodDialog());
            }
        }));
    }
    IEnumerator FetchUserMapFromServer(string apiURL, Action<bool> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return new WaitForSeconds(Time.deltaTime);
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                callback(false);
            }
            else
            {
                _friendsDataFetched = JsonUtility.FromJson<BestFriendData>(www.downloadHandler.text);
                callback(true);
            }
            www.Dispose();
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
    }
    public void AddFriendToHome()
    {
        StartCoroutine(BuildMoodDialog());
    }


    private void UpdateFriendPost(ReceivedFriendPostData data)
    {
        print("________________________ " + data.creatorId);
        print("________________________ " + data.text_mood);
        foreach (var frds in SpawnFriendsObj)
        {
            if (frds.id == int.Parse(data.creatorId))
            {
                if (!string.IsNullOrEmpty(data.text_post) && !data.text_post.Equals("null"))
                {
                    frds.friendPostBubbleObj.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = data.text_post;
                    frds.friendPostBubbleObj.transform.GetChild(0).gameObject.SetActive(true);
                }

                if (!string.IsNullOrEmpty(data.text_mood) && !data.text_mood.Equals("null"))
                {
                    print("________________________ " + data.text_mood);
                    GameManager.Instance.PostManager.GetComponent<UserAnimationPostFeature>().SetMood(data.text_mood, frds.friendObj.GetComponent<Actor>());
                    // Update Animation Here
                }
            }
        }

       
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