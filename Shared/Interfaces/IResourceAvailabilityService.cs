using System.Collections.Generic;
using System.Threading.Tasks;

namespace GIBS.Module.Resource.Services
{
    public interface IResourceAvailabilityService
    {
        Task<List<Models.ResourceAvailability>> GetAvailabilitiesAsync(int resourceId, int moduleId);

        Task<Models.ResourceAvailability> GetAvailabilityAsync(int availabilityId, int moduleId);

        Task<Models.ResourceAvailability> AddAvailabilityAsync(Models.ResourceAvailability availability, int moduleId);

        Task<Models.ResourceAvailability> UpdateAvailabilityAsync(Models.ResourceAvailability availability, int moduleId);

        Task DeleteAvailabilityAsync(int availabilityId, int moduleId);
    }
}
