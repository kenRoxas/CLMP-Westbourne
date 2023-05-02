using CloudSwyft.Web.Api.Models;
using System.Collections.Generic;

namespace CloudSwyft.Web.Api.DataAccess
{
    public interface ILabHourExtensionDataAccess
    {
        IEnumerable<LabHourExtensionType> GetLabHourExtensionTypes();
        void SaveCourseLabHourExtension(SaveExtensionRequest request);
        void SaveUserLabHourExtension(SaveExtensionRequest request);
        IEnumerable<UserLabHourExtension> GetUsersWithLabHourExtensions(int veProfileId);
        IEnumerable<UserLabHourExtension> GetUsersWithFixedLabHourExtensions(GetUsersWithLabHourExtensionsRequest request);
        IEnumerable<UserLabHourExtension> GetUsersWithCustomLabHourExtensions(GetUsersWithLabHourExtensionsRequest request);
        void DeleteLabHourExtensionById(DeleteLabHourExtensionRequest request);
        void UpdateLabHourExtension(SaveExtensionRequest request);
        void UpdateLabHourExtensionV2(UpdateExtensionRequest request);
    }
}
