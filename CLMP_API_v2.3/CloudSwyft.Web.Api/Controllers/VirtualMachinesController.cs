using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CloudSwyft.Web.Api.Models;
using System.Text;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Web;
using System.Reflection;
using Newtonsoft.Json;
using System.Web.Configuration;
using Microsoft.ServiceBus.Messaging;
using System.Web.Http.Hosting;
#pragma warning disable CS0105 // The using directive for 'CloudSwyft.Web.Api.Models' appeared previously in this namespace
using CloudSwyft.Web.Api.Models;
#pragma warning restore CS0105 // The using directive for 'CloudSwyft.Web.Api.Models' appeared previously in this namespace
using System.Collections;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Configuration;

namespace CloudSwyft.Web.Api.Controllers
{
    //[Authorize]
    [RoutePrefix("api/VirtualMachines")]
    public class VirtualMachinesController : ApiController
    {
        private VirtualEnvironmentDbContext db = new VirtualEnvironmentDbContext();


        //// GET: api/VirtualMachines
        //public IEnumerable<GuacamoleInstance> GetVirtualMachines()
        //{
        //    List<GuacamoleInstance> guacInstances = db.GuacamoleInstances.Include(gi => gi.VirtualMachine).ToList();

        //    return guacInstances;
        //}

        //[HttpGet]
        //[Route("GetVEProfileByCourseId")]
        //public HttpResponseMessage GetVEProfileByCourseId(int courseID)
        //{
        //    try
        //    {

        //        var checkPointList = new ArrayList();

        //        int veprofileId = 0;
        //        veprofileId = db.Database.SqlQuery<int>("SELECT DISTINCT VEProfileID FROM VEProfiles WHERE CourseID = {0}", courseID).ToList()[0];

        //        var dataRow = new Hashtable();

        //        dataRow.Add("VEProfileID", veprofileId);

        //        checkPointList.Add(dataRow);

        //        return Request.CreateResponse(HttpStatusCode.OK, checkPointList);
        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, e.Message);
        //    }

        //}

        //[AllowAnonymous]
        //[HttpGet]
        //[Route("HeartbeatInterval")]
        //public HttpResponseMessage HeartbeatInterval()
        //{
        //    return Request.CreateResponse(HttpStatusCode.OK, db.Database.SqlQuery<int>("SELECT TOP 1 HeartbeatInterval FROM dbo.VMConfig").ToList()[0]);
        //}

        //[AllowAnonymous]
        //[HttpGet]
        //[Route("MaxIdleTime")]
        //public HttpResponseMessage MaxIdleTime()
        //{
        //    return Request.CreateResponse(HttpStatusCode.OK, db.Database.SqlQuery<int>("SELECT TOP 1 MaxIdleTime FROM dbo.VMConfig").ToList()[0]);
        //}

        //[AllowAnonymous]
        //[HttpGet]
        //[Route("HeartbeatVersion")]
        //public HttpResponseMessage HeartbeatVersion()
        //{
        //    string version = FileVersionInfo.GetVersionInfo(HttpContext.Current.Server.MapPath("~") + "/Content/Heartbeat/VMHeartbeatService.exe").ProductVersion;

        //    return new HttpResponseMessage()
        //    {
        //        Content = new StringContent(
        //            version,
        //            Encoding.UTF8,
        //            "text/html"
        //        )
        //    };
        //}

        //[AllowAnonymous]
        //[HttpGet]
        //[Route("LinuxHeartbeatVersion")]
        //public HttpResponseMessage LinuxHeartbeatVersion()
        //{
        //    string version = FileVersionInfo.GetVersionInfo(HttpContext.Current.Server.MapPath("~") +
        //        "/Content/Heartbeat/VMHeartbeatApplication.exe").FileVersion;

        //    return new HttpResponseMessage()
        //    {
        //        Content = new StringContent(
        //            version,
        //            Encoding.UTF8,
        //            "text/html"
        //        )
        //    };
        //}

        //[HttpGet]
        //[Route("ByVEProfile")]
        //public IEnumerable<VirtualMachineBindingModel> ByVEProfile(int veProfileID, int guacLinkType = 1)
        //{
        //    List<GuacamoleInstance> guacInstances = db.GuacamoleInstances.Include(gi => gi.VirtualMachine).Where(gi => gi.VirtualMachine.VEProfileID == veProfileID && gi.GuacLinkType == guacLinkType).ToList();
        //    List<VirtualMachineBindingModel> virtualMachines = new List<VirtualMachineBindingModel>();

        //    foreach (GuacamoleInstance gI in guacInstances)
        //    {
        //        VirtualMachineBindingModel vm = TransformGuacInstance(gI);
        //        virtualMachines.Add(vm);
        //    }

        //    return virtualMachines;
        //}

        //[HttpGet]
        //[Route("GetVmByVeProfile")]
        //public IHttpActionResult GetVmByVeProfile(int veProfileID, int guacLinkType = 1)
        //{
        //    List<GuacamoleInstance> guacInstances = db.GuacamoleInstances.Include(gi => gi.VirtualMachine).Where(gi => gi.VirtualMachine.VEProfileID == veProfileID && gi.GuacLinkType == guacLinkType).ToList();
        //    List<VirtualMachineBindingModel> virtualMachines = new List<VirtualMachineBindingModel>();

        //    foreach (GuacamoleInstance gI in guacInstances)
        //    {
        //        VirtualMachineBindingModel vm = TransformGuacInstance(gI);
        //        virtualMachines.Add(vm);
        //    }

        //    return virtualMachines.Count < 1 ? (IHttpActionResult)NotFound() : Ok(virtualMachines);
        //}

        //private VirtualMachineBindingModel TransformGuacInstance(GuacamoleInstance gI)
        //{
        //    var vm = new VirtualMachineBindingModel
        //    {
        //        GuacamoleInstance = new GuacamoleInstance(),
        //        GuacamoleInstanceID = gI.GuacamoleInstanceID
        //    };
        //    vm.GuacamoleInstance.GuacamoleInstanceID = gI.GuacamoleInstanceID;
        //    vm.GuacamoleInstance.Connection_Name = gI.Connection_Name;
        //    vm.GuacamoleInstance.GuacLinkType = gI.GuacLinkType;
        //    vm.GuacamoleInstance.Hostname = gI.Hostname;
        //    vm.GuacamoleInstance.Url = gI.Url;

        //    vm.UserID = gI.VirtualMachine.UserID;
        //    vm.CourseID = gI.VirtualMachine.CourseID;
        //    vm.DateStartedTrigger = gI.VirtualMachine.DateStartedTrigger;
        //    vm.IsStarted = gI.VirtualMachine.IsStarted;
        //    vm.LastTimeStamp = gI.VirtualMachine.LastTimeStamp;
        //    vm.NetworkID = gI.VirtualMachine.NetworkID;
        //    vm.RoleName = gI.VirtualMachine.RoleName;
        //    vm.ServiceName = gI.VirtualMachine.ServiceName;
        //    vm.VEProfileID = gI.VirtualMachine.VEProfileID;
        //    vm.VirtualMachineID = gI.VirtualMachine.VirtualMachineID;
        //    vm.VirtualMachineType = gI.VirtualMachine.VirtualMachineType;
        //    vm.Port = gI.VirtualMachine.Port;
        //    vm.MachineInstance = gI.VirtualMachine.MachineInstance;
        //    // vm.DateCreated = gI.VirtualMachine.DateCreated;  removed as per kem
        //    return vm;
        //}

