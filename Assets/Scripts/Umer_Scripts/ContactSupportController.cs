using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperSimple;
using BestHTTP.JSON.LitJson;
using System.Drawing;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System;
using System.Text;

public class ContactSupportController : MonoBehaviour
{
    public SNSSettingController SettingControllerRef;
    public SMTPServerInitializationData SmtpServerAuthData = new SMTPServerInitializationData();

    public async void SendEmail(string _emailSubjectText, string _emailBodyText)
    {
        SmtpServerAuthData = await GetSMTPServerAuthData();
        byte[] decodedPassBytes = Convert.FromBase64String(SmtpServerAuthData.data.password);

        EmailService.Instance.Initialize(new EmailService.Settings()
        {
            SmtpHost = "smtp.office365.com",
            SenderEmail = SmtpServerAuthData.data.smtp_host,
            SenderPassword = Encoding.UTF8.GetString(decodedPassBytes),
            SenderName = "X-Summit"
        });

        EmailService.Instance.SendPlainText(SmtpServerAuthData.data.receiver_email,
_emailSubjectText,
_emailBodyText, (success) =>
{
    Debug.Log("Email sent successfully " + success);
    SmtpServerAuthData.data = null;
    EmailService.Instance.Destroy();
    SettingControllerRef.ContactSupportPanelRef.SetActive(false);
    LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
});

        #region Formate to send email with attachments
        //Formate to send email with attachments
        //EmailService.Instance.SendPlainText(new EmailService.Recipient[] {
        //    new EmailService.Recipient() {
        //        Address = "recipient1@email.com",
        //        DisplayName = "Tester1"
        //    }
        //}, "SSEmail Example 2", "this email has attachments", new string[] {
        //    Application.streamingAssetsPath + "/SSEmailExample/TestImage1.png",
        //    Application.streamingAssetsPath + "/SSEmailExample/TestImage2.png"
        //}, (success) => {
        //    Debug.Log("SSEmail Example 2 sent " + success);
        //});
        #endregion

        #region Formate to send email with cc and bcc option
        //EmailService.Instance.SendHtml(new EmailService.Recipient[] {
        //    new EmailService.Recipient() {
        //        Address = "recipient1@email.com",
        //        DisplayName = "Tester1"
        //    },
        //    new EmailService.Recipient() {
        //        Address = "recipient2@email.com",
        //        DisplayName = "Tester2",
        //        Type = EmailService.RecipientType.CC
        //    },
        //    new EmailService.Recipient() {
        //        Address = "recipient3@email.com",
        //        Type = EmailService.RecipientType.Bcc
        //    }
        //}, "SSEmail Example 3", "this is <b>html</b> email, <br>這是第二行文字", (success) => {
        //    Debug.Log("SSEmail Example 3 sent " + success);
        //});
        #endregion
    }

    async Task<SMTPServerInitializationData> GetSMTPServerAuthData()
    {
        string url;
        url = ConstantsGod.API_BASEURL + ConstantsGod.SMTPSERVERAUTHDATA;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            await www.SendWebRequest();
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                www.Dispose();
                return null;
            }
            else
            {
                Debug.Log("Received Data: " + www.downloadHandler.text);
                SMTPServerInitializationData smtpServerAuthData = new SMTPServerInitializationData();
                smtpServerAuthData = JsonUtility.FromJson<SMTPServerInitializationData>(www.downloadHandler.text);
                www.Dispose();
                return smtpServerAuthData;
            }
        }
    }

}

[System.Serializable]
public class SMTPServerInitializationData
{
    public string success;
    public CredentialsData data;
    public string msg;
}
[System.Serializable]
public class CredentialsData
{
    public string smtp_host;
    public string password;
    public string receiver_email;
}