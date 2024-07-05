using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DataManager_Shrine : MonoBehaviour
{
    private GameObject player;

    // API configs
    [SerializeField] private string id;
    [SerializeField] private string userName = "";
    [SerializeField] private string url;

    // UI Configs
    [SerializeField] private GameObject worshipFailUI;
    [SerializeField] private UIController_Shine uIController_Shine;
    [SerializeField] private GameObject coinParticle;

    void Start() {
        //player = GameObject.FindGameObjectWithTag("PhotonLocalPlayer");
        worshipFailUI.GetComponentInChildren<Button>().onClick.AddListener(closeWorshipFailUI);
        if (ConstantsHolder.userId != null)
        {
            id = ConstantsHolder.userId;
            userName = ConstantsHolder.uniqueUserName;
        }
        StartCoroutine(CheckPoint());
        StartCoroutine(InitPlayerDB(id, userName));
    }
    public void getPlayerData() => StartCoroutine(CommunicateWithDB(id));

    public IEnumerator InitPlayerDB(string id, string userName)
    {
        WWWForm form = new WWWForm();
        form.AddField("command", "initData");
        form.AddField("id", id);
        form.AddField("userName", userName);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        Debug.Log("initData" + www.downloadHandler.text);
        uIController_Shine.SetPointUI(www.downloadHandler.text);
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
        Debug.Log("getPoint" + point);

        uIController_Shine.SetPointUI(point);

        www.Dispose();
    }

    public void closeWorshipFailUI() {
        worshipFailUI.SetActive(false);
    }
}