        //[HttpGet]
        //[Route("ByCourse")]
        //public IEnumerable<VirtualMachineBindingModel> ByCourse(int courseId, int guacLinkType = 1)
        //{
        //    //List<VirtualMachineGuac> vmGuacs = db.VirtualMachineGuacs.Include(vmg => vmg.GuacamoleInstance).ToList();
        //    try
        //    {
        //        UserController userController = new UserController();
        //        List<User> userList = userController.GetUserListByCourse(courseId);

        //        List<GuacamoleInstance> guacInstances = db.GuacamoleInstances.Include(gi => gi.VirtualMachine).Where(gi => gi.VirtualMachine.CourseID == courseId && gi.GuacLinkType == guacLinkType).ToList();
        //        List<VirtualMachineBindingModel> virtualMachines = new List<VirtualMachineBindingModel>();

        //        foreach (GuacamoleInstance gI in guacInstances)
        //        {
        //            VirtualMachineBindingModel vm = TransformGuacInstance(gI);
        //            virtualMachines.Add(vm);
        //        }

        //        foreach (VirtualMachineBindingModel vmEntry in virtualMachines)
        //        {
        //            vmEntry.UserEntry = userList.Where(u => u.UserId == vmEntry.UserID).SingleOrDefault();
        //        }
        //        return virtualMachines;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("Error retrieving data", e);
        //    }

        //    /*
        //    var query = db.VirtualMachines
        //       .Join(db.VirtualMachineGuacs,
        //          vm => vm.VirtualMachineID,
        //          vmg => vmg.GuacamoleInstanceID,
        //          (vm, vmg) => new { VirtualMachine = vm, VirtualMachineGuac = vmg })
        //       .Join(db.GuacamoleInstances,
        //          vmg => vmg.GuacamoleInstanceID,
        //          (gi, vmg) => new { GuacamoleInstance gi, VirtualMachineGuac = vmg })

        //       .Where(vmguacs => vmguacs.VirtualMachineGuac.CourseID == courseId && vmguacs.VirtualMachine.GuacLinkType == 2).ToList();
        //    */


        //}

        //[HttpGet]
        //[Route("ByCourseUser")]
        //public IHttpActionResult ByCourseUser(int userID, int courseID, int guacLinkType = 1)
        //{
        //    try
        //    {
        //        VEProfile veProfile = db.VEProfiles.Where(ve => ve.CourseID == courseID).FirstOrDefault();
        //        if (veProfile == null)
        //            return NotFound();

        //        List<GuacamoleInstance> guacInstances = db.GuacamoleInstances.Include(gi => gi.VirtualMachine).Where(gi => gi.VirtualMachine.CourseID == courseID && gi.VirtualMachine.UserID == userID && gi.VirtualMachine.VEProfileID == veProfile.VEProfileID && gi.GuacLinkType == guacLinkType).ToList();
        //        List<VirtualMachineBindingModel> virtualMachines = new List<VirtualMachineBindingModel>();

        //        foreach (GuacamoleInstance gI in guacInstances)
        //        {
        //            VirtualMachineBindingModel vm = TransformGuacInstance(gI);
        //            virtualMachines.Add(vm);
        //        }

        //        if (virtualMachines.Count < 1)
        //            return NotFound();
        //        else
        //            return Ok(virtualMachines);
        //    }
        //    catch (Exception e)
        //    {
        //        return Ok(e);
        //    }


        //}

        //[HttpGet]
        //[Route("CustomByCourseUser")]
        //public IHttpActionResult CustomByCourseUser(int userID, int courseID, int veProfileID, int guacLinkType = 1)
        //{
        //    //VEProfile veProfile = db.VEProfiles.Where(ve => ve.CourseID == courseID).FirstOrDefault();
        //    //if (veProfile == null)
        //    //    return NotFound();

        //    var veQuery = db.VEProfiles.Include(vp => vp.VirtualEnvironment).SingleOrDefault(ve => ve.VEProfileID == veProfileID);
        //    if (veQuery == null)
        //        return NotFound();

        //    List<GuacamoleInstance> guacInstances = db.GuacamoleInstances.Include(gi => gi.VirtualMachine).Where(gi => gi.VirtualMachine.CourseID == courseID && gi.VirtualMachine.UserID == userID && gi.VirtualMachine.VEProfileID == veQuery.VEProfileID && gi.GuacLinkType == guacLinkType).ToList();
        //    List<VirtualMachineBindingModel> virtualMachines = new List<VirtualMachineBindingModel>();

        //    foreach (GuacamoleInstance gI in guacInstances)
        //    {
        //        VirtualMachineBindingModel vm = TransformGuacInstance(gI);
        //        virtualMachines.Add(vm);
        //    }

        //    if (virtualMachines.Count < 1)
        //        return NotFound();
        //    else
        //        return Ok(virtualMachines);
        //}

        //[HttpGet]
        //[Route("RemoteByCourseUser")]
        //public IHttpActionResult RemoteByCourseUser(int userID, int courseID)
        //{
        //    List<GuacamoleInstance> guacInstances = db.GuacamoleInstances.Include(gi => gi.VirtualMachine).Where(gi => gi.VirtualMachine.CourseID == courseID && gi.VirtualMachine.UserID == userID && gi.GuacLinkType == 2).ToList();
        //    List<VirtualMachineBindingModel> virtualMachines = new List<VirtualMachineBindingModel>();

        //    foreach (GuacamoleInstance gI in guacInstances)
        //    {
        //        VirtualMachineBindingModel vm = TransformGuacInstance(gI);
        //        virtualMachines.Add(vm);
        //    }

        //    if (virtualMachines.Count < 1)
        //        return NotFound();
        //    else
        //        return Ok(virtualMachines);
        //}

        //[HttpGet]
        //[Route("ByVeProfileUser")]
        //public IHttpActionResult ByVeProfileUser(int userID, int veProfileId, int guacLinkType = 1)
        //{
        //    List<GuacamoleInstance> guacInstances = db.GuacamoleInstances.Include(gi => gi.VirtualMachine).Where(gi => gi.VirtualMachine.VEProfileID == veProfileId && gi.VirtualMachine.UserID == userID && gi.GuacLinkType == guacLinkType).ToList();
        //    List<VirtualMachineBindingModel> virtualMachines = new List<VirtualMachineBindingModel>();

        //    foreach (GuacamoleInstance gI in guacInstances)
        //    {
        //        VirtualMachineBindingModel vm = TransformGuacInstance(gI);
        //        virtualMachines.Add(vm);
        //    }

        //    if (virtualMachines.Count < 1)
        //        return NotFound();
        //    else
        //        return Ok(virtualMachines);
        //}

        //[HttpGet]
        //[Route("ToggleVM")]
        //public IHttpActionResult ToggleVM(int veProfileId, int userID, int started)
        //{
        //    VirtualMachine virtualMachine = db.VirtualMachines.Where(vm => vm.VEProfileID == veProfileId && vm.UserID == userID).FirstOrDefault(vm => vm.UserID == userID);

