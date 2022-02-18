using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class EmailSend : MonoBehaviour
{
    public bool sendEmailInProgress = false;

    private static EmailSend Instance;
    public static EmailSend instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject newinstance = new GameObject("EmailSend");
                Instance = newinstance.AddComponent<EmailSend>();
                newinstance.transform.parent = MyUtilities.createAndSetToScriptFolder(false).transform;
            }

            return Instance;
        }

        set
        {
            if (Instance == null)
                Instance = value;
        }
    }


    public void sendBugReportMail(string bugReportText)
    {
        StartCoroutine(sendBugReportMailCoroutine(bugReportText));
    }

    private IEnumerator sendBugReportMailCoroutine(string bugReportText)
    {
        sendEmailInProgress = true;
        yield return null;

        MailMessage mail = new MailMessage();

        mail.From = new MailAddress("wrbstudio.info@gmail.com");
        mail.To.Add("wrbstudio.info@gmail.com");
        mail.Subject = "BugReport: Be the SPACE INVADERS (" + Application.version + ")";
        mail.Body = "BugReport \n" + 
                    "Game: " + Application.productName + "\n" +
                    "Version: " + Application.version + "\n" +
                    "Date: " + DateTime.Now + "\n\n" + 
                    "Report: \n" + bugReportText;

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new NetworkCredential("wrbstudio.info@gmail.com", "wilbicrunkpcuriw");
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
        smtpServer.SendAsync(mail, "Request Email");
        sendEmailInProgress = false;
    }

}
