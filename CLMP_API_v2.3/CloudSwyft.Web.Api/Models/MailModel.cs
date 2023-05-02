using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net.Mime;
using System.Web.Configuration;

namespace CloudSwyft.Web.Api.Models
{
    public class MailHelper
    {
        public static void SendMail(MailModel model)
        {
            MailMessage mailMsg = new MailMessage();
            mailMsg.To.Add(new MailAddress(model.sendTo));
            mailMsg.From = new MailAddress("no-reply@cloudswyft.com", "CloudSwyft Global Systems Inc");
            mailMsg.Subject = model.subject;
            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(model.htmlBody, null, MediaTypeNames.Text.Html));
            SmtpClient smtpClient = new SmtpClient(WebConfigurationManager.AppSettings["smtpHost"], Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(WebConfigurationManager.AppSettings["smtpUser"], WebConfigurationManager.AppSettings["smtpPass"]);
            smtpClient.Credentials = credentials;

            smtpClient.Send(mailMsg);
        }
    }

    public class MailModel
    {
        public string sendTo;
        public string subject;
        public string htmlBody;
    }
}