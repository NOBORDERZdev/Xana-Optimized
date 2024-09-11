# SSEmail #
A super simple email sending service by SMTP.

## Features ##
* Easy to use
* More efficient by reusing connection
* Full code available
* Works on multiple platforms: Windows, iOS, Android
* Multiple recipients, CC and BCC are supported
* Email display name is supported
* HTML body is supported
* Multiple attachments are supported
* Inline attachment is supported for Pro version
* Bytes attachment is supported for Pro version

## Quickstart ##
Replace the SMTP server and credentials settings, and recipient email address below: 
```c#
using UnityEngine;
using SuperSimple;

public class Quickstart : MonoBehaviour
{
    void Start()
    {
        EmailService.Instance.Initialize(new EmailService.Settings() {
            SmtpHost = "smtp.name.com",
            SenderEmail = "username@youraddress.com",
            SenderPassword = "yourpassword",
            SenderName = "SuperSimple EmailService Sender"
        });
        EmailService.Instance.SendPlainText("recipient1@email.com", "SSEmail Quickstart", "this is plain text email", (success) => {
            Debug.Log("SSEmail Quickstart sent " + success);
        });
    }
}
```
Or just checkout the Example.

## EmailService Script Reference ##
### Initialize ###
Initialize EmailService with the Settings below: 
* SmtpHost: The SMTP server hostname.
* SmtpPort: The SMTP server port, default is 587.
* EnableSSL: Enable SSL/TLS connection, default is true.
* SenderEmail: The sender's email address, and also the user name associated with the credentials.
* SenderPassword: The password for the user name associated with the credentials.
* SenderName: The sender's display name.

### SendOOO ###
Methods: 
* **SendPlainText**: Send email with plain text body.
* **SendHtml**: Send email with HTML body.
* **SendInline**: Send email with inline attachments, only supported for Pro version. Use `<img src="cid:attachment_idx">` in the HTML body, e.g. 
    ```html
    <img src="cid:0" /><br>
    <img src="cid:1" />
    ```

Parameters: 
* recipient: The recipient email address string.
* recipients: The Recipient array with Address, DisplayName, Type(RecipientType: To, CC, Bcc) data structures.
* subject: The email subject string.
* body: The email body string, it could be plain text or in HTML.
* fileNames: The string array of file path to be the attachments.
* byteFiles: The ByteFile array with Bytes, FileName data structures, to be the attachments and only supported for Pro version.
* onComplete: The SendCallback(bool success), called when the send operation ends.

## About Gmail SMTP ##
The most well-known limited free SMTP server. If you want to use your Gmail, simply configure SMTP server settings below: 
* SmtpHost: smtp.gmail.com
* SmtpPort: 587
* EnableSSL: true

If you are sure that you already configured the correct credentials but still receive **System.Net.Mail.SmtpException: 535-5.7.8 Username and Password not accepted.** error, it typically indicates that you did not enable Less Secure Apps of your Google account.  
You may try toggling your [Less Secure Apps](https://myaccount.google.com/lesssecureapps) setting on.  
Learn more about [Less Secure Apps & your Google account](https://support.google.com/accounts/answer/6010255).