        //    if (virtualMachine == null)
        //        return NotFound();
        //    else
        //    {
        //        virtualMachine.IsStarted = started;
        //        db.Entry(virtualMachine).State = EntityState.Modified;
        //        db.SaveChanges();

        //        return Ok();
        //    }
        //}

        //[HttpGet]
        //[Route("ToggleVM")]
        //public IHttpActionResult ToggleVM(string roleName, int started)
        //{
        //    var virtualMachine = db.VirtualMachines.FirstOrDefault(vm => vm.RoleName == roleName);

        //    if (virtualMachine == null)
        //        return NotFound();

        //    virtualMachine.IsStarted = started;
        //    db.Entry(virtualMachine).State = EntityState.Modified;
        //    db.SaveChanges();

        //    return Ok();
        //}


        //[HttpGet]
        //[Route("StartVM")]
        //public async Task<IHttpActionResult> StartVM (int userID, int veProfileId, int GroupId)
        //{
        //    var cloudLabGroup = db.CloudLabsGroups.Where(clg => clg.CloudLabsGroupID == GroupId).FirstOrDefault();

        //    var veQuery = db.VEProfiles.Include(vp => vp.VirtualEnvironment).SingleOrDefault(ve => ve.VEProfileID == veProfileId);
        //    if (veQuery == null)
        //        return NotFound();



        //    var cloudProvider = db.CloudProviders.SingleOrDefault
        //        (cp => cp.CloudProviderID == veQuery.VirtualEnvironment.CloudProviderID);

        //    var vmInstances = db.VirtualMachines.Where(vms => vms.VEProfileID == veProfileId && vms.UserID == userID).ToList();
        //    var vmMappings = db.VirtualMachineMappings.Where(vms => vms.VEProfileID == veProfileId && vms.UserID == userID).ToList();

        //    var machName = db.VirtualMachineMappings.Where(p => p.VEProfileID == veProfileId && p.UserID == userID).FirstOrDefault();
        //    char[] dashSplit = { '-' };
        //    var prefix = ""; var sqlCode = string.Empty;
        //    foreach (var virtualMachine in vmInstances)
        //    {
        //        _connTenant.Open();
        //        var sqlTenant = new SqlCommand(string.Format("SELECT Code FROM Tenants WHERE Code = '{0}'", machName.VMName.Split(dashSplit)), _connTenant);
        //        //var sqlCode = string.Empty;
        //        using (var sqlReader = sqlTenant.ExecuteReader())
        //        {
        //            while (sqlReader.Read())
        //            {
        //                sqlCode = sqlReader["Code"].ToString();
        //            }
        //        }

        //        //var prefix = virtualMachine.RoleName.Split(dashSplit).Length > 3 ?
        //        //  db.Database.SqlQuery<string>("SELECT TOP 1 VMPrefix FROM dbo.TenantData").ToList()[0] :
        //        //  string.Empty;

        //        //prefix = machName.RoleName.Split(dashSplit).Length > 3 ?
        //        // sqlCode : string.Empty;


        //        var suffix = virtualMachine.RoleName.Split(dashSplit).Length > 4 ?
        //               virtualMachine.RoleName.Split(dashSplit)[4] :
        //               string.Empty;

        //        #region for multi-tenancy provisioning
        //        //_conn.Open();
        //        //var sqlCommand = new SqlCommand(string.Format("SELECT DISTINCT ApiUrl, AzurePort, GuacConnection, GuacamoleUrl, Region FROM TenantCodes WHERE Code = '{0}' AND GuacConnection is not null", prefix), _conn);
        //        var sqlCommand = new SqlCommand(string.Format("SELECT ApiUrl FROM Tenants WHERE Code = '{0}'", prefix), _connTenant);

        //        var apiUrl = string.Empty;
        //        using (var sqlReader = sqlCommand.ExecuteReader())
        //        {
        //            while (sqlReader.Read())
        //            {
        //                apiUrl = sqlReader["ApiUrl"].ToString();
        //            }
        //        }
        //        //_conn.Close();
        //        _connTenant.Close();
        //        #endregion
        //        //NOT SURE WHAT IT DOES
        //        //if (virtualMachine.DateStartedTrigger.AddMinutes(5) > DateTime.Now) continue;
        //        var vmOperation = new VMOperation
        //        {
        //            Operation = "Start",
        //            CourseID = virtualMachine.CourseID,
        //            VEProfileID = virtualMachine.VEProfileID,
        //            UserID = virtualMachine.UserID,
        //            Prefix = sqlCode,
        //            Suffix = suffix,
        //            WebApiUrl = apiUrl,
        //            MachineName = machName.VMName
        //        };
        //        vmOperationMultiple.Add(vmOperation);

        //        UpdateStartDate(virtualMachine);
        //    }

        //    var dataSend = new VMSendMessage
        //    {
        //        Rolename = vmOperationMultiple[0].RoleName,
        //        Machinename = vmOperationMultiple[0].MachineName,
        //        ApiUrl = WebConfigurationManager.AppSettings["CSWebApiUrl"],
        //        ResourceGroup = ResourceGroupName(cloudLabGroup.CLPrefix, sqlCode).ToUpper(),
        //        UserId = vmOperationMultiple[0].UserID,
        //        VEProfileId = vmOperationMultiple[0].VEProfileID
        //    };

        //    var dataMessage = JsonConvert.SerializeObject(dataSend);

        //    if (vmInstances.Count > 0)
        //    {
        //        await ApiCall("POST", FAApiUrl, StartMachine, dataMessage);
        //        //SendStartMessage(cloudProvider.Name);
        //    }

        //    return Ok();
        //}

        //[HttpGet]
        //[Route("StartCourseVM")]
        //public IHttpActionResult StartCourseVM(int veProfileId)
        //{
        //    var veQuery = db.VEProfiles.Include(vp => vp.VirtualEnvironment).SingleOrDefault(ve => ve.VEProfileID == veProfileId);
        //    if (veQuery == null)
        //        return NotFound();

        //    var cloudProvider = db.CloudProviders.SingleOrDefault
        //        (cp => cp.CloudProviderID == veQuery.VirtualEnvironment.CloudProviderID);

        //    var vmInstances = db.VirtualMachines.Where(vms => vms.VEProfileID == veProfileId).ToList();

        //    char[] dashSplit = { '-' };

        //    foreach (var virtualMachine in vmInstances)
        //    {
        //        _connTenant.Open();
        //        var sqlTenant = new SqlCommand(string.Format("SELECT Code FROM Tenants WHERE Code = '{0}'", virtualMachine.RoleName.Split(dashSplit)), _connTenant);
        //        var sqlCode = string.Empty;
        //        using (var sqlReader = sqlTenant.ExecuteReader())
        //        {
        //            while (sqlReader.Read())
        //            {
        //                sqlCode = sqlReader["Code"].ToString();
        //            }
        //        }

        //        //var prefix = virtualMachine.RoleName.Split(dashSplit).Length > 3 ?
        //        //  db.Database.SqlQuery<string>("SELECT TOP 1 VMPrefix FROM dbo.TenantData").ToList()[0] :
        //        //  string.Empty;
        //        var prefix = virtualMachine.RoleName.Split(dashSplit).Length > 3 ?
        //            sqlCode : string.Empty;

