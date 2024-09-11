using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;
using System.IO;
using UnityEngine;

namespace SuperSimple
{
    public partial class EmailService
    {
        private static readonly Lazy<EmailService> mLazyInstance = new Lazy<EmailService>(() => new EmailService());
        public static EmailService Instance { get { return mLazyInstance.Value; } }

        public class Settings
        {
            public string SmtpHost;
            public int StmpPort = 587;
            /// <summary>
            /// enable SSL/TLS, default is true
            /// </summary>
            public bool EnableSSL = true;
            public string SenderEmail;
            public string SenderPassword;
            public string SenderName;
        }
        private Settings m_Settings;

        private SmtpClient m_SmtpClient;

        public enum RecipientType
        {
            To,
            CC,
            Bcc
        }
        public class Recipient
        {
            public string Address;
            public string DisplayName;
            public RecipientType Type;
        }

        public delegate void SendCallback(bool success);

        private class MailData
        {
            public MailMessage Message;
            public SendCallback Callback;
        }
        private Queue<MailData> m_QueuedMailData;
        private bool m_IsSending = false;

        public void Initialize(Settings settings)
        {
            if (m_SmtpClient != null)
            {
                Debug.LogWarning("EmailService is already initialized!");
                return;
            }

            m_Settings = new Settings();
            m_Settings.SmtpHost = settings.SmtpHost;
            m_Settings.StmpPort = settings.StmpPort;
            m_Settings.EnableSSL = settings.EnableSSL;
            m_Settings.SenderEmail = settings.SenderEmail;
            m_Settings.SenderPassword = settings.SenderPassword;
            m_Settings.SenderName = settings.SenderName;

            m_SmtpClient = new SmtpClient(m_Settings.SmtpHost, m_Settings.StmpPort);

            m_SmtpClient.EnableSsl = m_Settings.EnableSSL;
            if (m_Settings.EnableSSL)
            {
                // to accept a self signed certificate
                ServicePointManager.ServerCertificateValidationCallback = this.OnCertificateValidate;
            }
            
            if (!string.IsNullOrEmpty(m_Settings.SenderPassword))
            {
                m_SmtpClient.Credentials = new NetworkCredential(m_Settings.SenderEmail, m_Settings.SenderPassword) as ICredentialsByHost;
            }

            m_SmtpClient.SendCompleted += this.OnSendCompleted;
        }

        private bool IsInitialized()
        {
            if (m_SmtpClient == null)
            {
                Debug.LogWarning("EmailService should be initialized first!");
                return false;
            }

            return true;
        }

        public void Destroy()
        {
            if (!this.IsInitialized()) return;

            m_SmtpClient.Dispose();
            m_SmtpClient = null;
            m_Settings = null;
        }

        public void SendPlainText(string recipient, string subject, string body, SendCallback onComplete = null)
        {
            this.Send(new Recipient[] {
                new Recipient() {
                    Address = recipient
                }
            }, subject, body, false, null, onComplete);
        }

        public void SendHtml(string recipient, string subject, string body, SendCallback onComplete = null)
        {
            this.Send(new Recipient[] {
                new Recipient() {
                    Address = recipient
                }
            }, subject, body, true, null, onComplete);
        }

        public void SendPlainText(Recipient[] recipients, string subject, string body, SendCallback onComplete = null)
        {
            this.Send(recipients, subject, body, false, null, onComplete);
        }

        public void SendHtml(Recipient[] recipients, string subject, string body, SendCallback onComplete = null)
        {
            this.Send(recipients, subject, body, true, null, onComplete);
        }

        private Attachment[] CreateAttachments(string[] fileNames)
        {
            List<Attachment> attachments = new List<Attachment>();
            if (fileNames != null)
            {
                foreach (var fileName in fileNames)
                {
                    if (File.Exists(fileName))
                    {
                        attachments.Add(new Attachment(fileName));
                    }
                    else
                    {
                        Debug.LogError($"\"{fileName}\" not found.");
                        return null;
                    }
                }
            }
            return attachments.ToArray();
        }

        public void SendPlainText(Recipient[] recipients, string subject, string body, string[] fileNames, SendCallback onComplete = null)
        {
            var attachments = this.CreateAttachments(fileNames);
            if (attachments == null) // got error
                onComplete?.Invoke(false);
            else
                this.Send(recipients, subject, body, false, attachments, onComplete);
        }

        public void SendHtml(Recipient[] recipients, string subject, string body, string[] fileNames, SendCallback onComplete = null)
        {
            var attachments = this.CreateAttachments(fileNames);
            if (attachments == null) // got error
                onComplete?.Invoke(false);
            else
                this.Send(recipients, subject, body, true, attachments, onComplete);
        }

