using Newtonsoft.Json;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static RegisterAsCompanyEmails;

public class NotificationTokenApiHandler : MonoBehaviour
{
    // Update user Token API
    //https://api-test.xana.net/toyotaAichiWorlds/save-user-token
    private const int THARoomId = 4;     // Home cosulting room Id in Toyota World
    [SerializeField] int thaPageNumber;
    [SerializeField] int thaPageSize;


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
        form.AddField("worldId", THARoomId);
        form.AddField("email", FB_Notification_Initilizer.Instance.toyotaUserEmail);
        form.AddField("userToken", token);

        UnityWebRequest request = UnityWebRequest.Post(url.ToString(), form);
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Error: " + request.error);
        else
        {
            GetUpdatedToken();
            //Debug.Log("Response: " + request.downloadHandler.text);
        }
        request.Dispose();
    }


    private async void GetUpdatedToken()
    {
        StringBuilder ApiURL = new StringBuilder();
        ApiURL.Append(ConstantsGod.API_BASEURL + ConstantsGod.toyotaEmailApi + THARoomId + "/" + thaPageNumber + "/" + thaPageSize);
        //Debug.Log("API URL is : " + ApiURL.ToString());
        using (UnityWebRequest request = UnityWebRequest.Get(ApiURL.ToString()))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            await request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("Error is" + request.error);
            }
            else
            {
                StringBuilder data = new StringBuilder();
                data.Append(request.downloadHandler.text);
                THAEmailDataResponse json = JsonConvert.DeserializeObject<THAEmailDataResponse>(data.ToString());
                for (int i = 0; i < json.data.rows.Count; i++)
                {
                    FB_Notification_Initilizer.Instance.companyEmails.Add(json.data.rows[i].email);
                    FB_Notification_Initilizer.Instance.fbTokens.Add(json.data.rows[i].userToken);
                }
            }
        }
    }
}
