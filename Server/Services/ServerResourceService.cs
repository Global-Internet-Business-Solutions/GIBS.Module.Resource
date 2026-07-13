using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Security;
using Oqtane.Shared;
using GIBS.Module.Resource.Repository;

namespace GIBS.Module.Resource.Services
{
    public class ServerResourceService : IResourceService
    {
        private readonly IResourceRepository _ResourceRepository;
        private readonly IUserPermissions _userPermissions;
        private readonly ILogManager _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly Alias _alias;

        public ServerResourceService(IResourceRepository ResourceRepository, IUserPermissions userPermissions, ITenantManager tenantManager, ILogManager logger, IHttpContextAccessor accessor)
        {
            _ResourceRepository = ResourceRepository;
            _userPermissions = userPermissions;
            _logger = logger;
            _accessor = accessor;
            _alias = tenantManager.GetAlias();
        }

        public Task<List<Models.Resource>> GetResourcesAsync(int ModuleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.View))
            {
                return Task.FromResult(_ResourceRepository.GetResources(ModuleId).ToList());
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Resource Get Attempt {ModuleId}", ModuleId);
                return null;
            }
        }

        public Task<Models.Resource> GetResourceAsync(int ResourceId, int ModuleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.View))
            {
                return Task.FromResult(_ResourceRepository.GetResource(ResourceId));
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Resource Get Attempt {ResourceId} {ModuleId}", ResourceId, ModuleId);
                return null;
            }
        }

        public Task<Models.Resource> AddResourceAsync(Models.Resource Resource)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, Resource.ModuleId, PermissionNames.Edit))
            {
                Resource = _ResourceRepository.AddResource(Resource);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "Resource Added {Resource}", Resource);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Resource Add Attempt {Resource}", Resource);
                Resource = null;
            }
            return Task.FromResult(Resource);
        }

        public Task<Models.Resource> UpdateResourceAsync(Models.Resource Resource)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, Resource.ModuleId, PermissionNames.Edit))
            {
                Resource = _ResourceRepository.UpdateResource(Resource);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "Resource Updated {Resource}", Resource);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Resource Update Attempt {Resource}", Resource);
                Resource = null;
            }
            return Task.FromResult(Resource);
        }

        public Task DeleteResourceAsync(int ResourceId, int ModuleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.Edit))
            {
                _ResourceRepository.DeleteResource(ResourceId);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Resource Deleted {ResourceId}", ResourceId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Resource Delete Attempt {ResourceId} {ModuleId}", ResourceId, ModuleId);
            }
            return Task.CompletedTask;
        }
    }
}
