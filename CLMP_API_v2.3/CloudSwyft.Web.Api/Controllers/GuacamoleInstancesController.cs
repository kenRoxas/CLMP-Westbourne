using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CloudSwyft.Web.Api.Models;
using System.Text;
using System.IO;
using MySql.Data.MySqlClient;
using System.Web.Configuration;
using Newtonsoft.Json;
using System.Threading;

namespace CloudSwyft.Web.Api.Controllers
{
    //[Authorize]
    [RoutePrefix("api/GuacamoleInstances")]
    public class GuacamoleInstancesController : ApiController
    {
        private VirtualEnvironmentDbContext db = new VirtualEnvironmentDbContext();

        private MySqlConnection guacDatabase = new MySqlConnection(WebConfigurationManager.AppSettings["GuacMySqlConnectionString"]);        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string AddMachineToDatabase(string machineName, string GuacamoleUrl, string guacCon, string vmusername, string vmpassword, int VETypeId, string environment, string region, string fqdn)
        {
            try
            {
                var guacDatabase = new MySqlConnection(guacCon);
                var Environment = environment.Trim() == "D" ? "Staging" : environment.Trim() == "Q" ? "QA" : environment.Trim() == "U" ? "Demo" : "Prod";

                guacDatabase.Open();
                string selectQuery = "";
                string protocol = "rdp";

                selectQuery = $"SELECT connection_id FROM guacamole_connection WHERE connection_name LIKE '{machineName}%'";
                var MySqlCommandConn = new MySqlCommand(selectQuery, guacDatabase);
                var dataReader = MySqlCommandConn.ExecuteReader();

                dataReader.Read();
                if (!dataReader.HasRows)
                {
                    dataReader.Close();

                    var hostName = machineName;

                    var insertQuery = "INSERT INTO guacamole_connection (connection_name, protocol, max_connections, max_connections_per_user) " +
                        $"VALUES (\'{hostName}-{protocol}\', \'{protocol}\', \'5\', \'4\')";

                    var insertCommand = new MySqlCommand(insertQuery, guacDatabase);


                    insertCommand.ExecuteNonQuery();

                    selectQuery = $"SELECT connection_id FROM guacamole_connection WHERE connection_name = \'{hostName}-{protocol}\'";

                    var MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);

                    var dataReaderIns = MySqlCommand.ExecuteReader();

                    dataReaderIns.Read();
                    var connectionId = Convert.ToInt32(dataReaderIns["connection_id"]);

                    dataReaderIns.Close();

                    var guacUrlHostName = GuacamoleUrl;
                    guacUrlHostName = guacUrlHostName.Replace("http://", string.Empty);

                    var insertParamsQuery = string.Empty;

                   
                    insertParamsQuery =
                        "INSERT INTO guacamole_connection_parameter (connection_id, parameter_name, parameter_value) " +
                        //$"VALUES ({connectionId}, 'hostname', '{machineName.ToLower()}.{region}.cloudapp.azure.com'), " +
                        $"VALUES ({connectionId}, 'hostname', '{fqdn}'), " +
                        $"({connectionId}, 'ignore-cert', 'true'), " +
                        $"({connectionId}, 'password', '{vmpassword}'), " +
                        $"({connectionId}, 'security', 'nla'), " +
                        $"({connectionId}, 'port', '3389'), " +
                        $"({connectionId}, 'enable-wallpaper', 'true'), " +
                        $"({connectionId}, 'username', '{vmusername}')";

                    MySqlCommand insertParamsCommand = new MySqlCommand();
                    //windows
                    if (VETypeId == 1 || VETypeId == 3)
                    {
                        insertParamsCommand = new MySqlCommand(insertParamsQuery, guacDatabase);
                    }
                    //linux
                    else if (VETypeId == 2 || VETypeId == 4)
                    {
                        insertParamsQuery = "INSERT INTO guacamole_connection_parameter (connection_id, parameter_name, parameter_value) " +
                        $"VALUES ({connectionId}, 'hostname', '{fqdn}'), " +
                        $"({connectionId}, 'ignore-cert', 'true'), " +
                        $"({connectionId}, 'password', '{vmpassword}'), " +
                        $"({connectionId}, 'security', ''), " +
                        $"({connectionId}, 'port', '3389'), " +
                        $"({connectionId}, 'enable-wallpaper', 'true'), " +
                        $"({connectionId}, 'username', '{vmusername}')";

                        insertParamsCommand = new MySqlCommand(insertParamsQuery, guacDatabase);
                    }


                    insertParamsCommand.ExecuteNonQuery();
                    selectQuery = $"SELECT entity_id FROM guacamole_entity WHERE name = '{Environment}'";
                    MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);
                    var dataReader2 = MySqlCommand.ExecuteReader();
                    dataReader2.Read();
                    var userId = Convert.ToInt32(dataReader2["entity_id"]);
                    dataReader2.Close();

                    var insertPermissionQuery = string.Format("INSERT INTO guacamole_connection_permission(entity_id, connection_id, permission) VALUES ({1},{0}, 'READ')", connectionId, userId);

                    var insertPermissionCommand = new MySqlCommand(insertPermissionQuery, guacDatabase);

                    insertPermissionCommand.ExecuteNonQuery();

                    var clientId = new string[3] { connectionId.ToString(), "c", "mysql" };

                    var bytes = Encoding.UTF8.GetBytes(string.Join("\0", clientId));
                    var connectionString = Convert.ToBase64String(bytes);

                    var guacUrl =
                        $"{GuacamoleUrl}/guacamole/#/client/{connectionString}?username={Environment}&password=pr0v3byd01n6!";


                    var guacamoleInstance = new GuacamoleInstance()
                    {
                        Connection_Name = hostName,
                        Hostname = guacUrlHostName,
                        Url = guacUrl
                    };

                    guacDatabase.Close();

                    return  guacamoleInstance.Url;
                }
                else
                {
                    dataReader.Close();
                    return "";

                }
            }
            catch (Exception ex)
            {
                var x = ex.InnerException;
                return "";
            }

        }
      

    }


}