using UnityEngine;
using SuperSimple;

public class EmailServiceExample : MonoBehaviour
{
    void Start()
    {
        EmailService.Instance.Initialize(new EmailService.Settings() {
            SmtpHost = "smtp.name.com",
            SenderEmail = "username@youraddress.com",
            SenderPassword = "yourpassword",
            SenderName = "超簡單 EmailService Sender"
        });

        EmailService.Instance.SendPlainText("recipient1@email.com", "SSEmail Example 1", 
@"this is plain text email, 
這是第二行文字", (success) => {
            Debug.Log("SSEmail Example 1 sent " + success);
        });

        EmailService.Instance.SendPlainText(new EmailService.Recipient[] {
            new EmailService.Recipient() {
                Address = "recipient1@email.com",
                DisplayName = "Tester1"
            }
        }, "SSEmail Example 2", "this email has attachments", new string[] {
            Application.streamingAssetsPath + "/SSEmailExample/TestImage1.png",
            Application.streamingAssetsPath + "/SSEmailExample/TestImage2.png"
        }, (success) => {
            Debug.Log("SSEmail Example 2 sent " + success);
        });

        EmailService.Instance.SendHtml(new EmailService.Recipient[] {
            new EmailService.Recipient() {
                Address = "recipient1@email.com",
                DisplayName = "Tester1"
            },
            new EmailService.Recipient() {
                Address = "recipient2@email.com",
                DisplayName = "Tester2",
                Type = EmailService.RecipientType.CC
            },
            new EmailService.Recipient() {
                Address = "recipient3@email.com",
                Type = EmailService.RecipientType.Bcc
            }
        }, "SSEmail Example 3", "this is <b>html</b> email, <br>這是第二行文字", (success) => {
            Debug.Log("SSEmail Example 3 sent " + success);
        });
    }

    void OnDestroy() {
        EmailService.Instance.Destroy();
    }
}
