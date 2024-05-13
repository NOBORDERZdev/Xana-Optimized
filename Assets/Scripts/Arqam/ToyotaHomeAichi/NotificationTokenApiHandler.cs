using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NotificationTokenApiHandler : MonoBehaviour
{
    // Update user Token API
    //https://api-test.xana.net/toyotaAichiWorlds/save-user-token
    private const int worldId = 4;     // Home cosulting room Id in Toyota World

    private void Start()
    {
        FB_Notification_Initilizer.Instance.onReceiveToken += SendTokenToWeb;
    }
    private void OnDisable()
    {
        FB_Notification_Initilizer.Instance.onReceiveToken -= SendTokenToWeb;
    }

    private void SendTokenToWeb(string token)
    {
        StartCoroutine(PostToken(token));
    }
    IEnumerator PostToken(string token)
    {
        StringBuilder url = new StringBuilder();
        url.Append(ConstantsGod.API_BASEURL + ConstantsGod.toyotaNotificationApi);

        WWWForm form = new WWWForm();
        form.AddField("worldId", worldId);
        form.AddField("email", FB_Notification_Initilizer.Instance.toyotaUserEmail);
        form.AddField("userToken", token);

        UnityWebRequest request = UnityWebRequest.Post(url.ToString(), form);
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Error: " + request.error);
        else
            Debug.Log("Response: " + request.downloadHandler.text);

        request.Dispose();
    }


}
