using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using WinLabAssessment;

namespace CloudSwyft.Web.Api.Controllers
{
    [RoutePrefix("api/LabAssessments")]
    public class LabAssessmentController : ApiController
    {
        public ArrayList GetLabAssesment(int veProfileId)
        {
            ArrayList labAssessments = new ArrayList();
            SqlConnection conDatabase = new SqlConnection(WebConfigurationManager.ConnectionStrings["VirtualEnvironmentDbContext"].ToString());
            conDatabase.Open();

            string sql = "SELECT LabAssessment from LabAssessments where veProfileId = " + veProfileId.ToString();
            SqlCommand dbCommand = new SqlCommand(sql, conDatabase);
            SqlDataReader dataReader = dbCommand.ExecuteReader();

            while (dataReader.Read())
                labAssessments.Add(dataReader["LabAssessments"]);

            return labAssessments;
        }
    }
}
