using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DataManager_Shrine : MonoBehaviour
{
    private GameObject player;

    // API configs
    [SerializeField] private string id;
    [SerializeField] private string userName = "--";
    [SerializeField] private string url;

    // UI Configs
    [SerializeField] private GameObject worshipFailUI;
    [SerializeField] private UIController_Shine uIController_Shine;
    [SerializeField] private GameObject coinParticle;

    void Start() {

        worshipFailUI.GetComponentInChildren<Button>().onClick.AddListener(closeWorshipFailUI);
        if (ConstantsHolder.userId != null)
        {
            id = ConstantsHolder.userId;
            StartCoroutine(CheckPoint());

        }
        if (ConstantsHolder.uniqueUserName != null)
        {
            userName = ConstantsHolder.uniqueUserName;
            StartCoroutine(InitPlayerDB(id, userName));
        }
        else
        {
            StartCoroutine(IERequestGetUserDetails());
        }

    }

    public void GetPlayerData() => StartCoroutine(CommunicateWithDB(id));

    public IEnumerator InitPlayerDB(string id, string userName)
    {
        Debug.LogError("User ID: " + id);
        Debug.LogError("uniqueUserName: " + userName);
        WWWForm form = new WWWForm();
        form.AddField("command", "initData");
        form.AddField("id", id);
        form.AddField("userName", userName);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        Debug.LogError("initData" + www.downloadHandler.text);
        string point = www.downloadHandler.text;
        if (point == "There is no player Data") point = "0";
        uIController_Shine.SetPointUI(point);
        www.Dispose();
    }
    public IEnumerator CommunicateWithDB(string id) {
        WWWForm form = new WWWForm();
        form.AddField("command", "checkPoint");
        form.AddField("id", id);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        if (www.downloadHandler.text == "success") {
            uIController_Shine.GetWorshipGameUI().gameObject.SetActive(true);
            //TODO: 동전 효과 넣기
            coinParticle.GetComponent<ParticleSystem>().Play();
        } else if (www.downloadHandler.text == "fail") {
            worshipFailUI.SetActive(true);
        }
        www.Dispose();
        StartCoroutine(CheckPoint());        
    }

    IEnumerator CheckPoint(){
        WWWForm form = new WWWForm();
        form.AddField("command", "getPoint");
        form.AddField("id", id);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        string point = www.downloadHandler.text;
        if (point == "There is no player Data") point = "0";

        Debug.LogError("getPoint" + point);

        uIController_Shine.SetPointUI(point);

        www.Dispose();
    }

    public void closeWorshipFailUI() {
        worshipFailUI.SetActive(false);
    }
    public IEnumerator IERequestGetUserDetails()
    {
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetUserDetails)))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }
            if (www.isNetworkError || www.isHttpError)
            {
                StartCoroutine(IERequestGetUserDetails());
            }
            else
            {
                GetUserDetailRoot tempMyProfileDataRoot = JsonUtility.FromJson<GetUserDetailRoot>(www.downloadHandler.text.ToString());
                ConstantsHolder.uniqueUserName = tempMyProfileDataRoot.data.userProfile.username;
                ConstantsHolder.userId = tempMyProfileDataRoot.data.id.ToString();
                id = ConstantsHolder.userId;
                userName = ConstantsHolder.uniqueUserName;

                StartCoroutine(InitPlayerDB(id, userName));
            }
        }
    }
}
