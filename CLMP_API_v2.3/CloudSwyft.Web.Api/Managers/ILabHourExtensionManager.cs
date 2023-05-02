using CloudSwyft.Web.Api.Models;
using System.Collections.Generic;

namespace CloudSwyft.Web.Api.Managers
{
    public interface ILabHourExtensionManager
    {
        IEnumerable<LabHourExtensionType> GetLabHourExtensionTypes();
        void SaveLabHourExtension(SaveExtensionRequest request);
        IEnumerable<UserLabHourExtension> GetUsersWithLabHourExtensions(int veProfileId);
        IEnumerable<UserLabHourExtension> GetUsersWithFixedLabHourExtensions(GetUsersWithLabHourExtensionsRequest request);
        IEnumerable<UserLabHourExtension> GetUsersWithCustomLabHourExtensions(GetUsersWithLabHourExtensionsRequest request);
        void DeleteLabHourExtensionById(DeleteLabHourExtensionRequest request);
        void UpdateLabHourExtension(SaveExtensionRequest request);
        void UpdateLabHourExtensionV2(UpdateExtensionRequest request);
    }
}
