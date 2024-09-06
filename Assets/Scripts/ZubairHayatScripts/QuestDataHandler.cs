using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class QuestDataHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public static QuestDataHandler Instance;
    public GameObject MyQuestButton;

    [Header("*APIDATA*")]
    [SerializeField] private QuestTask _quest;
    private CompareQuestTask _compareQuestTask;
    [SerializeField] private ClaimReward _reward;

    [Header("*CurrentActiveTask*")]
    [SerializeField] private Activetask _currentTask;

    [Header("*UI-References*")]
    [SerializeField] private TextMeshProUGUI _totalReward;
    [SerializeField] private TextMeshProUGUI _totalQuestCompleted;
    [SerializeField] private TextMeshProUGUI _questTaskCompleted;
    [SerializeField] private Sprite _blackSpriteOnButton;
    [SerializeField] private Sprite _greySpriteOnButton;
    [SerializeField] private Image _totalQuesttaskFilledbar;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _claimButton;

    [Header("*GameObject-References*")]
    [SerializeField] private GameObject _taskPrefab;
    [SerializeField] private GameObject _questPanel;
    [SerializeField] private GameObject _rewardPopUp;
    [SerializeField] private GameObject _taskPopUp;
    [SerializeField] private GameObject _parentobject;
    [SerializeField] private RectTransform _container;
    [SerializeField] private List<QuestTaskDetails> _questTaskDetails;

    [Header("*Variables*")]
    [SerializeField] private int _countCompletedTasks;
    [SerializeField] private int _currentTaskIndex;
    private bool _currentActiveTask = false;
    private int _pageNumber = 1;
    private int _pageSize = 10;
    private bool _questDataLoaded = false;
    private bool _compareQuestDataLoaded = false;
    private bool _taskComplete = false;
    private string msg;

    #region Mono Functions
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(_parentobject);
            Invoke("CallingAPI", 3f);
        }
        else
        {
            Destroy(_parentobject);
        }
    }
    private void Start()
    {
        _backButton.onClick.AddListener(() => OpenAndCloseQuestPanel(false));
    }
    private void OnDisable()
    {
        UserPostFeature.OnPostButtonPressed -= TaskProgession;
        FindFriendWithNameItem.OnFollowButtonPressed -= TaskProgession;
        PlayerSelfieController.OnSelfieButtonPressed -= TaskProgession;
    }
    #endregion

    #region API Calling and UI mentaining 
    private async void ClaimMyQuestReward()
    {
        if (_quest.data.questData.id == 0)
            return;

        string url = ConstantsGod.API_BASEURL + ConstantsGod.ClaimQuestRewardCheque + _quest.data.questData.id;
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
                _reward = JsonUtility.FromJson<ClaimReward>(response.downloadHandler.text.ToString());
                ClaimDataResposne();
                updateCountQuestbar();
            }

        }
        catch (System.Exception ex)
        {

        }
        response.Dispose();
    }
    private async void GetQuestTaskDataFromAPI()
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.GetAllTaskDataFromCurrentQuest + _pageNumber + "/" + _pageSize;
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
                _quest = JsonUtility.FromJson<QuestTask>(response.downloadHandler.text.ToString());
            }

        }
        catch (System.Exception ex)
        {

        }
        response.Dispose();
        _questDataLoaded = true;
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
                _compareQuestTask = JsonUtility.FromJson<CompareQuestTask>(response.downloadHandler.text.ToString());
            }

        }
        catch (System.Exception ex)
        {

        }
        response.Dispose();
        _compareQuestDataLoaded = true;
        CompareAndUpdateData();
    }
    private void CompareAndUpdateData() // continue 
    {
        if (_compareQuestDataLoaded && _questDataLoaded)
        {
            for (int i = 0; i < _quest.data.count; i++)
            {
                for (int j = 0; j < _compareQuestTask.data.Length; j++)
                {
                    if (_quest.data.rows[i].id == _compareQuestTask.data[j].questTaskId)
                    {
                        // update all task quest list 
                        _quest.data.rows[i].actionCount = _compareQuestTask.data[j].actionCount;
                        _quest.data.rows[i].status = _compareQuestTask.data[j].isComplete;
                        _quest.data.rows[i].isActive = _compareQuestTask.data[j].isActive;
                    }
                }
            }
            _compareQuestTask = null;
            TransferData();
        }
    }
    private void TransferData()
    {
        for (int i = 0; i < _quest.data.count; i++)
        {
            string str = _quest.data.rows[i].taskName.ToLower();
            str.Trim();
            switch (str)
            {
                case "selfie":
                    _quest.data.rows[i]._taskType = TaskData.taskType.Selfie;
                    if (TakeReferenceOfActiveTask(i))
                    {
                        PlayerSelfieController.OnSelfieButtonPressed += TaskProgession;
                    }
                    break;
                case "follow":
                    _quest.data.rows[i]._taskType = TaskData.taskType.Follow;
                    if (TakeReferenceOfActiveTask(i))
                    {
                        FindFriendWithNameItem.OnFollowButtonPressed += TaskProgession;
                    }
                    break;
                case "folllow"://dummy remove this 
                    _quest.data.rows[i]._taskType = TaskData.taskType.Follow;
                    if (TakeReferenceOfActiveTask(i))
                    {
                        FindFriendWithNameItem.OnFollowButtonPressed += TaskProgession;
                    }
                    break;
                case "post":
                    _quest.data.rows[i]._taskType = TaskData.taskType.Post;
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

        if (_quest.data.rows[i].isActive)
        {

            _currentTask.task_id = _quest.data.rows[i].id;
            _currentTask._taskType = (Activetask.taskType)_quest.data.rows[i]._taskType;
            _currentTask.numberOfTimesTaskPrefrom = _quest.data.rows[i].actionCount;
            _currentTask.isActive = _quest.data.rows[i].isActive;
            _currentTask.isCompleted = _quest.data.rows[i].status;
            _currentTaskIndex = i;
            value = true;
        }

        return value;
    }
    private void InitQuestData(int i)
    {

        _totalReward.text = _quest.data.questData.rewards.ToString();
        GameObject task = Instantiate(_taskPrefab, _container) as GameObject;
        QuestTaskDetails taskDetails = task.GetComponent<QuestTaskDetails>();
        taskDetails.TaskDescription.text = _quest.data.rows[i].description;
        StartCoroutine(ImagesDownload(_quest.data.rows[i].taskIcon, taskDetails.TaskIcon));
        if (_quest.data.rows[i].status)
        {
            taskDetails.TaskButtonImage.sprite = _blackSpriteOnButton;
            taskDetails.TaskButtonText.text = "Done";
            taskDetails.TaskButtonText.color = Color.black;
            taskDetails.TaskButton.interactable = false;
            _countCompletedTasks++;
        }
        else if (_currentTask.task_id == _quest.data.rows[i].id)
        {
            taskDetails.TaskButtonImage.sprite = _greySpriteOnButton;
            taskDetails.TaskButtonText.text = "Active";
            taskDetails.TaskButtonText.color = Color.white;
            taskDetails.TaskButton.interactable = false;
            _currentActiveTask = true;
        }
        else
        {
            taskDetails.TaskButtonImage.sprite = _greySpriteOnButton;
            taskDetails.TaskButtonText.text = "Start";
            taskDetails.TaskButton.onClick.AddListener(() => OnclickQuestStartButton());
            if (_currentActiveTask)
                taskDetails.TaskButton.interactable = false;
        }
        _questTaskDetails.Add(taskDetails);
        taskDetails.Task_id = _quest.data.rows[i].id;
        // CheckForCurrentlyActiveTask();
    }
    private IEnumerator ImagesDownload(string url, RawImage icon)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture downloadTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            icon.texture = downloadTexture;
        }
    }
    private IEnumerator SaveTaskInformationVivaApi(int task_id, int actionCount, bool isComplete, bool isActive)
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

        www.Dispose();
    }
    private IEnumerator QuestRewardClaimedByUser(int quest_id, int coins)
    {
        string token = ConstantsGod.AUTH_TOKEN;
        WWWForm form = new WWWForm();
        form.AddField("questId", quest_id);
        form.AddField("rewards", coins.ToString());
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
        www.Dispose();
    }
    #endregion

    #region Quest Task related functions 
    public void CollectQuestReward()
    {
        _claimButton.interactable = false;
        _rewardPopUp.SetActive(true);
        StartCoroutine(QuestRewardClaimedByUser(_quest.data.questData.id, _quest.data.questData.rewards));
    }
    public void OpenAndCloseQuestPanel(bool value)
    {
        _questPanel.SetActive(value);
    }
    private void updateCountQuestbar()
    {
        if (_countCompletedTasks == 0)
        {
            return;
        }
        else
        {
            float fillAmount = (float)_countCompletedTasks / _quest.data.count;
            _totalQuesttaskFilledbar.fillAmount = fillAmount;
            _totalQuestCompleted.text = _countCompletedTasks.ToString() + "/" + _quest.data.count.ToString();
            if (!_reward.data.isClaimed && _quest.data.count == _countCompletedTasks)
            {
                _claimButton.interactable = true;
                print("All task Completed !");
                // add sparkling animation
            }
        }
    }
    private void ClaimDataResposne()
    {
        if (!_reward.data.isClaimed && _quest.data.count == _countCompletedTasks)
        {
            _claimButton.interactable = true;
            print("All task Completed ! v2");
            // add sparkling animation
        }
    }
    public void OnclickQuestStartButton()
    {
        GameObject temp = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.transform.parent.gameObject;
        int id = temp.GetComponent<QuestTaskDetails>().Task_id;
        CurrentlyTaskInProgress(id);

    }
    private void CurrentlyTaskInProgress(int id)
    {
        for (int i = 0; i < _questTaskDetails.Count; i++)
        {
            if (_questTaskDetails[i].Task_id == id)
            {
                _currentTask.task_id = _quest.data.rows[i].id;
                _currentTask._taskType = (Activetask.taskType)_quest.data.rows[i]._taskType;
                _currentTask.numberOfTimesTaskPrefrom = _quest.data.rows[i].actionCount;
                _currentTask.isActive = true;
                _currentTaskIndex = i;

                _questTaskDetails[i].TaskButton.GetComponent<Image>().sprite = _greySpriteOnButton;
                _questTaskDetails[i].TaskButtonText.text = "Active";
                _questTaskDetails[i].TaskButtonText.color = Color.white;
                _questTaskDetails[i].TaskButton.interactable = false;
                _questTaskDetails[i].TaskButton.onClick.RemoveListener(() => OnclickQuestStartButton());

                switch (_currentTask._taskType)
                {
                    case Activetask.taskType.Selfie:
                        PlayerSelfieController.OnSelfieButtonPressed += TaskProgession;
                        StartCoroutine(SaveTaskInformationVivaApi(_quest.data.rows[i].id, _quest.data.rows[i].actionCount, false, true));
                        break;
                    case Activetask.taskType.Follow:
                        FindFriendWithNameItem.OnFollowButtonPressed += TaskProgession;
                        StartCoroutine(SaveTaskInformationVivaApi(_quest.data.rows[i].id, _quest.data.rows[i].actionCount, false, true));
                        break;
                    case Activetask.taskType.Post:
                        UserPostFeature.OnPostButtonPressed += TaskProgession;
                        StartCoroutine(SaveTaskInformationVivaApi(_quest.data.rows[i].id, _quest.data.rows[i].actionCount, false, true));
                        break;
                }

                msg = "Your Xana Quest Task [" + _questTaskDetails[i].TaskDescription + "] is Started";
            }
        }

        DisableEnableAllStartFunctions(false);
    }
    private void DisableEnableAllStartFunctions(bool value)
    {
        for (int i = 0; i < _questTaskDetails.Count; i++)
        {
            if (_quest.data.rows[i].isActive == false && _quest.data.rows[i].status == false)
            {
                _questTaskDetails[i].TaskButton.interactable = value;
            }
        }
    }
    private void UpdateQuestUITaskList()
    {
        _questTaskDetails[_currentTaskIndex].TaskButton.GetComponent<Image>().sprite = _blackSpriteOnButton;
        _questTaskDetails[_currentTaskIndex].TaskButtonText.text = "Done";
        _questTaskDetails[_currentTaskIndex].TaskButtonText.color = Color.black;
        _countCompletedTasks++;
        DisableEnableAllStartFunctions(true);
        updateCountQuestbar();
    }
    #endregion

    #region Custom Funtions
    public void QuestButton()
    {
        if (_quest.data.count > 0)
        {
            MyQuestButton.GetComponent<Button>().onClick.AddListener(() => OpenAndCloseQuestPanel(true));
        }
        else
        {
            MyQuestButton.SetActive(false);
        }
    }
    public void CheckForTaskCDomplete()
    {
        if (_taskComplete)
        {
            _questTaskCompleted.text = msg;
            _taskComplete = false;
            Invoke("AddDelayInPopUp", 3f);
        }
    }

    private void AddDelayInPopUp()
    {
        _taskPopUp.SetActive(true);
    }

    private void TaskProgession()
    {
        if (_currentTask.numberOfTimesTaskPrefrom > 0)
        {
            _currentTask.numberOfTimesTaskPrefrom = _currentTask.numberOfTimesTaskPrefrom - 1;
            if (_currentTask.numberOfTimesTaskPrefrom == 0)
            {
                _taskComplete = true;
                _quest.data.rows[_currentTaskIndex].status = true;
                _quest.data.rows[_currentTaskIndex].isActive = false;
                msg = "Your Xana Quest Task [" + _quest.data.rows[_currentTaskIndex].description + "] is Completed";
                StartCoroutine(SaveTaskInformationVivaApi(_currentTask.task_id, 0, true, false));
                UnsubscribeEvents();
                UpdateQuestUITaskList();
                Scene currentScene = SceneManager.GetActiveScene();
                string sceneName = currentScene.name;
                if (sceneName == "Home")
                {
                    _taskPopUp.SetActive(true);
                    _taskComplete = false;
                }
            }
            else
            {
                StartCoroutine(SaveTaskInformationVivaApi(_currentTask.task_id, _currentTask.numberOfTimesTaskPrefrom, false, true));
            }
        }
    }
    private void UnsubscribeEvents()
    {
        switch (_currentTask._taskType)
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
    private void CallingAPI()
    {
        GetQuestTaskDataFromAPI();
        GetComapreQuestTaskDataFromAPI();
        Invoke("ClaimMyQuestReward", 2f);
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

[System.Serializable]
public class ClaimReward
{
    public bool success;
    public ClaimRewardData data;
    public string msg;
}
[System.Serializable]
public class ClaimRewardData
{
    public bool isClaimed;
}

#endregion


