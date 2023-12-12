using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FriendHomeManager : MonoBehaviour
{
    [NonReorderable]
    [SerializeField]
    BestFriendData _friendsDataFetched;
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
                Debug.LogError("Successssss-----> ");

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
            Debug.LogError("ConstantsGod.AUTH_TOKEN "+ ConstantsGod.AUTH_TOKEN);
            Debug.LogError("XanaConstants.xanaConstants.userId " + XanaConstants.xanaConstants.userId);

            while (!www.isDone)
                yield return new WaitForSeconds(Time.deltaTime);
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                 Debug.LogError("FAILED "+www.downloadHandler.text);
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
    public List<SavingCharacterDataClass> userOccupiedAssets;
}