using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Services;
using Oqtane.Shared;

namespace GIBS.Module.Resource.Services
{
    public class ClientResourceAvailabilityService : ServiceBase, IResourceAvailabilityService
    {
        public ClientResourceAvailabilityService(HttpClient http, SiteState siteState) : base(http, siteState)
        {
        }

        private string Apiurl => CreateApiUrl("ResourceAvailability");

        public async Task<List<Models.ResourceAvailability>> GetAvailabilitiesAsync(int resourceId, int moduleId)
        {
            var availabilities = await GetJsonAsync<List<Models.ResourceAvailability>>(
                CreateAuthorizationPolicyUrl($"{Apiurl}/resource/{resourceId}/{moduleId}", EntityNames.Module, moduleId),
                Enumerable.Empty<Models.ResourceAvailability>().ToList());

            return availabilities
                .OrderBy(item => item.DayOfWeek)
                .ThenBy(item => item.StartTime)
                .ToList();
        }

        public async Task<Models.ResourceAvailability> GetAvailabilityAsync(int availabilityId, int moduleId)
        {
            return await GetJsonAsync<Models.ResourceAvailability>(
                CreateAuthorizationPolicyUrl($"{Apiurl}/{availabilityId}/{moduleId}", EntityNames.Module, moduleId));
        }

        public async Task<Models.ResourceAvailability> AddAvailabilityAsync(Models.ResourceAvailability availability, int moduleId)
        {
            return await PostJsonAsync<Models.ResourceAvailability>(
                CreateAuthorizationPolicyUrl($"{Apiurl}/{moduleId}", EntityNames.Module, moduleId),
                availability);
        }

        public async Task<Models.ResourceAvailability> UpdateAvailabilityAsync(Models.ResourceAvailability availability, int moduleId)
        {
            return await PutJsonAsync<Models.ResourceAvailability>(
                CreateAuthorizationPolicyUrl($"{Apiurl}/{availability.AvailabilityId}/{moduleId}", EntityNames.Module, moduleId),
                availability);
        }

        public async Task DeleteAvailabilityAsync(int availabilityId, int moduleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{availabilityId}/{moduleId}", EntityNames.Module, moduleId));
        }
    }
}
