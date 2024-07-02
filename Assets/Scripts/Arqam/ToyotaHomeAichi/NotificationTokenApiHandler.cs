using Newtonsoft.Json;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static RegisterAsCompanyEmails;

public class NotificationTokenApiHandler : MonoBehaviour
{
    private  int _thaRoomId;     // Home cosulting room Id in Toyota World
    [SerializeField] int thaPageNumber;
    [SerializeField] int thaPageSize;


    private void Start()
    {
        FB_Notification_Initilizer.Instance.onReceiveToken += SendTokenToWeb;
        if (APIBasepointManager.instance.IsXanaLive)
        {
            _thaRoomId = 2;
        }
        else
        {
            _thaRoomId = 4;
        }
    }
    private void OnDisable()
    {
        FB_Notification_Initilizer.Instance.onReceiveToken -= SendTokenToWeb;
    }

    private void SendTokenToWeb(string token)
    {
        StartCoroutine(PostToken(token));
    }
    private IEnumerator PostToken(string token)
    {
        StringBuilder url = new StringBuilder();
        url.Append(ConstantsGod.API_BASEURL + ConstantsGod.toyotaNotificationApi);

        WWWForm form = new WWWForm();
        form.AddField("worldId", _thaRoomId);
        form.AddField("email", FB_Notification_Initilizer.Instance.toyotaUserEmail);
        form.AddField("userToken", token);

        UnityWebRequest request = UnityWebRequest.Post(url.ToString(), form);
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.Log("Error: " + request.error);
        else
        {
            Debug.Log("Token sent successfully");
        }
        request.Dispose();
    }
}
