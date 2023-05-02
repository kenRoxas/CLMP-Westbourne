using System;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web.Configuration;

namespace CloudSwyft.OAuthServer.Helpers
{
    public class MailHelper
    {
        public static void SendMail(MailInfo model)
        {
            MailMessage mailMsg = new MailMessage();
            mailMsg.To.Add(new MailAddress(model.SendTo));
            mailMsg.From = new MailAddress(WebConfigurationManager.AppSettings["smtpSender"], "CloudSwyft Global Systems Inc");
            mailMsg.Subject = model.Subject;
            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(model.HtmlBody, null, MediaTypeNames.Text.Html));
            SmtpClient smtpClient = new SmtpClient(WebConfigurationManager.AppSettings["smtpHost"], Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(WebConfigurationManager.AppSettings["smtpUser"], WebConfigurationManager.AppSettings["smtpPass"]);
            smtpClient.Credentials = credentials;

            smtpClient.Send(mailMsg);
        }
    }

    public class MailInfo
    {
        public string SendTo { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
    }
}
