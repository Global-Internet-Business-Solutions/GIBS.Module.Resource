using System.Collections.Generic;
using System.Threading.Tasks;

namespace GIBS.Module.Resource.Services
{
    public interface IResourceService 
    {
        Task<List<Models.Resource>> GetResourcesAsync(int ModuleId);

        Task<Models.Resource> GetResourceAsync(int ResourceId, int ModuleId);

        Task<Models.Resource> AddResourceAsync(Models.Resource Resource);

        Task<Models.Resource> UpdateResourceAsync(Models.Resource Resource);

        Task DeleteResourceAsync(int ResourceId, int ModuleId);
    }
}
