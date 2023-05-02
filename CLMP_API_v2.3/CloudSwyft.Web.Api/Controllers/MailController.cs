using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using CloudSwyft.Web.Api.Models;
using MySql.Data.MySqlClient;
using System.Net.Http.Headers;
#pragma warning disable CS0105 // The using directive for 'CloudSwyft.Web.Api.Models' appeared previously in this namespace
using CloudSwyft.Web.Api.Models;
#pragma warning restore CS0105 // The using directive for 'CloudSwyft.Web.Api.Models' appeared previously in this namespace
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using System.Web.Configuration;


namespace CloudSwyft.Web.Api.Controllers
{
    [RoutePrefix("api/Mail")]
    public class MailController : ApiController
    {
        // Get api/Course/Courses
        [HttpPost]
        [Route("SendMail")]
        //[AllowAnonymous] 
        public HttpResponseMessage SendMail(MailModel mailInfo)
        {
            MailMessage mailMsg = new MailMessage();

            // To
            mailMsg.To.Add(new MailAddress(mailInfo.sendTo));

            // From
            mailMsg.From = new MailAddress("no-reply@cloudswyft.com", "CloudSywft Global Systems Inc");

            // Subject and multipart/alternative Body
            mailMsg.Subject = mailInfo.subject;
            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(mailInfo.htmlBody, null, MediaTypeNames.Text.Html));

            // Init SmtpClient and send
            SmtpClient smtpClient = new SmtpClient(WebConfigurationManager.AppSettings["smtpHost"], Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(WebConfigurationManager.AppSettings["smtpUser"], WebConfigurationManager.AppSettings["smtpPass"]);
            smtpClient.Credentials = credentials;

            smtpClient.Send(mailMsg);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("SendProvisioningMail")]
        public HttpResponseMessage SendProvisioningMail(string email, string firstname, string lastname, int numberOfMachines, string courseName)
        {
            MailModel mailInfo = new MailModel();
            mailInfo.sendTo = email;
            mailInfo.subject = "Grant Access to a Machine";


            string htmlBody = string.Empty;

            htmlBody = "<head>"
                    + "<meta charset='utf-8'>"
                    + "<meta name='viewport' content='width=device-width, initial-scale=1.0'/>"
                    + "<title> Grant Access to a Machine! </title>"
                    + "<link rel='stylesheet' href=''>"
                    + "</head>"
                    + "<body style='margin: 0; padding: 0; font-family:'Open Sans', sans;'>"

                    + "<table border='0' cellpadding='0' cellspacing='0' width='90%'>"
                    + "<tr><td><table align = 'center' border = '0' cellpadding = '0' cellspacing = '0' width = '90%' style = 'border-collapse: collapse; border:1px solid #1058a0;'></tr></td>"
                    + "</table>"

                    + "<table align='center' width='90%' style='background-color: #00bff6;padding-top:2%;border-top:2px solid blue;'>"
                    + "<tr align='center'>"
                    + "</table>"
                    + "<table align = 'center' style='background-color: white; width: 90%;padding:15px 40px 15px 40px; border: 1px solid #00bff6; text-align: center; margin-bottom: 15%;'>"
                    + "<tr align = 'center' ><td align='center' style='font-family: Verdana; font-size: 15px; text-align: left;'> There are currently <strong>" + numberOfMachines + "</strong> machines being provisioned</td></tr >"
                    + "<tr align = 'center' ><td align='center' style='font-family: Verdana; font-size: 15px; text-align: left;'>for <strong>" + courseName + "</strong> </td></tr >"
                    + "<tr align = 'center' ><td align='center' style='font-family: Verdana; font-size: 15px; text-align: left;'>By <strong>" + firstname + " " + lastname + " (" + email + ") </strong></td></tr >"

                    + "</table>"

                    + "For any issues or concerns, please send an email to <a href='mailto: support@cloudswyft.com'>support@cloudswyft.com</a>."
                    + "</table>";
            mailInfo.htmlBody = htmlBody;


            MailHelper.SendMail(mailInfo);

            return Request.CreateResponse(HttpStatusCode.OK);

        }
    }
}