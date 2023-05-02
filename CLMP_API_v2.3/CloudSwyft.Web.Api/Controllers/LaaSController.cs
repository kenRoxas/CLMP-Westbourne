using CloudSwyft.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CloudSwyft.Web.Api.Controllers
{
    [RoutePrefix("api/Provision")]
    public class LaaSController : ApiController
    {
        private VirtualEnvironmentDbContext db = new VirtualEnvironmentDbContext();


        //[HttpGet]
        //[Route("CheckAvailProvision")]
        //public int CheckAvailProvision(string CourseCode, string email)
        //{
        //    try
        //    {
        //        var virtualEnvironmentID = db.VirtualEnvironments.Where(x => x.Title == CourseCode).FirstOrDefault().VirtualEnvironmentID;

        //        var veProfileId = db.VEProfiles.Where(x => x.VirtualEnvironmentID == virtualEnvironmentID).FirstOrDefault().VEProfileID;
        //        var userId = db.CloudLabUsers.Where(x => x.Email == email).FirstOrDefault().UserId;
        //        var userGroup = db.CloudLabUsers.Where(x => x.Email == email && x.UserId == userId).FirstOrDefault().UserGroup;

        //        if (db.VirtualMachineMappings.Where(x => x.UserID == userId && x.VEProfileID == veProfileId).FirstOrDefault() == null)
        //        {
        //            //int isLaasProvisioning = db.VirtualMachineMappings.Where(x => x.UserID == userId && x.VEProfileID == veProfileId).FirstOrDefault().IsLaasProvisioned;

        //            bool isNoAvailHours = db.VEProfileLabCreditMappings.Where(x => x.VEProfileID == veProfileId && x.GroupID == userGroup).FirstOrDefault().TotalRemainingCourseHours == 0;

        //            if (isNoAvailHours)
        //                return 0; //error

        //            else
        //                return 1; // can provision
        //        }
        //        else
        //            return 0; // already provision
                
        //    }
        //    catch (Exception e)
        //    {
        //        return 0;
        //    }
        //}

        //[HttpGet]
        //[Route("RequestProvision")]
        //public async Task<int> RequestProvision(string CourseCode, string email)
        //{
        //    try
        //    {
        //        bool isLaas = true;
        //        VEProfilesController veController = new VEProfilesController();

        //        List<User> users = new List<User>();

        //        var virtualEnvironmentID = db.VirtualEnvironments.Where(x => x.Title == CourseCode).FirstOrDefault().VirtualEnvironmentID;
        //        var veProfileId = db.VEProfiles.Where(x => x.VirtualEnvironmentID == virtualEnvironmentID).FirstOrDefault().VEProfileID;
        //        var user = db.CloudLabUsers.Where(x => x.Email == email).FirstOrDefault();
        //        var userGroup = db.CloudLabUsers.Where(x => x.Email == email && x.UserId == user.UserId).FirstOrDefault().UserGroup;
        //        var veProfileLabcreditMappings = db.VEProfileLabCreditMappings.Where(x => x.VEProfileID == veProfileId && x.GroupID == user.UserGroup).FirstOrDefault();

        //        User q = new User()
        //        {
        //            UserId = user.UserId,
        //            Email = user.Email,
        //            labHoursRemaining = Convert.ToInt32(veProfileLabcreditMappings.TotalRemainingCourseHours),
        //            labHoursTotal = Convert.ToInt32(veProfileLabcreditMappings.TotalCourseHours)
        //        };

        //        users.Add(q);

        //        ProvisionMachineDetails s = new ProvisionMachineDetails
        //        {
        //            CLUsers = users,
        //            labCreditMapping = veProfileLabcreditMappings
        //        };

        //        VEProfilesController ve = new VEProfilesController();

        //        await ve.ProvisionMachines(s, isLaas);
        //        return 1;
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}

    }
}