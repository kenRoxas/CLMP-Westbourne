using CloudSwyft.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using System.IO;
using System.Web.Util;
using System.Text;
using System.IO.Compression;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web;
using Microsoft.ServiceBus.Messaging;
using System.Configuration;
using Microsoft.ServiceBus;
using System.Dynamic;
using System.Web.Configuration;
using System.Data.SqlClient;

namespace CloudSwyft.Web.Api.Controllers
{  
    [RoutePrefix("api/Files")]
    public class FileConverterController : ApiController
    {
        //this is get so we can use filegetcontents in php :)
        private void SendMessage(object message)
        {
            string queueName = WebConfigurationManager.AppSettings["QueueName"];
            string connectionString = WebConfigurationManager.AppSettings["QueueConnectionString"];// @"Endpoint=sb://cloudswyft.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=RF10n14Rrhmf+p5zLy7QPvuZ+tLi555dWnL0uXZDXzc=";
            QueueClient Client =
              QueueClient.CreateFromConnectionString(connectionString, queueName); //SamplQueue is a test Queue
            string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(message);

            BrokeredMessage sendMessage = new BrokeredMessage(jsonStr);

            Client.Send(sendMessage);
        
        }

        //this will be called by the file conversion module.
        //[Route("UpdateDBCnv")]
        //public IHttpActionResult GetUpdateDBCnv(string url = "", int courseid = 0, int contextid = 0)
        //{ 

        //    SqlConnection conDatabase = new SqlConnection(WebConfigurationManager.AppSettings["AuthConnectionString"]);


        //    try
        //    {
        //        conDatabase.Open();
        //        SqlCommand db_cmd = new SqlCommand("spSwapUrlFileDetails", conDatabase);
        //        db_cmd.CommandType = CommandType.StoredProcedure;
        //        db_cmd.Parameters.AddWithValue("@courseid", courseid);
        //        db_cmd.Parameters.AddWithValue("@url", url.ToString());
        //        db_cmd.Parameters.AddWithValue("@contextid", contextid);

        //        SqlDataReader dbread = db_cmd.ExecuteReader();
        //        string retStr = "";
        //        while (dbread.Read())
        //        {
        //            retStr += dbread.GetString(1).TrimEnd();
        //        }
        //        return Ok(retStr);
        //    }
        //    catch(Exception e){
        //        return Ok(e.ToString());
        //    }

           

        //}



      



    }
}
