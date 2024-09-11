using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperSimple;
using BestHTTP.JSON.LitJson;

public class ContactSupportController : MonoBehaviour
{
    public SNSSettingController SettingControllerRef;

    void Start()
    {
        EmailService.Instance.Initialize(new EmailService.Settings()
        {
            SmtpHost = "smtp.office365.com",
            SenderEmail = "x-summit@outlook.com",
            SenderPassword = "Noborderz@12345",
            SenderName = "X-Summit"
        });
    }


    void OnDestroy()
    {
        EmailService.Instance.Destroy();
    }

    public void SendEmail(string _emailSubjectText, string _emailBodyText)
    {
        EmailService.Instance.SendPlainText("umernoborderz@gmail.com",
                    _emailSubjectText,
                    _emailBodyText, (success) =>
                {
                    Debug.Log("SSEmail Example 1 sent " + success);
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
}
