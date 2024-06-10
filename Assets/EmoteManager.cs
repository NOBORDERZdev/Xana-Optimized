using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using static ReactionManager;

public class EmoteManager : MonoBehaviour
{
    public EmoteAnimationResponse EmoteServerData;
    public enum EmoteGroup {Dance, Moves, Reaction, Idle, Walk}
    public EmoteGroup EmoteGroupSelected = EmoteGroup.Moves;
    public void GetServerData()
    {
        StartCoroutine(GetEmoteServerData());
    }
    private IEnumerator GetEmoteServerData()
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
                        Debug.LogError(request.downloadHandler.text.ToString());
                    }
                }
            }
            else
            {
            
            }
            request.Dispose();

        }
    }
    public void OpenEmoteDialogUITabClick(int index)
    {
        if (EmoteGroupSelected == (EmoteGroup)index)
            return;

        EmoteGroupSelected = (EmoteGroup)index;
        OpenEmoteDialogUI();
    }
    public void OpenEmoteDialogUI()
    {
        List<EmoteAnimationList> items = EmoteServerData.data.animationList.FindAll(x => x.group == EmoteGroupSelected.ToString());
        EmoteReactionUIHandler.SetViewItemsEmote?.Invoke(items, EmoteReactionItemBtnHandler.ItemType.Emote);
    }
}

[System.Serializable]
public class EmoteAnimationList
{
    public int id;
    public string name;
    public string group;
    public string thumbnail;
    public string android_file;
    public string ios_file;
    public string description;
    public DateTime createdAt;
    public DateTime updatedAt;
}
[System.Serializable]
public class DataEmoteAnimationList
{
    [SerializeField]
    public List<EmoteAnimationList> animationList;
}
[System.Serializable]
public class EmoteAnimationResponse
{
    public bool success;
    public DataEmoteAnimationList data;
    public string msg;
}
