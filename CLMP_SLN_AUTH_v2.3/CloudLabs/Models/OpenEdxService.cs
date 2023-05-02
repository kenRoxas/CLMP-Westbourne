using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CloudSwyft.CloudLabs.Models
{
    public class OpenEdxService
    {
        private HttpClient client { get; set; }

        public OpenEdxService(OpenEdxToken token)
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);

        }

        public async Task<OpenEdxUser> getUser(string userid)
        {

            OpenEdxUser user = new OpenEdxUser();

            var uri = new Uri(Constants.HOSTNAME + "/learn/api/public/v1/users" + "/" + userid);
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<OpenEdxUser>(content);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return user;

        }
        //Check if the user is enrolled in the course
        //public async Task<CourseMembership> getCourseMemberShip(string courseid, string userid)
        //{

        //    CourseMembership cm = new CourseMembership();

        //    var uri = new Uri(Constants.HOSTNAME + "/learn/api/public/v1/courses/" + courseid + "/users/" + userid);
        //    try
        //    {
        //        var response = await client.GetAsync(uri);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var content = await response.Content.ReadAsStringAsync();
        //            cm = JsonConvert.DeserializeObject<CourseMembership>(content);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }

        //    return cm;

        //}

        public async Task<VeProfile> GetVEProfile(int courseId, string apiUrl)
        {
            var ve = new VeProfile();

            var uri = new Uri(apiUrl + "/VeProfiles/ByCourse/?courseID=" + courseId);
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ve = JsonConvert.DeserializeObject<VeProfile>(content);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ve;
        }

        public async Task SaveGrade(int courseid, int userid, double grade, int contentid, string apiUrl)
        {
            var uri = new Uri(apiUrl + "/User/SaveGrade?courseid=" + courseid + "&userid=" + userid + "&contentid=" + contentid + "&grade=" + grade);
            await client.GetAsync(uri);
        }


        //gets all students enrolled in the course
        public async Task<bool> SaveNewUser(OpenEdxUser oeu, string apiUrl)
        {
            var uri = new Uri(apiUrl + "/User/NewBBUser");

            try
            {
                var json = JsonConvert.SerializeObject(oeu);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    //put success message here or whatever
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return false;

        }


        //public async Task<VeProfile> ProvisionUsers(List<CourseMembership> members, int veid, string apiUrl)
        //{
        //    var ve = new VeProfile();
        //    var vm = new UserVEViewModel();
        //    var uri = new Uri(apiUrl + "/VeProfiles/ProvisionUsersBB");
        //    try
        //    {
        //        ve.VEProfileID = veid;
        //        vm.Users = members;
        //        vm.VeProfile = ve;

        //        var json = JsonConvert.SerializeObject(vm);

        //        var body = new StringContent(json, Encoding.UTF8, "application/json");

        //        HttpResponseMessage response = await client.PostAsync(uri, body);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            return ve;
        //            //var content = await response.Content.ReadAsStringAsync();
        //            //JObject jsonreturn = JObject.Parse(content);

        //            //datasource = JsonConvert.DeserializeObject<Datasource>(content);
        //            //Debug.WriteLine(@" Datasource successfully created.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //    return ve;
        //}

        //Get course data for BB grading
        //public async Task<BBUserLRA> getGradingDetails(int courseid, int userid, int contentid, string apiUrl)
        //{
        //    var lraUser = new BBUserLRA();
        //    var uri = new Uri(apiUrl + "/VirtualMachines/BBUsersCourse?courseId=" + courseid + "&userid=" + userid + "&contentid=" + contentid);

        //    try
        //    {
        //        HttpResponseMessage response = await client.GetAsync(uri);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var content = await response.Content.ReadAsStringAsync();
        //            var listlra = JsonConvert.DeserializeObject<List<BBUserLRA>>(content);
        //            return listlra[0];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //    return lraUser;
        //}
    }

}