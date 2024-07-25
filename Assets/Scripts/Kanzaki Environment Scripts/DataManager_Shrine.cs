using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DataManager_Shrine : MonoBehaviour
{
    //private GameObject player;

    // API configs
    [SerializeField] private string id;
    [SerializeField] private string userName = "--";
    [SerializeField] private string url;

    // UI Configs
    [SerializeField] private GameObject worshipFailUI;
    [SerializeField] private UIController_Shine uIController_Shine;
    [SerializeField] private GameObject coinParticle;
    [SerializeField] private Transform altar;


    void Start() 
    {
        ConstantsHolder.xanaConstants.comingFrom = ConstantsHolder.ComingFrom.None;
        worshipFailUI.GetComponentInChildren<Button>().onClick.AddListener(closeWorshipFailUI);
        if (ConstantsHolder.userId != null)
        {
            id = ConstantsHolder.userId;
            //StartCoroutine(CheckPoint());

        }
        if (ConstantsHolder.userId != null && ConstantsHolder.uniqueUserName != null)
        {
            id = ConstantsHolder.userId;
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
        Debug.Log("User ID: " + id);
        Debug.Log("uniqueUserName: " + userName);
        WWWForm form = new WWWForm();
        form.AddField("command", "initData");
        form.AddField("id", id);
        form.AddField("userName", userName);

        using UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            StartCoroutine(InitPlayerDB(id, userName));
            Debug.LogError("Network Error initData" + www.downloadHandler.text);

        }
        else
        {
            Debug.LogError("initData" + www.downloadHandler.text);
            string point = www.downloadHandler.text;
            if (point == "There is no player Data") point = "0";
            uIController_Shine.SetPointUI(point);
            //www.Dispose();
        }
    }
    public IEnumerator CommunicateWithDB(string id) {

        WWWForm form = new WWWForm();
        form.AddField("command", "checkPoint");
        form.AddField("id", id);
        Debug.LogError("User ID checkPoint: " + id);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        if (www.downloadHandler.text == "success") {
            uIController_Shine.GetWorshipGameUI().gameObject.SetActive(true);
            //TODO: 동전 효과 넣기
            coinParticle.GetComponent<ParticleSystem>().Play();
            Transform player = ReferencesForGamePlay.instance.MainPlayerParent.transform;
            if (player != null)
            {
                Vector3 directionToAltar = (altar.position - player.transform.position).normalized;
                Vector3 newPosition = altar.position - directionToAltar * 2.0f;
                player.transform.position = newPosition;

                Vector3 lookDirection = altar.position - player.transform.position;
                float angleY = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
                player.transform.rotation = Quaternion.Euler(0, angleY, 0);
            }
            else
            {
                Debug.LogWarning("Player not found!");
            }
        } else if (www.downloadHandler.text == "fail") {
            worshipFailUI.SetActive(true);
        }
        www.Dispose();
        StartCoroutine(CheckPoint());        
    }

    IEnumerator CheckPoint() {
        WWWForm form = new WWWForm();
        form.AddField("command", "getPoint");
        form.AddField("id", id);
        Debug.Log("User ID CheckPoint getPoint: " + id);

        using UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            StartCoroutine(CheckPoint());
            Debug.LogError("ConnectionError getPoint");
        }
        else
        {
            string point = www.downloadHandler.text;
            if (point == "There is no player Data") point = "0";

            Debug.Log("getPoint" + point);

            uIController_Shine.SetPointUI(point);
            //www.Dispose();
        }
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
