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
    public class ServerResourceAvailabilityService : IResourceAvailabilityService
    {
        private readonly IResourceAvailabilityRepository _resourceAvailabilityRepository;
        private readonly IResourceRepository _resourceRepository;
        private readonly IUserPermissions _userPermissions;
        private readonly ILogManager _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly Alias _alias;

        public ServerResourceAvailabilityService(IResourceAvailabilityRepository resourceAvailabilityRepository, IResourceRepository resourceRepository, IUserPermissions userPermissions, ITenantManager tenantManager, ILogManager logger, IHttpContextAccessor accessor)
        {
            _resourceAvailabilityRepository = resourceAvailabilityRepository;
            _resourceRepository = resourceRepository;
            _userPermissions = userPermissions;
            _logger = logger;
            _accessor = accessor;
            _alias = tenantManager.GetAlias();
        }

        public Task<List<Models.ResourceAvailability>> GetAvailabilitiesAsync(int resourceId, int moduleId)
        {
            if (!IsAuthorized(moduleId, PermissionNames.View))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Availability Get Attempt {ResourceId} {ModuleId}", resourceId, moduleId);
                return null;
            }

            var resource = _resourceRepository.GetResourceForModule(resourceId, moduleId);
            return Task.FromResult(resource == null ? new List<Models.ResourceAvailability>() : _resourceAvailabilityRepository.GetAvailabilities(resourceId).ToList());
        }

        public Task<Models.ResourceAvailability> GetAvailabilityAsync(int availabilityId, int moduleId)
        {
            if (!IsAuthorized(moduleId, PermissionNames.View))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Availability Get Attempt {AvailabilityId} {ModuleId}", availabilityId, moduleId);
                return null;
            }

            var availability = _resourceAvailabilityRepository.GetAvailability(availabilityId);
            if (availability == null)
            {
                return Task.FromResult<Models.ResourceAvailability>(null);
            }

            var resource = _resourceRepository.GetResourceForModule(availability.ResourceId, moduleId);
            return Task.FromResult(resource == null ? null : availability);
        }

        public Task<Models.ResourceAvailability> AddAvailabilityAsync(Models.ResourceAvailability availability, int moduleId)
        {
            if (!IsAuthorized(moduleId, PermissionNames.Edit))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Availability Add Attempt {Availability}", availability);
                return Task.FromResult<Models.ResourceAvailability>(null);
            }

            var resource = _resourceRepository.GetResourceForModule(availability.ResourceId, moduleId, true);
            if (resource == null)
            {
                return Task.FromResult<Models.ResourceAvailability>(null);
            }

            availability = _resourceAvailabilityRepository.AddAvailability(availability);
            _logger.Log(LogLevel.Information, this, LogFunction.Create, "Availability Added {Availability}", availability);
            return Task.FromResult(availability);
        }

        public Task<Models.ResourceAvailability> UpdateAvailabilityAsync(Models.ResourceAvailability availability, int moduleId)
        {
            if (!IsAuthorized(moduleId, PermissionNames.Edit))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Availability Update Attempt {Availability}", availability);
                return Task.FromResult<Models.ResourceAvailability>(null);
            }

            var existing = _resourceAvailabilityRepository.GetAvailability(availability.AvailabilityId);
            if (existing == null)
            {
                return Task.FromResult<Models.ResourceAvailability>(null);
            }

            var resource = _resourceRepository.GetResourceForModule(availability.ResourceId, moduleId, true);
            if (resource == null)
            {
                return Task.FromResult<Models.ResourceAvailability>(null);
            }

            availability = _resourceAvailabilityRepository.UpdateAvailability(availability);
            _logger.Log(LogLevel.Information, this, LogFunction.Update, "Availability Updated {Availability}", availability);
            return Task.FromResult(availability);
        }

        public Task DeleteAvailabilityAsync(int availabilityId, int moduleId)
        {
            if (!IsAuthorized(moduleId, PermissionNames.Edit))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Availability Delete Attempt {AvailabilityId} {ModuleId}", availabilityId, moduleId);
                return Task.CompletedTask;
            }

            var availability = _resourceAvailabilityRepository.GetAvailability(availabilityId);
            if (availability == null)
            {
                return Task.CompletedTask;
            }

            var resource = _resourceRepository.GetResourceForModule(availability.ResourceId, moduleId);
            if (resource == null)
            {
                return Task.CompletedTask;
            }

            _resourceAvailabilityRepository.DeleteAvailability(availabilityId);
            _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Availability Deleted {AvailabilityId}", availabilityId);
            return Task.CompletedTask;
        }

        private bool IsAuthorized(int moduleId, string permissionName)
        {
            return _userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, permissionName);
        }
    }
}
