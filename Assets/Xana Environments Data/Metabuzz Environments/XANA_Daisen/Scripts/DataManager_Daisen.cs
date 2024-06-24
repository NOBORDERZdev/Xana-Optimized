using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager_Daisen : MonoBehaviour
{
    private GameObject player;

    // API configs
    [SerializeField] private string id;
    [SerializeField] private string url;
    [SerializeField] private UIController uiController;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PhotonLocalPlayer");
        StartCoroutine(CheckPoint());
    }

    IEnumerator CheckPoint()
    {
        WWWForm form = new WWWForm();
        form.AddField("command", "getPoint");
        form.AddField("id", id);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        string point = www.downloadHandler.text;
        uiController.SetPointUI(point);

        www.Dispose();
    }
}
