using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReactionManager : MonoBehaviour
{
    public EmoteAnimationResponse EmoteServerData;
    private void Start()
    {
        StartCoroutine(GetEmoteServerData());
    }
    public IEnumerator GetEmoteServerData()
    {
        yield return new WaitForSeconds(5f);
        UnityWebRequest emoteWebRequest = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.GetAllAnimatons + "/" + APIBasepointManager.instance.apiversionForAnimation);

        using (UnityWebRequest request = UnityWebRequest.Get(emoteWebRequest.url))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            yield return request.SendWebRequest();
            EmoteServerData = JsonUtility.FromJson<EmoteAnimationResponse>(request.downloadHandler.text);

            if (!request.isHttpError && !request.isNetworkError)
            {
                if (request.error == null)
                {
                    if (EmoteServerData.success == true)
                    {


                    }
                }
            }
            else
            {

            }
            request.Dispose();

        }
    }
}
#region DATA
[System.Serializable]
public class ReactEmote
{
    public string imageName;
    public string thumb;
    public string mainImage;
}
[System.Serializable]
public class ReactGestures
{
    public string imageName;
    public string thumb;
    public string mainImage;
}
[System.Serializable]
public class ReactOthers
{
    public string imageName;
    public string thumb;
    public string mainImage;
}
[System.Serializable]
public class ReactionList
{
    public int id;
    public string name;
    public object android_bundle;
    public object ios_bundle;
    public string thumbnail;
    public int version;
    public string group;
    public string icon3d;
    public DateTime createdAt;
    public DateTime updatedAt;
}
[System.Serializable]
public class Data
{
    public List<ReactionList> reactionList;
}
[System.Serializable]
public class ReactionDetails
{
    public bool success;
    public Data data;
    public string msg;
}
#endregion