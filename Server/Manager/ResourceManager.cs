using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Oqtane.Modules;
using Oqtane.Models;
using Oqtane.Infrastructure;
using Oqtane.Interfaces;
using Oqtane.Enums;
using Oqtane.Repository;
using GIBS.Module.Resource.Repository;
using System.Threading.Tasks;

namespace GIBS.Module.Resource.Manager
{
    public class ResourceManager : MigratableModuleBase, IInstallable, IPortable, ISearchable
    {
        private readonly IResourceRepository _ResourceRepository;
        private readonly IDBContextDependencies _DBContextDependencies;

        public ResourceManager(IResourceRepository ResourceRepository, IDBContextDependencies DBContextDependencies)
        {
            _ResourceRepository = ResourceRepository;
            _DBContextDependencies = DBContextDependencies;
        }

        public bool Install(Tenant tenant, string version)
        {
            return Migrate(new ResourceContext(_DBContextDependencies), tenant, MigrationType.Up);
        }

        public bool Uninstall(Tenant tenant)
        {
            return Migrate(new ResourceContext(_DBContextDependencies), tenant, MigrationType.Down);
        }

        public string ExportModule(Oqtane.Models.Module module)
        {
            string content = "";
            List<Models.Resource> Resources = _ResourceRepository.GetResources(module.ModuleId).ToList();
            if (Resources != null)
            {
                content = JsonSerializer.Serialize(Resources);
            }
            return content;
        }

        public void ImportModule(Oqtane.Models.Module module, string content, string version)
        {
            List<Models.Resource> Resources = null;
            if (!string.IsNullOrEmpty(content))
            {
                Resources = JsonSerializer.Deserialize<List<Models.Resource>>(content);
            }
            if (Resources != null)
            {
                foreach(var Resource in Resources)
                {
                    _ResourceRepository.AddResource(new Models.Resource { ModuleId = module.ModuleId, Name = Resource.Name });
                }
            }
        }

        public Task<List<SearchContent>> GetSearchContentsAsync(PageModule pageModule, DateTime lastIndexedOn)
        {
           var searchContentList = new List<SearchContent>();

           foreach (var Resource in _ResourceRepository.GetResources(pageModule.ModuleId))
           {
               if (Resource.ModifiedOn >= lastIndexedOn)
               {
                   searchContentList.Add(new SearchContent
                   {
                       EntityName = "GIBSResource",
                       EntityId = Resource.ResourceId.ToString(),
                       Title = Resource.Name,
                       Body = Resource.Name,
                       ContentModifiedBy = Resource.ModifiedBy,
                       ContentModifiedOn = Resource.ModifiedOn
                   });
               }
           }

           return Task.FromResult(searchContentList);
        }
    }
}
