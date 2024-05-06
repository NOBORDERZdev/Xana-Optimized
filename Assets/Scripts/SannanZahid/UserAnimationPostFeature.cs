using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UserAnimationPostFeature : MonoBehaviour
{
    [NonReorderable]
    [SerializeField]
    public MoodInfo _moodInfo;
    [SerializeField]
    public string _moodstr;
    ActorBehaviour.Category _selectedCategory;
    public Button postButton;
    [HideInInspector]
    public TextMeshProUGUI postButtonText;
    void Start()
    {
        _selectedCategory = ActorBehaviour.Category.Fun;
        InstantiateMoodTab();
       StartCoroutine( BuildMoodDialog());
        postButtonText=postButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    string PrepareApiURL()
    {
       return ConstantsGod.API_BASEURL + "/item/app-emojis";
    }
    IEnumerator BuildMoodDialog()
    {
        yield return new WaitForSeconds(1f);
        string finalAPIURL = PrepareApiURL();
       // LoadingHandler.Instance.worldLoadingScreen.SetActive(true);
        StartCoroutine(FetchUserMapFromServer(finalAPIURL, (isSucess) =>
        {
            if (isSucess)
            {
                InstantiateMoodsToUIHolder();
            }
            else
            {
                /* if (++CallBackCheck > 17)
                 {
                     LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
                     CallBackCheck = 0;
                     return;
                 }*/
                StartCoroutine(BuildMoodDialog());
            }
        }));
    }
    IEnumerator FetchUserMapFromServer(string apiURL, Action<bool> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL))
        {
            while (ConstantsGod.AUTH_TOKEN == "AUTH_TOKEN")
                yield return new WaitForSeconds(0.5f);

            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
           // yield return www;

            while (!www.isDone)
                yield return new WaitForSeconds(Time.deltaTime);
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
               // Debug.LogError(www.downloadHandler.text);
                callback(false);
            }
            else
            {
              //  Debug.LogError(www.downloadHandler.text);
                _moodInfo = JsonUtility.FromJson<MoodInfo>(www.downloadHandler.text);
                _moodstr = www.downloadHandler.text;
                callback(true);
            }
            www.Dispose();
        }
    }
    public Transform MoodDataTab;
    public List<Transform> MoodTabList = new List<Transform>();
    public int InstantiateMoodTabCount = 10;
    public void InstantiateMoodTab()
    { 
        for(int i = 0; i < InstantiateMoodTabCount; i++)
            MoodTabList.Add(Instantiate(MoodDataTab.gameObject, MoodDataTab.parent).transform);
    }
    void InstantiateMoodsToUIHolder()
    {
        string checkString = _selectedCategory.ToString();
        List<MoodRowList> selectedMoods = new List<MoodRowList>();
        foreach (MoodRowList item in _moodInfo.data.rows)
        {
            if (item.name.Contains(checkString))
                selectedMoods.Add(item);
        }
        foreach (Transform item in MoodTabList)
            item.gameObject.SetActive(false);
        for (int i = 0; i < selectedMoods.Count; i++)
        {
            string finalName = selectedMoods[i].name.Replace(checkString, "");
            finalName = finalName.Replace("-01", "");
            MoodTabList[i].GetComponent<MoodTabItemView>().InitItem(finalName, selectedMoods[i].emoji_link,this);
            MoodTabList[i].gameObject.SetActive(true);
        }
    }
    public void ChangeMoodView(int index)
    {
       _selectedCategory = (ActorBehaviour.Category)index;
      //  Debug.LogError("_selectedCategory ---> " + _selectedCategory.ToString());
        InstantiateMoodsToUIHolder();
    }
    public string MoodSelected = default;
    public int NoOfAnimations = default;
    public void SetMood(string moodName)
    {
        MoodSelected = _selectedCategory.ToString() + " " + moodName ;
      //  Debug.LogError("moodName ---> " + MoodSelected);
        NoOfAnimations = GameManager.Instance.ActorManager.GetNumberofIdleAnimations(MoodSelected);
        if(NoOfAnimations == 1)
        {
            GameManager.Instance.moodManager.ViewMoodActionAnimation(MoodSelected + " Idle", MoodSelected, GameManager.Instance.mainCharacter.GetComponent<Actor>().overrideController, GameManager.Instance.mainCharacter.transform.GetComponent<Animator>());
        }
        else
        {
            GameManager.Instance.moodManager.ViewMoodActionAnimation(MoodSelected + " Idle "+ UnityEngine.Random.Range(1,3), MoodSelected, GameManager.Instance.mainCharacter.GetComponent<Actor>().overrideController, GameManager.Instance.mainCharacter.transform.GetComponent<Animator>());
        }
        if(!postButton.interactable)
        {
            postButton.interactable = true;
            postButtonText.color= Color.white;
        }
    }

    public void SetMood(string moodName, Actor _animator)
    {
       // GameManager.Instance.moodManager.ViewMoodActionAnimation(MoodSelected , MoodSelected, _animator.overrideController);

        NoOfAnimations = GameManager.Instance.ActorManager.GetNumberofIdleAnimations(moodName);

        _animator.SetNewBehaviour(GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == moodName));
        if (NoOfAnimations == 1)
        {
            print("________________________ if ");
            GameManager.Instance.moodManager.ViewMoodActionAnimation(moodName + " Idle", moodName, _animator.overrideController, _animator.transform.GetComponent<Animator>());
        }
        else
        {
            print("________________________ else ");
            GameManager.Instance.moodManager.ViewMoodActionAnimation(moodName + " Idle " + UnityEngine.Random.Range(1, 3), moodName, _animator.overrideController, _animator.transform.GetComponent<Animator>());
        }
    }
}
[System.Serializable]
public class MoodInfo
{
    public bool success;
    public MoodClass data;
    public string msg;
}
[System.Serializable]
public class MoodClass
{
    public int count;
    public List<MoodRowList> rows;
}
[System.Serializable]
public class MoodRowList
{
    public string id;
    public string name;
    public string emoji_link;
    public string createdAt;
    public string updatedAt;
}