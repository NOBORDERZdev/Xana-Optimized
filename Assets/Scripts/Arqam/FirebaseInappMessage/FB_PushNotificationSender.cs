using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class FB_PushNotificationSender : MonoBehaviour
{
    [Space(5)]
    public const string serverKey = "AAAAk9GwOwQ:APA91bEC8oSce1_-Br-VCqagawpYZprcNazEad8Kf-JNJUyeevcvpCtCIFi03w5vbEJnCtt9_DKnPqAojPtJ-00dfCVfuQLJ_hOIsFpBosoPzTrkmO0zftytcB_u6Mt-tjcwA0Za0LPN"; //"14e5944ca3f569eb1f5266eb95a749f3506cd06d";
    public string recipientToken = "RECIPIENT_FCM_TOKEN";

    private void Start()
    {
        NFT_Holder_Manager.instance.pushNotification = this;
    }

    public void SendNotification()
    {
        string title = "Alert";
        string body = "I am in meeting.";
        StartCoroutine(CallAPISequentially(title, body)); ;
    }

    private IEnumerator CallAPISequentially(string title, string body)
    {
        int length = FB_Notification_Initilizer.Instance.fbTokens.Count;
        for (int i = 0; i < length; i++)
        {
            if (!string.IsNullOrEmpty(FB_Notification_Initilizer.Instance.fbTokens[i]) &&
                FB_Notification_Initilizer.Instance.fbTokens[i] != "")
            {
                recipientToken = FB_Notification_Initilizer.Instance.fbTokens[i];
                // Call your API for each item
                yield return StartCoroutine(SendNotification(title, body));
            }
        }
    }

    private IEnumerator SendNotification(string title, string body)
    {
        string url = "https://fcm.googleapis.com/fcm/send";
        string jsonBody = "{\"to\": \"" + recipientToken + "\", \"notification\": {\"title\": \"" + title + "\", \"body\": \"" + body + "\"}}";

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Authorization", "key=" + serverKey);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("Error sending notification: " + www.error);
            }
            else
            {
                // Debug.Log("Notification sent successfully!");
            }
            www.Dispose();
        }
    }
}