        public void Send(Recipient[] recipients, string subject, string body, bool isHtml = false, Attachment[] attachments = null, SendCallback onComplete = null)
        {
            if (!this.IsInitialized()) return;

            try
            {
                MailMessage msg = new MailMessage();
                msg.From = string.IsNullOrEmpty(m_Settings.SenderName) ? new MailAddress(m_Settings.SenderEmail) : new MailAddress(m_Settings.SenderEmail, m_Settings.SenderName, System.Text.Encoding.UTF8);

                foreach (var recipient in recipients)
                {
                    var mailAddress = string.IsNullOrEmpty(recipient.DisplayName) ? new MailAddress(recipient.Address) : new MailAddress(recipient.Address, recipient.DisplayName, System.Text.Encoding.UTF8);
                    switch (recipient.Type)
                    {
                        case RecipientType.Bcc:
                        {
                            msg.Bcc.Add(mailAddress);
                        }
                        break;

                        case RecipientType.CC:
                        {
                            msg.CC.Add(mailAddress);
                        }
                        break;

                        case RecipientType.To:
                        default:
                        {
                            msg.To.Add(mailAddress);
                        }
                        break;
                    }
                }

                msg.Subject = subject;
                msg.SubjectEncoding = System.Text.Encoding.UTF8;
                msg.Body = body;
                msg.BodyEncoding = System.Text.Encoding.UTF8;
                msg.IsBodyHtml = isHtml;

                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        msg.Attachments.Add(attachment);
                    }
                }

                if (!m_IsSending)
                {
                    Debug.Log("EmailService is sending...");
                    m_SmtpClient.SendAsync(msg, onComplete);
                    m_IsSending = true;
                    msg.Dispose();
                }
                else
                {
                    if (m_QueuedMailData == null) m_QueuedMailData = new Queue<MailData>();

                    m_QueuedMailData.Enqueue(new MailData() {
                        Message = msg,
                        Callback = onComplete
                    });
                }
            }
            catch (Exception error)
            {
                Debug.LogError(error);
                onComplete?.Invoke(false);
            }
        }

        /// <summary>
        /// a RemoteCertificateValidationCallback, to accept a self signed certificate
        /// <para>refer to https://learn.microsoft.com/en-us/previous-versions/office/developer/exchange-server-2010/dd633677(v=exchg.80)#example</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private bool OnCertificateValidate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (X509ChainStatus status in chain.ChainStatus)
                    {
                        if (certificate.Subject == certificate.Issuer && status.Status == X509ChainStatusFlags.UntrustedRoot)
                        {
                            // Self-signed certificates with an untrusted root are valid.
                            continue;
                        }
                        else
                        {
                            if (status.Status != X509ChainStatusFlags.NoError)
                            {
                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                return false;
                            }
                        }
                    }
                }

                // When processing reaches this line, the only errors in the certificate chain are 
                // untrusted root errors for self-signed certificates. These certificates are valid, so return true.
                return true;
            }
            else
            {
                // In all other cases, return false.
                return false;
            }
        }
    
        /// <summary>
        /// a SendCompletedEventHandler
        /// </summary>
        /// <param name="sender">System.Net.Mail.SmtpClient</param>
        /// <param name="e"></param>
        private void OnSendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Debug.Log("EmailService send completed.");

            bool success = false;
            if (e.Cancelled || e.Error != null)
            {
                if (e.Cancelled) // ??? still false when call SendAsyncCancel
                    Debug.Log("Email is cancelled.");
                if (e.Error != null)
                    Debug.LogError(e.Error);
            }
            else
            {
                success = true;
            }

            if (e.UserState != null)
            {
                (e.UserState as SendCallback)?.Invoke(success);
            }

            var go = GameObject.FindObjectOfType<MonoBehaviour>(); // todo: improve
            go?.StartCoroutine(SendNext());
        }

        private IEnumerator SendNext()
        {
            yield return new WaitForEndOfFrame();
            
            m_IsSending = false;

            if (m_QueuedMailData != null && m_QueuedMailData.Count > 0)
            {
                var data = m_QueuedMailData.Dequeue();
                try
                {
                    Debug.Log("EmailService is sending...");
                    m_SmtpClient.SendAsync(data.Message, data.Callback);
                    m_IsSending = true;
                    data.Message.Dispose();
                }
                catch (Exception error)
                {
                    Debug.LogError(error);
                    data.Callback?.Invoke(false);
                }
            }
        }
    }
}