        //        var suffix = virtualMachine.RoleName.Split(dashSplit).Length > 4 ?
        //               virtualMachine.RoleName.Split(dashSplit)[4] :
        //               string.Empty;

        //        #region for multi-tenancy provisioning
        //        //_conn.Open();
        //        //var sqlCommand = new SqlCommand(string.Format("SELECT DISTINCT ApiUrl, AzurePort, GuacConnection, GuacamoleUrl, Region FROM TenantCodes WHERE Code = '{0}' AND GuacConnection is not null", prefix), _conn);
        //        var sqlCommand = new SqlCommand(string.Format("SELECT ApiUrl FROM Tenants WHERE Code = '{0}'", prefix), _connTenant);

        //        var apiUrl = string.Empty;
        //        using (var sqlReader = sqlCommand.ExecuteReader())
        //        {
        //            while (sqlReader.Read())
        //            {
        //                apiUrl = sqlReader["ApiUrl"].ToString();
        //            }
        //        }
        //        //_conn.Close();
        //        _connTenant.Close();
        //        #endregion

        //        if (virtualMachine.DateStartedTrigger.AddMinutes(5) > DateTime.Now) continue;
        //        var vmOperation = new VMOperation
        //        {
        //            Operation = "Start",
        //            CourseID = virtualMachine.CourseID,
        //            VEProfileID = virtualMachine.VEProfileID,
        //            UserID = virtualMachine.UserID,
        //            Prefix = prefix,
        //            Suffix = suffix,
        //            WebApiUrl = apiUrl
        //        };
        //        vmOperationMultiple.Add(vmOperation);

        //        UpdateStartDate(virtualMachine);
        //    }

        //    if (vmInstances.Count > 0)
        //    {
        //        SendStartMessage(cloudProvider.Name);
        //    }

        //    return Ok();
        //}

        //private void UpdateStartDate(VirtualMachine virtualMachine)
        //{
        //    //throw new NotImplementedException();
        //    virtualMachine.DateStartedTrigger = DateTime.UtcNow;
        //    //virtualMachine.DateStartedTrigger = DateTime.Now;
        //    db.Entry(virtualMachine).State = EntityState.Modified;
        //    db.SaveChanges();
        //}

        //private void SendStartMessage(string cloudProviderName)
        //{
        //    string queueName = string.Empty;

        //    if (cloudProviderName == "WindowsAzure")
        //    {
        //        //queueName = WebConfigurationManager.AppSettings["AzureProvisionQueueName"];
        //        queueName = WebConfigurationManager.AppSettings["AzureMessageQueueName"];
        //    }
        //    else if (cloudProviderName == "AmazonWebServices")
        //    {
        //        queueName = WebConfigurationManager.AppSettings["AmazonProvisionQueueName"];
        //    }

        //    SendMessage(vmOperationMultiple, queueName);
        //    vmOperationMultiple.Clear();
        //}

        //private void SendShutdownMessage(string cloudProviderName)
        //{
        //    string queueName = string.Empty;

        //    if (cloudProviderName == "WindowsAzure")
        //    {
        //        //queueName = WebConfigurationManager.AppSettings["AzureProvisionQueueName"];
        //        queueName = WebConfigurationManager.AppSettings["AzureMessageQueueName"];

        //    }
        //    else if (cloudProviderName == "AmazonWebServices")
        //    {
        //        queueName = WebConfigurationManager.AppSettings["AmazonProvisionQueueName"];
        //    }

        //    SendMessage(vmOperationMultiple, queueName);
        //    vmOperationMultiple.Clear();
        //}

        //private void SendMessage(object message, string queueName)
        //{
        //    string connectionString = WebConfigurationManager.AppSettings["QueueConnectionString"];
        //    QueueClient Client =
        //      QueueClient.CreateFromConnectionString(connectionString, queueName); //SamplQueue is a test Queue
        //    string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(message);

        //    BrokeredMessage sendMessage = new BrokeredMessage(jsonStr);

        //    Client.Send(sendMessage);

        //    //string connectionString = WebConfigurationManager.AppSettings["QueueConnectionString"];
        //    //QueueClient Client = QueueClient.CreateFromConnectionString(connectionString, queueName); //SamplQueue is a test Queue
        //    //string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(message);

        //    //BrokeredMessage sendMessage = new BrokeredMessage(jsonStr);

        //    //Client.Send(sendMessage);


        //    //string connectionString = WebConfigurationManager.AppSettings["QueueConnectionString"];
        //    //QueueClient Client = QueueClient.CreateFromConnectionString(connectionString, queueName); //SamplQueue is a test Queue
        //    //string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(message);

        //    //BrokeredMessage sendMessage = new BrokeredMessage(jsonStr);

        //    //Client.Send(sendMessage);


        //}

        ///*
        //[HttpGet]
        //[Route("CheckVMStarted")]
        //public async Task<IHttpActionResult> CheckVMStarted(int userID, int courseID)
        //{
        //    VirtualMachine virtualMachine = db.VirtualMachines.Where(vm => vm.CourseID == courseID && vm.UserID == userID).FirstOrDefault();

        //    if (virtualMachine == null)
        //    {
        //        return NotFound();
        //    }

        //    string cloudServiceCallResponse = await ApiCall("GET", GenericStringsCollection.CSWebUrl, "vlabs/CloudService?serviceName=" + virtualMachine.ServiceName);

        //    CloudService cloudService = JsonConvert.DeserializeObject<CloudService>(cloudServiceCallResponse);

        //    RoleInstance roleInstanceEquivalent = null;

        //    Deployment deployment;

        //    for (int x = 0; x < cloudService.Deployments.Deployment.Count; x++)
        //    {
        //        deployment = cloudService.Deployments.Deployment[x];

        //        for (int y = 0; y < deployment.RoleInstanceList.RoleInstance.Count; y++)
        //        {
        //            if (deployment.RoleInstanceList.RoleInstance[y].RoleName == virtualMachine.RoleName)
        //            {
        //                roleInstanceEquivalent = deployment.RoleInstanceList.RoleInstance[y];
        //                break;
        //            }
        //        }
        //    }

        //    string powerState = "";

        //    if (roleInstanceEquivalent == null)
        //    {
        //        powerState = "Stopped(Deallocated)";
        //    } else
        //    {
        //        powerState = roleInstanceEquivalent.PowerState;
        //    }

        //    return Ok(powerState);
            
        //}
        //*/

        ////[HttpGet]
        ////[Route("ShutdownUnusedVMs")]
        ////public IHttpActionResult ShutdownUnusedVMs()
        ////{
        ////    List<VirtualMachine> vmList = db.VirtualMachines.Where(vm => vm.IsStarted == 1).ToList();
        ////    List<VirtualMachine> shutdownVMList = new List<VirtualMachine>();

        ////    VirtualMachine currentVM;

        ////    foreach (VirtualMachine t in vmList)
        ////    {
        ////        currentVM = t;

