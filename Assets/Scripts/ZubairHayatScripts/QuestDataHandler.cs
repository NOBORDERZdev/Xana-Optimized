using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class QuestDataHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public static QuestDataHandler instance;
    [Header("*APIDATA*")]
    [SerializeField] QuestTask quest;
    CompareQuestTask CompareQuestTask;
    [Header("*CurrentActiveTask*")]
    [SerializeField] Activetask currentTask;
    [Header("*UI-References*")]
    [SerializeField] TextMeshProUGUI totalReward;
    [SerializeField] TextMeshProUGUI totalQuestCompleted;
    [SerializeField] Sprite blackSpriteOnButton;
    [SerializeField] Sprite greySpriteOnButton;
    [SerializeField] Image totalQuesttaskFilledbar;
    [SerializeField] Button backButton;
    [SerializeField] Button claimButton;

    [Header("*GameObject-References*")]
    [SerializeField] GameObject taskPrefab;
    public GameObject questButton;
    [SerializeField] GameObject questPanel;
    [SerializeField] GameObject rewardPopUp;
    [SerializeField] GameObject parentobject;
    [SerializeField] RectTransform container;
    [SerializeField] List<QuestTaskDetails> questTaskDetails;

    [Header("*Variables*")]
    [SerializeField] int countCompletedTasks;
    [SerializeField] int currentTaskIndex;



    private int pageNumber = 1;
    private int pageSize = 10;

    private bool questDataLoaded = false;
    private bool compareQuestDataLoaded = false;

    private string msg;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(parentobject);
        }
        else
        {
            Destroy(parentobject);
        }
    }

    void Start()
    {
        Invoke("CallingAPI", 5f);
        backButton.onClick.AddListener(() => OpenAndCloseQuestPanel(false));
    }

    private void OnDisable()
    {
        UserPostFeature.OnPostButtonPressed -= TaskProgession;
        FindFriendWithNameItem.OnFollowButtonPressed -= TaskProgession;
        PlayerSelfieController.OnSelfieButtonPressed -= TaskProgession;
    }

    private void CallingAPI()
    {
        GetQuestTaskDataFromAPI();
        GetComapreQuestTaskDataFromAPI();
    }

    #region API Calling and UI mentaining 
    private async void GetQuestTaskDataFromAPI()
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.GetAllTaskDataFromCurrentQuest + pageNumber + "/" + pageSize;
        UnityWebRequest response = UnityWebRequest.Get(url);
        response.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        try
        {
            await response.SendWebRequest();
            if (response.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(response.error);
            }
            else
            {
                quest = JsonUtility.FromJson<QuestTask>(response.downloadHandler.text.ToString());
            }

        }
        catch (System.Exception ex)
        {

        }
        response.Dispose();
        questDataLoaded = true;
        CompareAndUpdateData();
    }
    private async void GetComapreQuestTaskDataFromAPI()
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.UpdateComapreQuestTaskDataPerformance;
        UnityWebRequest response = UnityWebRequest.Get(url);
        response.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        try
        {
            await response.SendWebRequest();
            if (response.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(response.error);
            }
            else
            {
                CompareQuestTask = JsonUtility.FromJson<CompareQuestTask>(response.downloadHandler.text.ToString());
            }

        }
        catch (System.Exception ex)
        {

        }
        response.Dispose();
        compareQuestDataLoaded = true;
        CompareAndUpdateData();
    }
    private void CompareAndUpdateData() // continue 
    {
        if (compareQuestDataLoaded && questDataLoaded)
        {
            for (int i = 0; i < quest.data.count; i++)
            {
                for (int j = 0; j < CompareQuestTask.data.Length; j++)
                {
                    if (quest.data.rows[i].id == CompareQuestTask.data[j].questTaskId)
                    {
                        // update all task quest list 
                        quest.data.rows[i].actionCount = CompareQuestTask.data[j].actionCount;
                        quest.data.rows[i].status = CompareQuestTask.data[j].isComplete;
                        quest.data.rows[i].isActive = CompareQuestTask.data[j].isActive;
                    }
                }
            }
            CompareQuestTask = null;
            TransferData();
        }
    }
    private void TransferData()
    {
        for (int i = 0; i < quest.data.count; i++)
        {
            string str = quest.data.rows[i].taskName.ToLower();
            str.Trim();
            switch (str)
            {
                case "selfie":
                    quest.data.rows[i]._taskType = TaskData.taskType.Selfie;
                    if (TakeReferenceOfActiveTask(i))
                    {
                        PlayerSelfieController.OnSelfieButtonPressed += TaskProgession;
                    }
                    break;
                case "follow ":
                    quest.data.rows[i]._taskType = TaskData.taskType.Follow;
                    if (TakeReferenceOfActiveTask(i))
                    {
                        FindFriendWithNameItem.OnFollowButtonPressed += TaskProgession;
                    }
                    break;
                case "post":
                    quest.data.rows[i]._taskType = TaskData.taskType.Post;
                    if (TakeReferenceOfActiveTask(i))
                    {
                        UserPostFeature.OnPostButtonPressed += TaskProgession;
                    }
                    break;
            }
            // update current task object 
            InitQuestData(i);
        }
    }
    private bool TakeReferenceOfActiveTask(int i)
    {
        bool value = false;

        if (quest.data.rows[i].isActive)
        {

            currentTask.task_id = quest.data.rows[i].id;
            currentTask._taskType = (Activetask.taskType)quest.data.rows[i]._taskType;
            currentTask.numberOfTimesTaskPrefrom = quest.data.rows[i].actionCount;
            currentTask.isActive = quest.data.rows[i].isActive;
            currentTask.isCompleted = quest.data.rows[i].status;
            currentTaskIndex = i;
            value = true;
        }

        return value;
    }
    private void InitQuestData(int i)
    {

        totalReward.text = quest.data.questData.rewards.ToString();
        GameObject task = Instantiate(taskPrefab, container) as GameObject;
        QuestTaskDetails taskDetails = task.GetComponent<QuestTaskDetails>();
        taskDetails.taskDescription.text = quest.data.rows[i].description;
        StartCoroutine(ImagesDownload(quest.data.rows[i].taskIcon, taskDetails.taskIcon));
        if (quest.data.rows[i].status)
        {
            taskDetails.taskButtonImage.sprite = blackSpriteOnButton;
            taskDetails.taskButtonText.text = "Done";
            taskDetails.taskButtonText.color = Color.white;
            taskDetails.taskButton.interactable = false;
            countCompletedTasks++;
        }
        else if (currentTask.task_id == quest.data.rows[i].id)
        {
            taskDetails.taskButtonImage.sprite = blackSpriteOnButton;
            taskDetails.taskButtonText.text = "Active";
            taskDetails.taskButtonText.color = Color.white;
        }
        else
        {
            taskDetails.taskButtonImage.sprite = greySpriteOnButton;
            taskDetails.taskButtonText.text = "Start";
            taskDetails.taskButton.onClick.AddListener(() => OnclickQuestStartButton());

        }
        questTaskDetails.Add(taskDetails);
        taskDetails.task_id = quest.data.rows[i].id;
        CheckForCurrentlyActiveTask();
    }
    IEnumerator ImagesDownload(string url, RawImage icon)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture downloadTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            icon.texture = downloadTexture;
        }
    }
    IEnumerator SaveTaskInformationVivaApi(int task_id, int actionCount, bool isComplete, bool isActive)
    {
        string token = ConstantsGod.AUTH_TOKEN;
        WWWForm form = new WWWForm();
        form.AddField("questTaskId", task_id);
        form.AddField("actionCount", actionCount);
        form.AddField("isComplete", isComplete.ToString());
        form.AddField("isActive", isActive.ToString());
        UnityWebRequest www;
        www = UnityWebRequest.Post(ConstantsGod.API_BASEURL + ConstantsGod.UpdateQuestTaskDataPerformance, form);
        www.SetRequestHeader("Authorization", token);
        www.SendWebRequest();
        while (!www.isDone)
        {
            yield return null;
        }
        if (www.result != UnityWebRequest.Result.ConnectionError && www.result != UnityWebRequest.Result.ProtocolError)
            Debug.Log("Update Task Details : ");
        else
            Debug.Log("Error Update Task Details" + www.error);
    }
    IEnumerator QuestRewardClaimedByUser(int Quest_id, int Coins)
    {
        string token = ConstantsGod.AUTH_TOKEN;
        WWWForm form = new WWWForm();
        form.AddField("questId", Quest_id);
        form.AddField("rewards", Coins.ToString());
        UnityWebRequest www;
        www = UnityWebRequest.Post(ConstantsGod.API_BASEURL + ConstantsGod.ClaimQuestReward, form);
        www.SetRequestHeader("Authorization", token);
        www.SendWebRequest();
        while (!www.isDone)
        {
            yield return null;
        }
        if (www.result != UnityWebRequest.Result.ConnectionError && www.result != UnityWebRequest.Result.ProtocolError)
            Debug.Log("Claimed By user : ");
        else
            Debug.Log("Error in Claiming Reward" + www.error);
    }
    public void QuestButton()
    {
        if (quest.data.count > 0)
        {
            questButton.GetComponent<Button>().onClick.AddListener(() => OpenAndCloseQuestPanel(true));
        }
        else
        {
            questButton.SetActive(false);
        }
    }
    #endregion

    #region Quest Task related functions 
    private void CheckForCurrentlyActiveTask()
    {
        for (int i = 0; i < questTaskDetails.Count; i++)
        {
            if (currentTask.task_id != -1)
            {
                questTaskDetails[i].taskButton.interactable = true;
            }
        }

        updateCountQuestbar();
    }
    private void updateCountQuestbar()
    {
        if (countCompletedTasks == 0)
        {
            return;
        }
        else
        {
            float fillAmount = (float)countCompletedTasks / quest.data.count;
            totalQuesttaskFilledbar.fillAmount = fillAmount;
            totalQuestCompleted.text = countCompletedTasks.ToString() + "/" + quest.data.count.ToString();
            if (fillAmount == 1)
            {
                claimButton.interactable = true;
                print("All task Completed !");
                // add sparkling animation
            }
        }
    }
    public void OnclickQuestStartButton()
    {
        GameObject temp = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.transform.parent.gameObject;
        int id = temp.GetComponent<QuestTaskDetails>().task_id;
        CurrentlyTaskInProgress(id);

    }
    private void CurrentlyTaskInProgress(int id)
    {
        for (int i = 0; i < questTaskDetails.Count; i++)
        {
            if (questTaskDetails[i].task_id == id)
            {
                currentTask.task_id = quest.data.rows[i].id;
                currentTask._taskType = (Activetask.taskType)quest.data.rows[i]._taskType;
                currentTask.numberOfTimesTaskPrefrom = quest.data.rows[i].actionCount;
                currentTask.isActive = true;

                questTaskDetails[i].taskButton.GetComponent<Image>().sprite = blackSpriteOnButton;
                questTaskDetails[i].taskButtonText.text = "Active";
                questTaskDetails[i].taskButtonText.color = Color.white;
                questTaskDetails[i].taskButton.onClick.RemoveListener(() => OnclickQuestStartButton());

                switch (currentTask._taskType)
                {
                    case Activetask.taskType.Selfie:
                        UserPostFeature.OnPostButtonPressed += TaskProgession;
                        StartCoroutine(SaveTaskInformationVivaApi(quest.data.rows[i].id, quest.data.rows[i].actionCount, false, true));
                        break;
                    case Activetask.taskType.Follow:
                        UserPostFeature.OnPostButtonPressed += TaskProgession;
                        StartCoroutine(SaveTaskInformationVivaApi(quest.data.rows[i].id, quest.data.rows[i].actionCount, false, true));
                        break;
                    case Activetask.taskType.Post:
                        UserPostFeature.OnPostButtonPressed += TaskProgession;
                        StartCoroutine(SaveTaskInformationVivaApi(quest.data.rows[i].id, quest.data.rows[i].actionCount, false, true));
                        break;
                }

                msg = "Your Xana Quest Task [" + questTaskDetails[i].taskDescription + "] is Started";
                SNSNotificationHandler.Instance.ShowNotificationMsg(msg);
            }
            else
            {
                questTaskDetails[i].taskButton.interactable = false;
            }
        }
    }
    private void UpdateQuestUITaskList()
    {
        questTaskDetails[currentTaskIndex].taskButtonText.text = "Done";
        for (int i = 0; i < questTaskDetails.Count; i++)
        {
            questTaskDetails[i].taskButton.interactable = true;
        }
        countCompletedTasks++;
        updateCountQuestbar();
    }
    public void OpenAndCloseQuestPanel(bool value)
    {
        questPanel.SetActive(value);
    }
    public void CollectQuestReward()
    {
        rewardPopUp.SetActive(true);
        StartCoroutine(QuestRewardClaimedByUser(quest.data.questData.id, quest.data.questData.rewards));
    }
    #endregion

    #region Custom Funtions
    private void TaskProgession()
    {
        if (currentTask.numberOfTimesTaskPrefrom > 0)
        {
            currentTask.numberOfTimesTaskPrefrom = currentTask.numberOfTimesTaskPrefrom - 1;
            if (currentTask.numberOfTimesTaskPrefrom == 0)
            {
                print("Complete");
                quest.data.rows[currentTaskIndex].status = true;
                quest.data.rows[currentTaskIndex].isActive = false;
                msg = "Your Xana Quest Task [" + quest.data.rows[currentTaskIndex].description + "] is Completed";
                SNSNotificationHandler.Instance.ShowNotificationMsg(msg);
                StartCoroutine(SaveTaskInformationVivaApi(currentTask.task_id, 0, true, false));
                UnsubscribeEvents();
                UpdateQuestUITaskList();
            }
            else
            {
                StartCoroutine(SaveTaskInformationVivaApi(currentTask.task_id, currentTask.numberOfTimesTaskPrefrom, false, true));
                print("remove 1");

            }
        }
    }

    private void UnsubscribeEvents()
    {
        switch (currentTask._taskType)
        {
            case Activetask.taskType.Selfie:
                PlayerSelfieController.OnSelfieButtonPressed -= TaskProgession;
                break;
            case Activetask.taskType.Post:
                UserPostFeature.OnPostButtonPressed -= TaskProgession;
                break;
            case Activetask.taskType.Follow:
                FindFriendWithNameItem.OnFollowButtonPressed -= TaskProgession;
                break;
        }
    }
    #endregion

}


