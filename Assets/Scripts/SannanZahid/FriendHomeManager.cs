using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FriendHomeManager : MonoBehaviour
{
    public Transform FriendAvatarPrefab, NameTagFriendAvatarPrefab;
    [NonReorderable]
    [SerializeField]
    BestFriendData _friendsDataFetched;

    public List<Transform> SpawnFriendsObj = new List<Transform>();
    void Start()
    {
        StartCoroutine(BuildMoodDialog());
    }

    string PrepareApiURL()
    {
        return "https://api-test.xana.net/social/get-close-friends/"+ XanaConstants.xanaConstants.userId;
    }
    IEnumerator BuildMoodDialog()
    {
       // while (XanaConstants.xanaConstants.userId == "")
         //   yield return new WaitForSeconds(0.5f);

        while (ConstantsGod.AUTH_TOKEN == "AUTH_TOKEN")
            yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(1f);
        string finalAPIURL = PrepareApiURL();
        StartCoroutine(FetchUserMapFromServer(finalAPIURL, (isSucess) =>
        {
            if (isSucess)
            {
               // Debug.LogError("Successssss-----> ");
                foreach(FriendsDetail friend in _friendsDataFetched.data.rows)
                {
                   // for(int i=0;i<20;i++)
                    {

                        Transform CreatedFriend = Instantiate(FriendAvatarPrefab, FriendAvatarPrefab.parent).transform;
                        Transform CreatedNameTag = Instantiate(NameTagFriendAvatarPrefab, NameTagFriendAvatarPrefab.parent).transform;
                        CreatedNameTag.GetComponent<FollowUser>().targ = CreatedFriend;
                        CreatedNameTag.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = friend.name;
                        CreatedFriend.GetComponent<Actor>().NameTagHolderObj = CreatedNameTag;
                        CreatedFriend.gameObject.SetActive(true);
                        CreatedFriend.GetComponent<Actor>().Init(GameManager.Instance.ActorManager.actorBehaviour[0]);
                        CreatedFriend.GetComponent<FriendAvatarController>().IntializeAvatar(friend.userOccupiedAssets[0].json);
                        SpawnFriendsObj.Add(CreatedFriend);
                        SpawnFriendsObj.Add(CreatedNameTag);
                       // Debug.LogError("Friend Spawned ----->  " + friend.id);
                        GameManager.Instance.PostManager.GetComponent<UserPostFeature>().GetLatestPostOfFriend(
                            friend.id, 
                            CreatedFriend.GetComponent<PlayerPostBubbleHandler>(), 
                            CreatedFriend.GetComponent<Actor>().overrideController
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
            // yield return www;
           // Debug.LogError("ConstantsGod.AUTH_TOKEN "+ ConstantsGod.AUTH_TOKEN);
           // Debug.LogError("XanaConstants.xanaConstants.userId " + XanaConstants.xanaConstants.userId);

            while (!www.isDone)
                yield return new WaitForSeconds(Time.deltaTime);
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                // Debug.LogError("FAILED "+www.downloadHandler.text);
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
        foreach(Transform SpawnFriendsObjref in SpawnFriendsObj)
        {
            SpawnFriendsObjref.gameObject.SetActive(flag);
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