        ////        if (currentVM.ServiceName != "Web")
        ////        {
        ////            string nowTimeString = DateTime.Now.ToUniversalTime().ToString();
        ////            string vmLastTimeString = currentVM.LastTimeStamp.ToString();

        ////            TimeSpan timeSpan = DateTime.Now.ToUniversalTime() - currentVM.LastTimeStamp;

        ////            /*
        ////            if (timeSpan.TotalMinutes >= 5.0d && currentVM.LastTimeStamp != DateTime.Parse("1/1/1753 12:00:00 AM"))
        ////            {
                        
        ////            }
        ////            */
        ////            shutdownVMList.Add(currentVM);
        ////        }
        ////    }

        ////    foreach (VirtualMachine t in shutdownVMList)
        ////    {
        ////        ShutdownVM(t.CourseID, t.VEProfileID, t.UserID);
        ////    }

        ////    string shutdownVMNames = "";

        ////    for (int z = 0; z < shutdownVMList.Count; z++)
        ////    {
        ////        if (z != 0) shutdownVMNames = shutdownVMNames + ", ";

        ////        shutdownVMNames = shutdownVMNames + shutdownVMList[z].RoleName;
        ////    }

        ////    return Ok("Virtual machines queued for shutdown: " + shutdownVMNames);
        ////}

        ////[HttpPost]
        ////[Route("ShutdownVM")]
        ////public async Task<IHttpActionResult> ShutdownVM(int courseId, int veProfileId, int userId, int GroupId)
        ////{
        ////    var cloudLabGroup = db.CloudLabsGroups.Where(clg => clg.CloudLabsGroupID == GroupId).FirstOrDefault();

        ////    var veQuery = db.VEProfiles.Include(vp => vp.VirtualEnvironment).SingleOrDefault(ve => ve.VEProfileID == veProfileId);
        ////    if (veQuery == null)
        ////        return Ok();

        ////    var cloudProvider = db.CloudProviders.SingleOrDefault
        ////        (cp => cp.CloudProviderID == veQuery.VirtualEnvironment.CloudProviderID);

        ////    var virtualMachines = db.VirtualMachines.Where(vm => vm.CourseID == courseId
        ////        && vm.VEProfileID == veProfileId && vm.UserID == userId).ToList();
        ////    var machName = db.VirtualMachineMappings.Where(p => p.VEProfileID == veProfileId && p.UserID == userId && p.CourseID == courseId).FirstOrDefault();


        ////    char[] dashSplit = { '-' };
        ////    var prefix = ""; var sqlCode = string.Empty;
        ////    foreach (var virtualMachine in virtualMachines)
        ////    {
        ////        _connTenant.Open();
        ////        var sqlTenant = new SqlCommand(string.Format("SELECT Code FROM Tenants WHERE Code = '{0}'", machName.RoleName.Split(dashSplit)), _connTenant);

        ////        using (var sqlReader = sqlTenant.ExecuteReader())
        ////        {
        ////            while (sqlReader.Read())
        ////            {
        ////                sqlCode = sqlReader["Code"].ToString();
        ////            }
        ////        }

        ////        //var prefix = virtualMachine.RoleName.Split(dashSplit).Length > 3 ?
        ////        //  db.Database.SqlQuery<string>("SELECT TOP 1 VMPrefix FROM dbo.TenantData").ToList()[0] :
        ////        //  string.Empty;
        ////        prefix = virtualMachine.RoleName.Split(dashSplit).Length > 3 ?
        ////            sqlCode : string.Empty;

        ////        var suffix = virtualMachine.RoleName.Split(dashSplit).Length > 4 ?
        ////           virtualMachine.RoleName.Split(dashSplit)[4] :
        ////           string.Empty;

        ////        //SendShutdownMessage(prefix, virtualMachine.CourseID,
        ////        //    virtualMachine.VEProfileID, virtualMachine.UserID, cloudProvider.Name, suffix);

        ////        #region for multi-tenancy provisioning
        ////        //_conn.Open();
        ////        //var sqlCommand = new SqlCommand(string.Format("SELECT DISTINCT ApiUrl, AzurePort, GuacConnection, GuacamoleUrl, Region FROM TenantCodes WHERE Code = '{0}' AND GuacConnection is not null", prefix), _conn);
        ////        var sqlCommand = new SqlCommand(string.Format("SELECT ApiUrl FROM Tenants WHERE Code = '{0}'", prefix), _connTenant);

        ////        var apiUrl = string.Empty;
        ////        using (var sqlReader = sqlCommand.ExecuteReader())
        ////        {
        ////            while (sqlReader.Read())
        ////            {
        ////                apiUrl = sqlReader["ApiUrl"].ToString();
        ////            }
        ////        }
        ////        _conn.Close();
        ////        _connTenant.Close();
        ////        #endregion

        ////        var vmOperation = new VMOperation
        ////        {
        ////            Operation = "Shutdown",
        ////            CourseID = virtualMachine.CourseID,
        ////            VEProfileID = virtualMachine.VEProfileID,
        ////            UserID = virtualMachine.UserID,
        ////            Prefix = prefix,
        ////            Suffix = suffix,
        ////            WebApiUrl = apiUrl,
        ////            MachineName = machName.RoleName
        ////        };


        ////        vmOperationMultiple.Add(vmOperation);

        ////        //virtualMachine.IsStarted = 6;

        ////        db.Entry(virtualMachine).State = EntityState.Modified;

        ////        db.SaveChanges();
        ////    }
        ////    var dataSend = new VMSendMessage
        ////    {
        ////        Rolename = vmOperationMultiple[0].RoleName,
        ////        Machinename = vmOperationMultiple[0].MachineName,
        ////        ApiUrl = WebConfigurationManager.AppSettings["CSWebApiUrl"],
        ////        ResourceGroup = ResourceGroupName(cloudLabGroup.CLPrefix, sqlCode).ToUpper(),
        ////        UserId = vmOperationMultiple[0].UserID,
        ////        VEProfileId = vmOperationMultiple[0].VEProfileID
        ////    };

        ////    var dataMessage = JsonConvert.SerializeObject(dataSend);

        ////    if (virtualMachines.Count > 0)
        ////    {
        ////        await ApiCall("POST", FAApiUrl, ShutdownMachine, dataMessage);
                    
        ////        //SendShutdownMessage(cloudProvider.Name);
        ////    }

        ////    return Ok(virtualMachines);
        ////}

        //private string ResourceGroupName(string GroupPrefix, string TenantPrefix)
        //{

        //    return ("CS-" + TenantPrefix + '-' + GroupPrefix);

        //}

        //// GET: api/VirtualMachines/5
        //[ResponseType(typeof(VirtualMachine))]
        //public IHttpActionResult GetVirtualMachine(int id, int guacLinkType = 1)
        //{
        //    List<GuacamoleInstance> guacInstances = db.GuacamoleInstances.Include(gi => gi.VirtualMachine).Where(gi => gi.VirtualMachine.VirtualMachineID == id && gi.GuacLinkType == guacLinkType).ToList();
        //    List<VirtualMachineBindingModel> virtualMachines = new List<VirtualMachineBindingModel>();

        //    if (guacInstances == null)
        //    {
        //        return NotFound();
        //    }