#region API Json Format 

[System.Serializable]
public class Activetask
{
    public enum taskType
    {
        Selfie,
        Post,
        Follow
    }
    public taskType _taskType;
    public int task_id;
    public int numberOfTimesTaskPrefrom;
    public bool isActive;
    public bool isCompleted;
}

[System.Serializable]
public class QuestTask //parent 
{
    public bool success;
    public TaskCount data;
    string msg;

}

[System.Serializable]
public class QuestData
{
    public int id;
    public string name;
    public string startTime;
    public string endTime;
    public int rewards;
}

[System.Serializable]
public class TaskCount
{
    [Header("*---Quest---*")]
    public QuestData questData;
    [Header("*---Quest Task---*")]
    public TaskData[] rows;
    public int count;
}
[System.Serializable]
public class TaskData
{
    public enum taskType
    {
        Selfie,
        Post,
        Follow
    }
    public taskType _taskType;
    public int id;
    public int questId;
    public int actionCount;
    public string taskIcon;
    public string description;
    public string taskName;
    public bool status = false;
    public bool isActive = false;
}

// comparing with completed and active tasks 
[System.Serializable]
public class CompareQuestTask //parent 
{
    public bool success;
    public ComapreTaskCount[] data;
    public string msg;
}
[System.Serializable]
public class ComapreTaskCount
{
    public int id;
    public int questTaskId;
    public int actionCount;
    public bool isComplete;
    public bool isActive;
}
#endregion


