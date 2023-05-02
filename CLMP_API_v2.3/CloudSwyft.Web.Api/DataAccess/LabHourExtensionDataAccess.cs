using CloudSwyft.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSwyft.Web.Api.DataAccess
{
    public class LabHourExtensionDataAccess : ILabHourExtensionDataAccess
    {
        public LabHourExtensionDataAccess()
        {
        }

        public IEnumerable<LabHourExtensionType> GetLabHourExtensionTypes()
        {
            using (var context = new VirtualEnvironmentDbContext())
            {
                try
                {
                    return context.LabHourExtensionTypes.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception($"{nameof(GetLabHourExtensionTypes)} failed. No records where retrieved.", ex);
                }
            }
        }

        public IEnumerable<UserLabHourExtension> GetUsersWithLabHourExtensions(int veProfileId)
        {
            using (var context = new VirtualEnvironmentDbContext())
            {
                try
                {
                    var users = from cu in context.CloudLabUsers
                                join m in context.MachineLabs on cu.UserId equals m.UserId
                                join cls in context.CloudLabsSchedule on m.MachineLabsId equals cls.MachineLabsId
                                join l in context.LabHourExtensions on cu.UserId equals l.UserId into labHourExts
                                from labHourExt in labHourExts.Where(l => l.IsDeleted == false && l.VEProfileId == veProfileId).DefaultIfEmpty()
                                where m.VEProfileId == veProfileId
                                orderby cu.LastName, cu.FirstName, labHourExt.StartDate
                                select new UserLabHourExtension
                                {
                                    UserId = cu.UserId,
                                    VEProfileId = m.VEProfileId,
                                    Name = cu.LastName + ", " + cu.FirstName,
                                    Email = cu.Email,
                                    ExtensionTypeId = labHourExt.ExtensionTypeId,
                                    LabHourExtensionId = labHourExt.Id,
                                    StartDate = labHourExt.StartDate,
                                    EndDate = labHourExt.EndDate
                                };

                    return users.AsEnumerable().ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception($"{nameof(GetUsersWithLabHourExtensions)} failed. No records where retrieved for VeProfileId {veProfileId}.", ex);
                }
            }
        }

        public IEnumerable<UserLabHourExtension> GetUsersWithFixedLabHourExtensions(GetUsersWithLabHourExtensionsRequest request)
        {
            return GetUsersWithLabHourExtensions(request, true, nameof(GetUsersWithFixedLabHourExtensions));
        }

        public IEnumerable<UserLabHourExtension> GetUsersWithCustomLabHourExtensions(GetUsersWithLabHourExtensionsRequest request)
        {
            return GetUsersWithLabHourExtensions(request, false, nameof(GetUsersWithCustomLabHourExtensions));
        }

        private IEnumerable<UserLabHourExtension> GetUsersWithLabHourExtensions(GetUsersWithLabHourExtensionsRequest request, bool isFixedLabHourExtension, string methodName)
        {
            using (var context = new VirtualEnvironmentDbContext())
            {
                try
                {
                    var labHourExtensions = from l in context.LabHourExtensions
                                             where l.VEProfileId == request.VEProfileId
                                             select l;

                    //filter out deleted records
                    if (!request.ShowAllRecords)
                    {
                        labHourExtensions = labHourExtensions.Where(l => l.IsDeleted == false);
                    }

                    if (isFixedLabHourExtension)
                    {
                        labHourExtensions = labHourExtensions.Where(l => l.IsFixedLabHourExtension != null && l.IsFixedLabHourExtension == true);
                    }
                    else 
                    {
                        labHourExtensions = labHourExtensions.Where(l => l.IsFixedLabHourExtension == null || l.IsFixedLabHourExtension.Value == false);
                    }

                    //filter by date range
                    if (request.StartDate.HasValue && !request.EndDate.HasValue)
                    {
                        DateTime startDate = request.StartDate.Value.ToUniversalTime();
                        labHourExtensions = labHourExtensions.Where(l => l.StartDate >= startDate || l.EndDate >= startDate );
                    }
                    else if (request.StartDate.HasValue && request.EndDate.HasValue)
                    {
                        DateTime startDate = request.StartDate.Value.ToUniversalTime();
                        DateTime endDate = request.EndDate.Value.ToUniversalTime();
                        labHourExtensions = labHourExtensions
                            .Where(l => (l.StartDate >= startDate && l.StartDate <= endDate) ||
                                        (l.EndDate >= startDate && l.EndDate <= endDate)
                                    );
                    }

                    //filter by name or email
                    var cloudLabUsers = from cu in context.CloudLabUsers
                                         join m in context.MachineLabs on cu.UserId equals m.UserId
                                         join cls in context.CloudLabsSchedule on m.MachineLabsId equals cls.MachineLabsId
                                         where m.VEProfileId == request.VEProfileId
                                         select new {
                                             FirstName = cu.FirstName,
                                             LastName = cu.LastName,
                                             Email = cu.Email,
                                             UserId = cu.UserId,
                                             VEProfileId = m.VEProfileId
                                         };

                    if (!string.IsNullOrWhiteSpace(request.SearchText.Trim()))
                    {
                        var searchText = request.SearchText.Trim();
                        cloudLabUsers = cloudLabUsers.Where(c => c.FirstName.Contains(searchText) ||  c.LastName.Contains(searchText) || c.Email.Contains(searchText));
                    }

                    var users = from cu in cloudLabUsers
                                join l in labHourExtensions on cu.UserId equals l.UserId into labHourExtGroup
                                from labHourExt in labHourExtGroup.DefaultIfEmpty()
                                join creator in context.CloudLabUsers on labHourExt.CreatedByUserId equals creator.UserId into createdByGroup
                                from createdBy in createdByGroup.DefaultIfEmpty()
                                select new UserLabHourExtension
                                {
                                    UserId = cu.UserId,
                                    VEProfileId = cu.VEProfileId,
                                    Name = cu.LastName + ", " + cu.FirstName,
                                    Email = cu.Email,
                                    ExtensionTypeId = labHourExt.ExtensionTypeId,
                                    LabHourExtensionId = labHourExt.Id,
                                    StartDate = labHourExt.StartDate,
                                    EndDate = labHourExt.EndDate,
                                    IsDeleted = labHourExt.IsDeleted,
                                    TotalHours = labHourExt.TotalHours,
                                    IsFixedLabHourExtension = labHourExt.IsFixedLabHourExtension,
                                    CreatedBy = createdBy.LastName + ", " + createdBy.FirstName
                                };

                    if (!string.IsNullOrWhiteSpace(request.SortField.Trim()))
                    {
                        switch (request.SortField.Trim())
                        {
                            case "Name":
                                if (request.SortDirection == "asc")
                                {
                                    users = users.OrderBy(u => u.Name);
                                }
                                else
                                {
                                    users = users.OrderByDescending(u => u.Name);
                                }
                                break;
                            case "Email":
                                if (request.SortDirection == "asc")
                                {
                                    users = users.OrderBy(u => u.Email);
                                }
                                else
                                {
                                    users = users.OrderByDescending(u => u.Email);
                                }
                                break;
                            case "ExtensionTypeId":
                                if (request.SortDirection == "asc")
                                {
                                    users = users.OrderBy(u => u.ExtensionTypeId);
                                }
                                else
                                {
                                    users = users.OrderByDescending(u => u.ExtensionTypeId);
                                }
                                break;
                            case "StartDate":
                                if (request.SortDirection == "asc")
                                {
                                    users = users.OrderBy(u => u.StartDate);
                                }
                                else
                                {
                                    users = users.OrderByDescending(u => u.StartDate);
                                }
                                break;
                            case "EndDate":
                                if (request.SortDirection == "asc")
                                {
                                    users = users.OrderBy(u => u.EndDate);
                                }
                                else
                                {
                                    users = users.OrderByDescending(u => u.EndDate);
                                }
                                break;
                            default:
                                users = users.OrderBy(u => u.Name).ThenBy(u => u.StartDate);
                                break;
                        }
                    }

                    return users.AsEnumerable().ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception($"{methodName} failed. No records where retrieved.", ex);
                }
            }
        }

        public void SaveCourseLabHourExtension(SaveExtensionRequest request)
        {
            using (var context = new VirtualEnvironmentDbContext())
            {
                try
                {
                    //delete overlaps
                    if (request.LabHourExtensionIds?.Any() == true)
                    {
                        var labHourExtensions = context.LabHourExtensions
                            .Where(l => l.VEProfileId == request.VEProfileId && request.LabHourExtensionIds.Any(ex => ex == l.Id))
                            .ToList();
                        foreach (var extension in labHourExtensions)
                        {
                            context.LabHourExtensions.Remove(extension);
                            context.SaveChanges();
                        }
                    }

                    //create
                    var users = context.MachineLabs.Where(q => q.VEProfileId == request.VEProfileId).ToList()
                        .Join(context.CloudLabsSchedule,
                        a => a.MachineLabsId,
                        b => b.MachineLabsId,
                        (a, b) => new { a, b }).Select(q => q.a).ToList();

                    foreach (var user in users)
                    {
                        LabHourExtension labHourExtension = new LabHourExtension();
                        int schedId = context.CloudLabsSchedule
                            .Where(q => q.UserId == user.UserId && q.VEProfileID == request.VEProfileId)
                            .FirstOrDefault()
                            .CloudLabsScheduleId;

                        labHourExtension.EndDate = request.EndDate.ToUniversalTime();
                        labHourExtension.StartDate = request.StartDate.ToUniversalTime();
                        labHourExtension.CreatedByUserId = request.CreatedByUserId;
                        labHourExtension.VEProfileId = request.VEProfileId;
                        labHourExtension.ExtensionTypeId = request.ExtensionTypeId;
                        labHourExtension.UserId = user.UserId;
                        labHourExtension.CloudLabsScheduleId = schedId;
                        labHourExtension.IsFixedLabHourExtension = request.IsFixedLabHourExtension;

                        if (request.IsFixedLabHourExtension)
                        {
                            labHourExtension.TimeRemaining = (double)(request.TotalHours.Value * 60);
                            labHourExtension.TotalHours = request.TotalHours;
                        }
                        else
                        {
                            labHourExtension.TimeRemaining = (
                                    labHourExtension
                                    .EndDate
                                    .AddSeconds(-labHourExtension.EndDate.Second)
                                    .AddMilliseconds(-labHourExtension.EndDate.Millisecond)
                                    -
                                    labHourExtension
                                    .StartDate
                                    .AddSeconds(-labHourExtension.StartDate.Second)
                                    .AddMilliseconds(-labHourExtension.StartDate.Millisecond))
                                    .TotalSeconds;
                        }

                        labHourExtension.DbTimeStamp = DateTime.UtcNow;

                        context.LabHourExtensions.Add(labHourExtension);
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"{nameof(SaveCourseLabHourExtension)} failed.", ex);
                }
            }
        }

        public void SaveUserLabHourExtension(SaveExtensionRequest request)
        {
            using (var context = new VirtualEnvironmentDbContext())
            {
                try
                {
                    //delete overlaps
                    if (request.LabHourExtensionIds?.Any() == true)
                    {
                        var labHourExtensions = context.LabHourExtensions
                            .Where(l => l.VEProfileId == request.VEProfileId && request.LabHourExtensionIds.Any(ex => ex == l.Id))
                            .ToList();
                        foreach (var extension in labHourExtensions)
                        {
                            context.LabHourExtensions.Remove(extension);
                            context.SaveChanges();
                        }
                    }

                    LabHourExtension userLabHourExtension = new LabHourExtension();
                    int schedId = context.CloudLabsSchedule
                        .Where(q => q.UserId == request.UserId.Value && q.VEProfileID == request.VEProfileId)
                        .FirstOrDefault()
                        .CloudLabsScheduleId;

                    userLabHourExtension.EndDate = request.EndDate.ToUniversalTime();
                    userLabHourExtension.StartDate = request.StartDate.ToUniversalTime();
                    userLabHourExtension.CreatedByUserId = request.CreatedByUserId;
                    userLabHourExtension.VEProfileId = request.VEProfileId;
                    userLabHourExtension.ExtensionTypeId = request.ExtensionTypeId;
                    userLabHourExtension.UserId = request.UserId.Value;
                    userLabHourExtension.CloudLabsScheduleId = schedId;
                    userLabHourExtension.IsFixedLabHourExtension = request.IsFixedLabHourExtension;
                    if (request.IsFixedLabHourExtension)
                    {
                        userLabHourExtension.TimeRemaining = (double)(request.TotalHours.Value * 60);
                        userLabHourExtension.TotalHours = request.TotalHours;
                    }
                    else
                    {
                        userLabHourExtension.TimeRemaining = (
                                userLabHourExtension
                                .EndDate
                                .AddSeconds(-userLabHourExtension.EndDate.Second)
                                .AddMilliseconds(-userLabHourExtension.EndDate.Millisecond)
                                -
                                userLabHourExtension
                                .StartDate
                                .AddSeconds(-userLabHourExtension.StartDate.Second)
                                .AddMilliseconds(-userLabHourExtension.StartDate.Millisecond))
                                .TotalSeconds;
                    }

                    userLabHourExtension.DbTimeStamp = DateTime.UtcNow;

                    context.LabHourExtensions.Add(userLabHourExtension);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"{nameof(SaveUserLabHourExtension)} failed.", ex);
                }
            }
        }

        public void DeleteLabHourExtensionById(DeleteLabHourExtensionRequest request)
        {
            using (var context = new VirtualEnvironmentDbContext())
            {
                try
                {
                    LabHourExtension labHourExtension = context.LabHourExtensions.Where(le => le.Id == request.LabHourExtensionId).FirstOrDefault();

                    if (labHourExtension != null)
                    {
                        labHourExtension.EditedByUserId = request.EditedByUserId;
                        labHourExtension.IsDeleted = true;

                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    if (!context.LabHourExtensions.Any(i => i.Id == request.LabHourExtensionId))
                    {
                        throw new Exception($"{nameof(DeleteLabHourExtensionById)} failed. Lab hour extension record with Id {request.LabHourExtensionId} is not found.");
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
        }

        public void UpdateLabHourExtension(SaveExtensionRequest request)
        {
            using (var context = new VirtualEnvironmentDbContext())
            {
                try
                {
                    if (request.LabHourExtensionIds?.Any() == true)
                    {
                        int labHourExtId = request.LabHourExtensionIds.First();

                        LabHourExtension labHourExtension = context.LabHourExtensions
                            .Where(l => l.VEProfileId == request.VEProfileId && l.Id == labHourExtId)
                            .FirstOrDefault();

                        if (labHourExtension != null)
                        {
                            labHourExtension.StartDate = request.StartDate.ToUniversalTime();
                            labHourExtension.EndDate = request.EndDate.ToUniversalTime();
                            labHourExtension.ExtensionTypeId = request.ExtensionTypeId;
                            labHourExtension.IsFixedLabHourExtension = labHourExtension.IsFixedLabHourExtension;

                            if (request.IsFixedLabHourExtension)
                            {
                                labHourExtension.TimeRemaining = (double)(request.TotalHours.Value * 60);
                                labHourExtension.TotalHours = request.TotalHours;
                            }
                            else
                            {
                                labHourExtension.TimeRemaining = (
                                        labHourExtension
                                        .EndDate
                                        .AddSeconds(-labHourExtension.EndDate.Second)
                                        .AddMilliseconds(-labHourExtension.EndDate.Millisecond)
                                        -
                                        labHourExtension
                                        .StartDate
                                        .AddSeconds(-labHourExtension.StartDate.Second)
                                        .AddMilliseconds(-labHourExtension.StartDate.Millisecond))
                                        .TotalSeconds;
                            }
                            labHourExtension.EditedByUserId = request.EditedByUserId.Value;

                            context.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"{nameof(UpdateLabHourExtension)} failed.", ex);
                }
            }
        }

        public void UpdateLabHourExtensionV2(UpdateExtensionRequest request)
        {
            using (var context = new VirtualEnvironmentDbContext())
            {
                try
                {
                    //delete overlaps
                    if (request.OverlappingExtensionIds?.Any() == true)
                    {
                        var labHourExtensions = context.LabHourExtensions
                            .Where(l => l.VEProfileId == request.VEProfileId && request.OverlappingExtensionIds.Any(ex => ex == l.Id))
                            .ToList();
                        foreach (var extension in labHourExtensions)
                        {
                            context.LabHourExtensions.Remove(extension);
                            context.SaveChanges();
                        }
                    }

                    LabHourExtension labHourExtension = context.LabHourExtensions
                        .Where(l => l.VEProfileId == request.VEProfileId && l.Id == request.LabHourExtensionId)
                        .FirstOrDefault();

                    if (labHourExtension != null)
                    {
                        labHourExtension.StartDate = request.StartDate.ToUniversalTime();
                        labHourExtension.EndDate = request.EndDate.ToUniversalTime();
                        labHourExtension.ExtensionTypeId = request.ExtensionTypeId;
                        labHourExtension.IsFixedLabHourExtension = labHourExtension.IsFixedLabHourExtension;

                        if (request.IsFixedLabHourExtension)
                        {
                            labHourExtension.TimeRemaining = (double)(request.TotalHours.Value * 60);
                            labHourExtension.TotalHours = request.TotalHours;
                        }
                        else
                        {
                            labHourExtension.TimeRemaining = (
                                    labHourExtension
                                    .EndDate
                                    .AddSeconds(-labHourExtension.EndDate.Second)
                                    .AddMilliseconds(-labHourExtension.EndDate.Millisecond)
                                    -
                                    labHourExtension
                                    .StartDate
                                    .AddSeconds(-labHourExtension.StartDate.Second)
                                    .AddMilliseconds(-labHourExtension.StartDate.Millisecond))
                                    .TotalSeconds;
                        }
                        labHourExtension.EditedByUserId = request.EditedByUserId.Value;

                        context.SaveChanges();
                    }
                    
                }
                catch (Exception ex)
                {
                    throw new Exception($"{nameof(UpdateLabHourExtension)} failed.", ex);
                }
            }
        }
    }
}