        //    foreach (GuacamoleInstance gI in guacInstances)
        //    {
        //        VirtualMachineBindingModel vm = TransformGuacInstance(gI);
        //        virtualMachines.Add(vm);
        //    }

        //    /*
        //    VirtualMachinePostModel vmPost = new VirtualMachinePostModel();
        //    vmPost.CourseID = virtualMachines[0].CourseID;
        //    vmPost.DateStartedTrigger = virtualMachines[0].DateStartedTrigger;
        //    vmPost.IsStarted = virtualMachines[0].IsStarted;
        //    vmPost.LastTimeStamp = virtualMachines[0].LastTimeStamp;
        //    vmPost.NetworkID = virtualMachines[0].NetworkID;
        //    vmPost.RoleName = virtualMachines[0].RoleName;
        //    vmPost.ServiceName = virtualMachines[0].ServiceName;
        //    vmPost.UserID = virtualMachines[0].UserID;
        //    vmPost.VEProfileID = virtualMachines[0].VEProfileID;
        //    vmPost.GuacamoleInstances = new List<GuacamoleInstance>();

        //    foreach (GuacamoleInstance gI in guacInstances)
        //    {
        //        vmPost.GuacamoleInstances.Add(gI);
        //    }
        //    */

        //    return Ok(virtualMachines);
        //}

        //[ResponseType(typeof(VirtualMachine))]
        //public IHttpActionResult GetVirtualMachine(string roleName, int guacLinkType = 1)
        //{
        //    GuacamoleInstance guacInstance = db.GuacamoleInstances.Include(gi => gi.VirtualMachine).Where(gi => gi.VirtualMachine.RoleName == roleName && gi.GuacLinkType == guacLinkType).FirstOrDefault();
        //    VirtualMachineBindingModel virtualMachine = new VirtualMachineBindingModel();

        //    if (guacInstance == null)
        //    {
        //        return NotFound();
        //    }

        //    virtualMachine = TransformGuacInstance(guacInstance);

        //    return Ok(virtualMachine);
        //}

        //// PUT: api/VirtualMachines/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutVirtualMachine(int id, VirtualMachine virtualMachine)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != virtualMachine.VirtualMachineID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(virtualMachine).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!VirtualMachineExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //// POST: api/VirtualMachines
        //[ResponseType(typeof(VirtualMachinePostModel))]
        //public IHttpActionResult PostVirtualMachine(VirtualMachinePostModel virtualMachine)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    VirtualMachine vms = db.VirtualMachines.Where(vmi => vmi.RoleName == virtualMachine.RoleName).FirstOrDefault();
        //    if (vms == null)
        //    {
        //        vms = new VirtualMachine();
        //        vms.CourseID = virtualMachine.CourseID;
        //        vms.DateStartedTrigger = virtualMachine.DateStartedTrigger;
        //        vms.IsStarted = virtualMachine.IsStarted;
        //        vms.LastTimeStamp = virtualMachine.LastTimeStamp;
        //        vms.NetworkID = virtualMachine.NetworkID;
        //        vms.RoleName = virtualMachine.RoleName;
        //        vms.ServiceName = virtualMachine.ServiceName;
        //        vms.UserID = virtualMachine.UserID;
        //        vms.VEProfileID = virtualMachine.VEProfileID;
        //        vms.VirtualMachineType = virtualMachine.VirtualMachineType;
        //        vms.MachineInstance = virtualMachine.MachineInstance;
        //        vms.DateCreated = virtualMachine.DateCreated;

        //        db.VirtualMachines.Add(vms);
        //        db.SaveChanges();
        //    }

        //    if (virtualMachine.GuacamoleInstances.Count > 0)
        //    {
        //        foreach (GuacamoleInstance guacIns in virtualMachine.GuacamoleInstances)
        //        {
        //            GuacamoleInstance guacInstance = db.GuacamoleInstances.Where(gi => gi.Url == guacIns.Url && gi.GuacLinkType == guacIns.GuacLinkType).FirstOrDefault();
        //            if (guacInstance == null)
        //            {
        //                guacInstance = new GuacamoleInstance();
        //                guacInstance.Connection_Name = guacIns.Connection_Name;
        //                guacInstance.GuacLinkType = guacIns.GuacLinkType;
        //                guacInstance.Hostname = guacIns.Hostname;
        //                guacInstance.Url = guacIns.Url;
        //                guacInstance.VirtualMachineID = vms.VirtualMachineID;

        //                db.GuacamoleInstances.Add(guacInstance);
        //            }
        //        }
        //        db.SaveChanges();
        //    }

        //    return Ok(vms);
        //}

        //// DELETE: api/VirtualMachines/5
        //[ResponseType(typeof(VirtualMachine))]
        //public IHttpActionResult DeleteVirtualMachine(int id)
        //{
        //    VirtualMachine virtualMachine = db.VirtualMachines.Find(id);
        //    if (virtualMachine == null)
        //    {
        //        return NotFound();
        //    }

        //    db.VirtualMachines.Remove(virtualMachine);
        //    db.SaveChanges();

        //    GuacamoleInstancesController guacController = new GuacamoleInstancesController();
        //    guacController.DeleteGuacamoleInstance(virtualMachine.VirtualMachineID);

        //    return Ok(virtualMachine);
        //}


        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        //private bool VirtualMachineExists(int id)
        //{
        //    return db.VirtualMachines.Count(e => e.VirtualMachineID == id) > 0;
        //}

        //private async Task<string> ApiCall(string method, string baseAddress, string url, string data = "")
        //{
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //    HttpClient client = new HttpClient();
        //    client.BaseAddress = new Uri(baseAddress);
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    HttpResponseMessage response = null;

        //    if (method == "POST")
        //    {
        //        response = await client.PostAsync(client.BaseAddress + url, new StringContent(data));
        //    }
        //    else if (method == "GET")
        //    {
        //        response = await client.GetAsync(url);
        //    }
        //    else if (method == "DELETE")
        //    {
        //        response = await client.DeleteAsync(url);
        //    }

        //    return await response.Content.ReadAsStringAsync();

        //}

        //[HttpDelete]
        //[Route("DeleteVms")]
        //public IHttpActionResult DeleteVms(int veProfileID)
        //{
        //    //VEProfile veProfile = _db.VEProfiles.Include(ve => ve.VirtualEnvironment).SingleOrDefault<VEProfile>(ve => ve.VEProfileID == veProfileID);

        //    VirtualMachine VirtualMachines = db.VirtualMachines.SingleOrDefault<VirtualMachine>(vm => vm.VEProfileID == veProfileID);

        //    List<VirtualMachine> veProfileVMs = db.VirtualMachines.Where(vm => vm.VEProfileID == VirtualMachines.VEProfileID).ToList();

        //    DeleteVMs(veProfileVMs);


        //    return Ok(veProfileVMs);
        //}

        //private void DeleteVMs(List<VirtualMachine> vmList)
        //{
        //    char[] dashSplit = { '-' };

        //    foreach (var t in vmList)
        //    {
        //        var prefix = t.RoleName.Split(dashSplit).Length > 3 ?
        //            db.Database.SqlQuery<string>("SELECT TOP 1 VMPrefix FROM dbo.TenantData").ToList()[0] :
        //            string.Empty;

