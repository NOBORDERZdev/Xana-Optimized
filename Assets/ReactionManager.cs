using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static EmoteManager;

public class ReactionManager : MonoBehaviour
{
    public enum ReactionGroup { Emote, Gestures, Others }
    public ReactionGroup ReactionGroupSelected = ReactionGroup.Emote;
    public ReactionAnimationDetails ReactionServerData;
    public void GetServerData()
    {
        StartCoroutine(GetEmoteServerData());
    }
    private IEnumerator GetEmoteServerData()
    {
        yield return new WaitForSeconds(5f);
        UnityWebRequest emoteWebRequest = UnityWebRequest.Get(ConstantsGod.API_BASEURL + 
            ConstantsGod.GetAllReactions + "/" + APIBasepointManager.instance.apiversion);

        using (UnityWebRequest request = UnityWebRequest.Get(emoteWebRequest.url))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            yield return request.SendWebRequest();
            ReactionServerData = JsonUtility.FromJson<ReactionAnimationDetails>(request.downloadHandler.text);

            if (!request.isHttpError && !request.isNetworkError)
            {
                if (request.error == null)
                {
                    if (ReactionServerData.success == true)
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
    public void OpenReactionDialogUITabClick(int index)
    {
        if (ReactionGroupSelected == (ReactionGroup)index)
            return;

        ReactionGroupSelected = (ReactionGroup)index;
        OpenReactionDialogUI();
    }
    public void OpenReactionDialogUI()
    {
        List<ReactionAnimationList> items = ReactionServerData.data.reactionList.FindAll(x => x.group == ReactionGroupSelected.ToString());
        Debug.LogError("----- >>> OpenReactionDialogUI " + items.Count);
        EmoteReactionUIHandler.SetViewItemsReaction?.Invoke(items, EmoteReactionItemBtnHandler.ItemType.Reaction);
    }
}
[System.Serializable]
public class ReactionAnimationList
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
public class ReactionAnimationData
{
    public List<ReactionAnimationList> reactionList;
}
[System.Serializable]
public class ReactionAnimationDetails
{
    public bool success;
    public ReactionAnimationData data;
    public string msg;
}