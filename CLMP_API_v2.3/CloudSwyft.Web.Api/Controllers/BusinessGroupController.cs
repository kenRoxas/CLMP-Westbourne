using CloudSwyft.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CloudSwyft.Web.Api.Controllers
{
    [RoutePrefix("api/BusinessGroup")]

    public class BusinessGroupController : ApiController
    {
        private VirtualEnvironmentDbContext _db = new VirtualEnvironmentDbContext();
        private VirtualEnvironmentDBTenantContext _dbTenant = new VirtualEnvironmentDBTenantContext();

        // GET: BusinessGroup
        [HttpGet]
        [Route("GetBusinessGroup")]
        public IHttpActionResult GetBusinessGroup()
        {
            var userGroupBusiness = _db.CloudLabsGroups.GroupJoin(_db.BusinessGroups, a => a.CloudLabsGroupID, b => b.UserGroupId, (a, b) => new { a, b })
                .Select(w => new BusinessGroupModel
                {
                    UserGroupName = w.a.GroupName,
                    ModifiedValidity = w.b.Any(q => q.UserGroupId == w.a.CloudLabsGroupID) ? w.b.Where(q => q.UserGroupId == w.a.CloudLabsGroupID).FirstOrDefault().ModifiedValidity : null,
                    BusinessType = w.b.Any(q => q.UserGroupId == w.a.CloudLabsGroupID) ?
                    w.b.Where(q => q.UserGroupId == w.a.CloudLabsGroupID).Join(_db.BusinessTypes, p => p.BusinessTypeId, l => l.BusinessId, (p, l) => new { p, l}).FirstOrDefault().l.BusinessType : null                    
                }).ToList();

            return Ok(userGroupBusiness);
        }

        [HttpPost]
        [Route("SaveBusinessGroup")]
        public IHttpActionResult SaveBusinessGroup(CreateEditBusinessGroup bg)
        {
            try
            {
                if (_db.CloudLabsGroups.Any(q => q.GroupName == bg.UserGroupName))
                {
                    var userGroupId = _db.CloudLabsGroups.Where(q => q.GroupName == bg.UserGroupName).FirstOrDefault().CloudLabsGroupID;
                    var tenantId = _db.CloudLabsGroups.Where(q => q.GroupName == bg.UserGroupName).FirstOrDefault().TenantId;
                    var createdByUserId = _db.CloudLabUsers.Where(q => q.Email == bg.CreatedBy).FirstOrDefault().UserId;

                    var businessTypeId = _db.BusinessTypes.Where(q => q.BusinessType == bg.BusinessGroup).FirstOrDefault().BusinessId;

                    var isExist = _db.BusinessGroups.Any(q => q.UserGroupId == userGroupId);

                    var azTenant = _dbTenant.AzTenants.Where(q => q.TenantId == tenantId).FirstOrDefault();

                    if (isExist)
                    {
                        var businessGroup = _db.BusinessGroups.Where(q => q.UserGroupId == userGroupId).FirstOrDefault();

                        businessGroup.ModifiedValidity = bg.ModifiedValidity;
                        businessGroup.CreatedBy = createdByUserId;
                        businessGroup.ModifiedDate = DateTime.UtcNow;
                        businessGroup.UserGroupId = userGroupId;
                        businessGroup.BusinessTypeId = businessTypeId;
                        _db.Entry(businessGroup).State = EntityState.Modified;
                        _db.SaveChanges();

                        azTenant.BusinessId = businessTypeId;
                        _dbTenant.Entry(azTenant).State = EntityState.Modified;
                        _dbTenant.SaveChanges();
                    }
                    else
                    {
                        BusinessGroups BG = new BusinessGroups();
                        BG.BusinessTypeId = businessTypeId;
                        BG.UserGroupId = userGroupId;
                        BG.CreatedBy = createdByUserId;
                        BG.ModifiedDate = DateTime.UtcNow;
                        BG.ModifiedValidity = bg.ModifiedValidity;

                        _db.BusinessGroups.Add(BG);
                        _db.SaveChanges();

                        azTenant.BusinessId = businessTypeId;
                        _dbTenant.Entry(azTenant).State = EntityState.Modified;
                        _dbTenant.SaveChanges();
                    }

                    return Ok();
                }
                else
                    return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }    
        }

        [HttpGet]
        [Route("CheckIfTheresMachine")]
        public IHttpActionResult CheckIfTheresMachine(string userGroupName)
        {
            return Ok(_db.MachineLabs.Join(_db.CloudLabUsers, a => a.UserId, b => b.UserId, (a, b) => new { a, b })
                .Join(_db.CloudLabsGroups, c=>c.b.UserGroup, d=>d.CloudLabsGroupID, (c,d) => new { c,d}).Any(q => q.d.GroupName.ToLower() == userGroupName.ToLower()));
        }

        [HttpGet]
        [Route("checkIfTheresMachineByClientCode")]
        public IHttpActionResult checkIfTheresMachineByClientCode(string clientCode)
        {
            var tenantId = _dbTenant.AzTenants.Where(q => q.ClientCode == clientCode).FirstOrDefault().TenantId;

            var isOkay = _db.MachineLabs.Join(_db.CloudLabUsers, a => a.UserId, b => b.UserId, (a, b) => new { a, b })
                .Join(_db.CloudLabsGroups, c => c.b.UserGroup, d => d.CloudLabsGroupID, (c, d) => new { c, d }).Any(q => q.d.TenantId == tenantId);

            return Ok(isOkay);
        }
    }
}