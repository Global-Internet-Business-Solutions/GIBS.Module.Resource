using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Services;
using Oqtane.Shared;

namespace GIBS.Module.Resource.Services
{

    public class ClientResourceService : ServiceBase, IResourceService
    {
        public ClientResourceService(HttpClient http, SiteState siteState) : base(http, siteState) { }

        private string Apiurl => CreateApiUrl("Resource");

        public async Task<List<Models.Resource>> GetResourcesAsync(int ModuleId)
        {
            List<Models.Resource> Resources = await GetJsonAsync<List<Models.Resource>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}", EntityNames.Module, ModuleId), Enumerable.Empty<Models.Resource>().ToList());
            return Resources.OrderBy(item => item.Name).ToList();
        }

        public async Task<Models.Resource> GetResourceAsync(int ResourceId, int ModuleId)
        {
            return await GetJsonAsync<Models.Resource>(CreateAuthorizationPolicyUrl($"{Apiurl}/{ResourceId}/{ModuleId}", EntityNames.Module, ModuleId));
        }

        public async Task<Models.Resource> AddResourceAsync(Models.Resource Resource)
        {
            return await PostJsonAsync<Models.Resource>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, Resource.ModuleId), Resource);
        }

        public async Task<Models.Resource> UpdateResourceAsync(Models.Resource Resource)
        {
            return await PutJsonAsync<Models.Resource>(CreateAuthorizationPolicyUrl($"{Apiurl}/{Resource.ResourceId}", EntityNames.Module, Resource.ModuleId), Resource);
        }

        public async Task DeleteResourceAsync(int ResourceId, int ModuleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{ResourceId}/{ModuleId}", EntityNames.Module, ModuleId));
        }
    }
}
