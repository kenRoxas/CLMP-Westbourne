using CloudSwyft.Web.Api.DataAccess;
using CloudSwyft.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSwyft.Web.Api.Managers
{
    public class LabHourExtensionManager : ILabHourExtensionManager
    {
        private readonly ILabHourExtensionDataAccess _labHourExtensionDataAccess;

        public LabHourExtensionManager(ILabHourExtensionDataAccess labHourExtensionDataAccess)
        {
            _labHourExtensionDataAccess = labHourExtensionDataAccess ?? new LabHourExtensionDataAccess();
        }

        public IEnumerable<LabHourExtensionType> GetLabHourExtensionTypes()
        {
            try
            {
                return _labHourExtensionDataAccess.GetLabHourExtensionTypes();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to retrieve list of lab hour extension types", ex);
            }
        }

        public void SaveLabHourExtension(SaveExtensionRequest request)
        {
            try
            {
                if (request.UserId.HasValue)
                {
                    _labHourExtensionDataAccess.SaveUserLabHourExtension(request);
                }
                else
                {
                    _labHourExtensionDataAccess.SaveCourseLabHourExtension(request);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Saving of lab hour extension failed.", ex);
            }
        }

        /// <summary>
        /// NOTE: To be removed
        /// </summary>
        /// <param name="veProfileId"></param>
        /// <returns></returns>
        public IEnumerable<UserLabHourExtension> GetUsersWithLabHourExtensions(int veProfileId)
        {
            try
            {
                return _labHourExtensionDataAccess.GetUsersWithLabHourExtensions(veProfileId);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to retrieve users with lab hour extensions", ex);
            }
        }

        public IEnumerable<UserLabHourExtension> GetUsersWithFixedLabHourExtensions(GetUsersWithLabHourExtensionsRequest request)
        {
            try
            {
                return _labHourExtensionDataAccess.GetUsersWithFixedLabHourExtensions(request);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to retrieve users with lab hour extensions", ex);
            }
        }

        public IEnumerable<UserLabHourExtension> GetUsersWithCustomLabHourExtensions(GetUsersWithLabHourExtensionsRequest request)
        {
            try
            {
                return _labHourExtensionDataAccess.GetUsersWithCustomLabHourExtensions(request);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to retrieve users with lab hour extensions", ex);
            }
        }

        public void DeleteLabHourExtensionById(DeleteLabHourExtensionRequest request)
        {
            try
            {
                _labHourExtensionDataAccess.DeleteLabHourExtensionById(request);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occured on {nameof(DeleteLabHourExtensionById)}. Unable to delete lab hour extension", ex);
            }
        }

        public void UpdateLabHourExtension(SaveExtensionRequest request)
        {
            try
            {
                _labHourExtensionDataAccess.UpdateLabHourExtension(request);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occured on {nameof(UpdateLabHourExtension)}. Unable to update lab hour extension", ex);
            }
        }

        public void UpdateLabHourExtensionV2(UpdateExtensionRequest request)
        {
            try
            {
                _labHourExtensionDataAccess.UpdateLabHourExtensionV2(request);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occured on {nameof(UpdateLabHourExtension)}. Unable to update lab hour extension", ex);
            }
        }
    }
}