        //        var suffix = t.RoleName.Split(dashSplit).Length > 4 ?
        //            t.RoleName.Split(dashSplit)[4] :
        //            string.Empty;

        //        #region for multi-tenancy provisioning
        //        //_conn.Open();
        //        _connTenant.Open();
        //        //var sqlCommand = new SqlCommand(string.Format("SELECT DISTINCT ApiUrl, AzurePort, GuacConnection, GuacamoleUrl, Region FROM TenantCodes WHERE Code = '{0}' AND guacConnection is not null", prefix), _conn);
        //        var sqlCommand = new SqlCommand(string.Format("SELECT ApiUrl FROM Tenants WHERE Code = '{0}'", prefix), _connTenant);

        //        var guacConnection = string.Empty;
        //        var apiUrl = string.Empty;
        //        using (var sqlReader = sqlCommand.ExecuteReader())
        //        {
        //            while (sqlReader.Read())
        //            {
        //                guacConnection = sqlReader["GuacConnection"].ToString();
        //                apiUrl = sqlReader["ApiUrl"].ToString();
        //            }
        //        }
        //        // _conn.Close();
        //        _connTenant.Close();
        //        #endregion

        //        //SendDeleteMessage(prefix, vmList[x].CourseID, vmList[x].VEProfileID, vmList[x].UserID, cloudProvider.Name, suffix);
        //        var vmOperation = new VMOperation
        //        {
        //            Operation = "Delete",
        //            CourseID = t.CourseID,
        //            VEProfileID = t.VEProfileID,
        //            UserID = t.UserID,
        //            Prefix = prefix,
        //            Suffix = suffix,
        //            GuacConnection = guacConnection,
        //            WebApiUrl = apiUrl
        //        };


        //        _vmOperationMultiple.Add(vmOperation);
        //    }
        //}

        //private void SendDeleteMessage(string cloudProviderName)
        //{
        //    string queueName = string.Empty;

        //    if (cloudProviderName == "WindowsAzure")
        //    {
        //        queueName = WebConfigurationManager.AppSettings["AzureProvisionQueueName"];
        //    }
        //    else if (cloudProviderName == "AmazonWebServices")
        //    {
        //        queueName = WebConfigurationManager.AppSettings["AmazonProvisionQueueName"];
        //    }

        //    SendMessage(_vmOperationMultiple, queueName);
        //}

        //[AllowAnonymous]
        //[HttpPost]
        //[Route("ShutdownHeartBeatVM")]
        //public async Task<IHttpActionResult> ShutdownHeartBeatVM(VirtualMachineLogVM log)
        //{
        //    var veQuery = db.VEProfiles.Include(vp => vp.VirtualEnvironment).SingleOrDefault(ve => ve.VEProfileID == log.VEProfileID);
        //    if (veQuery == null)
        //        return Ok();

        //    var cloudProvider = db.CloudProviders.SingleOrDefault
        //        (cp => cp.CloudProviderID == veQuery.VirtualEnvironment.CloudProviderID);

        //    var virtualMachines = db.VirtualMachines.Where(vm => vm.CourseID == log.CourseID
        //        && vm.VEProfileID == log.VEProfileID && vm.UserID == log.UserID).ToList();

        //    char[] dashSplit = { '-' };
        //    foreach (var virtualMachine in virtualMachines)
        //    {
        //        _connTenant.Open();
        //        var sqlTenant = new SqlCommand(string.Format("SELECT Code FROM Tenants WHERE Code = '{0}'", virtualMachine.RoleName.Split(dashSplit)), _connTenant);
        //        var sqlCode = string.Empty;

        //        using (var sqlReader = sqlTenant.ExecuteReader())
        //        {
        //            while (sqlReader.Read())
        //            {
        //                sqlCode = sqlReader["Code"].ToString();
        //            }
        //        }

        //        var prefix = virtualMachine.RoleName.Split(dashSplit).Length > 3 ?
        //            sqlCode : string.Empty;

        //        var suffix = virtualMachine.RoleName.Split(dashSplit).Length > 4 ?
        //           virtualMachine.RoleName.Split(dashSplit)[4] :
        //           string.Empty;

        //        var sqlCommand = new SqlCommand(string.Format("SELECT ApiUrl FROM Tenants WHERE Code = '{0}'", prefix), _connTenant);

        //        var apiUrl = string.Empty;
        //        using (var sqlReader = sqlCommand.ExecuteReader())
        //        {
        //            while (sqlReader.Read())
        //            {
        //                apiUrl = sqlReader["ApiUrl"].ToString();
        //            }
        //        }
        //        // _conn.Close();
        //        _connTenant.Close();

        //        var vmOperation = new VMOperation
        //        {
        //            Operation = "Shutdown",
        //            CourseID = virtualMachine.CourseID,
        //            VEProfileID = virtualMachine.VEProfileID,
        //            UserID = virtualMachine.UserID,
        //            Prefix = prefix,
        //            Suffix = suffix,
        //            WebApiUrl = apiUrl,
        //            MachineName = log.MachineName
        //        };

        //        vmOperationMultiple.Add(vmOperation);

        //        //virtualMachine.IsStarted = 6;

        //        db.Entry(virtualMachine).State = EntityState.Modified;

        //        db.SaveChanges();               
        //    }

        //    for (int i = 0; vmOperationMultiple.Count() > i; i++)
        //    {
        //        var dataSend = new VMSendMessage
        //        {
        //            Rolename = vmOperationMultiple[i].RoleName,
        //            Machinename = vmOperationMultiple[i].MachineName,
        //            ApiUrl = WebConfigurationManager.AppSettings["CSWebApiUrl"],
        //            ResourceGroup = WebConfigurationManager.AppSettings["ResourceGroup"]
        //        };

        //        var dataMessage = JsonConvert.SerializeObject(dataSend);

        //        ApiCall("POST", FAApiUrl, ShutdownMachine, dataMessage);
        //    }
            
             
        //    return Ok(virtualMachines);
        //}

        //[HttpGet]
        //[Route("GetLogs")]
        //public IHttpActionResult GetLogs(string Comment, int userID, int veProfileId)
        //{
        //    var vmQuery = db.VirtualMachineLogs.Where(p => p.Comment == Comment && p.UserID == userID && p.VEProfileID == veProfileId).FirstOrDefault();
        //    return Ok(vmQuery);
        //}

        //[HttpGet]
        //[Route("GetVMConfig")]
        //public HttpResponseMessage GetVMConfig()
        //{
        //    var data = db.Database.SqlQuery<VMConfig>("SELECT TOP 1 * FROM dbo.VMConfig").FirstOrDefault();
        //    return Request.CreateResponse(HttpStatusCode.OK, data);
        //}


        //[HttpPost]
        //[Route("SetVMConfig")]
        //public HttpResponseMessage SetVMConfig(VMConfig model)
        //{
        //    var data = db.Database.ExecuteSqlCommand("Update VMConfig set HeartbeatInterval = " + model.HeartbeatInterval + ", MaxIdleTime = " + model.MaxIdleTime + ", MaxJobIdleTime = " + model.MaxJobIdleTime);
        //    return Request.CreateResponse(HttpStatusCode.OK, data);
        //}

        ////[HttpPost]
        ////[Route("ShutdownMultipleVM")]
        ////public async Task<IHttpActionResult> ShutdownMultipleVM(List<UserVeProfile> user)
        ////{
        ////    UserVeProfile[] userDetails = user.ToArray();

        ////    try
        ////    {
        ////        for(int i = 0; user.Count() > i; i++)
        ////        {

        ////            var userId = userDetails[i].UserId;
        ////            var veprofileId = userDetails[i].VEProfileId;


        ////            var userDetail = db.VirtualMachineMappings
        ////                .Where(vmm => vmm.UserID == userId && vmm.VEProfileID == veprofileId)
        ////                .GroupJoin(db.VirtualMachines,
        ////                a => a.UserID,
        ////                b => b.UserID,
        ////                (a, b) => new { VirtualMachineMappings = a, Virtualmachines = b })
        ////                .Select(q => new
        ////                {
        ////                    Rolename = q.Virtualmachines.Select(vm => vm.RoleName).FirstOrDefault(),
        ////                    Machinename = q.VirtualMachineMappings.RoleName
        ////                }).ToList();

        ////            var dataSend = new VMSendMessage
        ////            {
        ////                Rolename = userDetail[0].Rolename,
        ////                Machinename = userDetail[0].Machinename,
        ////                ApiUrl = WebConfigurationManager.AppSettings["CSWebApiUrl"],
        ////                ResourceGroup = WebConfigurationManager.AppSettings["ResourceGroup"]
        ////            };

        ////            var dataMessage = JsonConvert.SerializeObject(dataSend);

        ////            var virtualMachines = db.VirtualMachines.Where(vm => vm.VEProfileID == veprofileId && vm.UserID == userId).SingleOrDefault();
                                        
        ////            virtualMachines.IsStarted = 6;

        ////            db.Entry(virtualMachines).State = EntityState.Modified;

        ////            db.SaveChanges();

        ////            ApiCall("POST", FAApiUrl, ShutdownMachine, dataMessage);

        ////            //await ApiCall("POST", "https://fa-api.azurewebsites.net/api/", "ShutdownMachine?code=raQHS1WKPyAdzLFHjNeg8uXAg0nlGiGSi9kV93rzaOuoL8OgD5aP8w==", dataMessage);
        ////        }

        ////        return Ok();
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        return Ok("Error");
        ////    }                
        ////}

        //[HttpPost]
        //[Route("TimeTrigger")]
        //public IHttpActionResult TimeTrigger(int UserId, int VEProfileId, DateTime DateTime, string Mode)
        //{
        //    try
        //    {
        //        CloudLabsSchedule schedule = db.CloudLabsSchedule.Where(s => s.UserId == UserId && s.VEProfileID == VEProfileId).FirstOrDefault();
        //        VirtualMachine virtualmachine = db.VirtualMachines.Where(q => q.UserID == UserId && q.VEProfileID == VEProfileId).FirstOrDefault();

        //        if (Mode == "Start")
        //        {
        //            schedule.StartLabTriggerTime = DateTime;
        //            virtualmachine.DateStartedTrigger = DateTime;
        //        }
        //        else if(Mode == "Render")
        //        {
        //            schedule.RenderPageTriggerTime = DateTime;
        //            schedule.StartLabTriggerTime = DateTime;
        //        }

        //        db.Entry(schedule).State = EntityState.Modified;
        //        db.SaveChanges();

        //        return Ok();
        //    }
        //    catch (Exception e)
        //    {
        //        return Ok("Error");
        //    }
        //}

        ////[HttpGet]
        ////[Route("ExpiredVM")]
        ////public IHttpActionResult ExpiredVM()
        ////{
        ////    var users = db.CloudLabUsers
        ////        .Join(db.VirtualMachines,
        ////        x => x.UserId,
        ////        y => y.UserID,
        ////        (x, y) => new { clu = x, vm = y })
        ////        .Select(s => new
        ////        {
        ////            s.clu,
        ////            s.vm
        ////        })
        ////        .Join(db.CloudLabsGroups,
        ////        e => e.clu.UserGroup,
        ////        r => r.CloudLabsGroupID,
        ////        (e, r) => new { cluVM = e, ug = r })
        ////        .Select(y => new {
        ////            y.cluVM,
        ////            y.ug.GroupName,
        ////            y.ug.CloudLabsGroupID
        ////        })
        ////        .Join(db.VirtualMachineMappings,
        ////        v => new { v.cluVM.vm.UserID, v.cluVM.vm.VEProfileID },
        ////        r => new { r.UserID, r.VEProfileID },
        ////        (v, r) => new { cluVMVmm = v, vmm = r })
        ////        .Select(h => new
        ////        {
        ////            h.cluVMVmm,
        ////            h.vmm.RoleName
        ////        })
        ////        .Join(db.VEProfiles,
        ////        l => new { l.cluVMVmm.cluVM.vm.VEProfileID },
        ////        b => new { b.VEProfileID },
        ////        (l, b) => new { cluVMVmmVe = l, ve = b })
        ////        .Select(g => new {
        ////            g.cluVMVmmVe,
        ////            g.ve.VEProfileID,
        ////            g.ve.Name
        ////        })
        ////        .ToList()
        ////        .OrderBy(w => w.cluVMVmmVe.cluVMVmm.cluVM.clu.FirstName + " " + w.cluVMVmmVe.cluVMVmm.cluVM.clu.LastName)
        ////        .Select(k => new
        ////        {
        ////            DaysRemaining = 180 - Math.Ceiling((DateTime.Today - k.cluVMVmmVe.cluVMVmm.cluVM.vm.DateCreated.Date).TotalDays),
        ////            Name = k.cluVMVmmVe.cluVMVmm.cluVM.clu.FirstName + " " + k.cluVMVmmVe.cluVMVmm.cluVM.clu.LastName,
        ////            UserId = k.cluVMVmmVe.cluVMVmm.cluVM.clu.UserId,
        ////            Email = k.cluVMVmmVe.cluVMVmm.cluVM.clu.Email,
        ////            UserGroup = k.cluVMVmmVe.cluVMVmm.CloudLabsGroupID,
        ////            UserGroupName = k.cluVMVmmVe.cluVMVmm.GroupName,
        ////            VEProfileId = k.VEProfileID,
        ////            ResourceGroup = "CS-" + k.cluVMVmmVe.RoleName.Substring(0, 7),
        ////            Rolename = k.cluVMVmmVe.RoleName,
        ////            VEName = k.Name,
        ////            DaysRemainingDemo = 10 - Math.Ceiling((DateTime.Today - k.cluVMVmmVe.cluVMVmm.cluVM.vm.DateCreated.Date).TotalDays),
        ////        }).OrderBy(q => q.VEName).ToList();

        ////    //.Join(db.CloudLabsSchedule,
        ////    //g => new { g.Virtualmachines.VEProfileID, g.CloudlabUsers.UserId },
        ////    //w => new { w.VEProfileID, w.UserId },
        ////    //(g, w) => new { Query = g, CloudLabsSchedule = w })
        ////    //.Select(d => new
        ////    //{
        ////    //    d
        ////    //})




        ////    return Ok(users);

        ////}

    }
}