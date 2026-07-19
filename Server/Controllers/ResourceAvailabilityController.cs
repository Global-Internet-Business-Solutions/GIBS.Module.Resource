using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using GIBS.Module.Resource.Services;
using Oqtane.Controllers;
using System.Net;
using System.Threading.Tasks;

namespace GIBS.Module.Resource.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class ResourceAvailabilityController : ModuleControllerBase
    {
        private readonly IResourceAvailabilityService _resourceAvailabilityService;

        public ResourceAvailabilityController(IResourceAvailabilityService resourceAvailabilityService, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _resourceAvailabilityService = resourceAvailabilityService;
        }

        [HttpGet("resource/{resourceid}/{moduleid}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<IEnumerable<Models.ResourceAvailability>> GetByResource(int resourceid, int moduleid)
        {
            if (IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                return await _resourceAvailabilityService.GetAvailabilitiesAsync(resourceid, moduleid);
            }

            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Availability Get Attempt {ResourceId} {ModuleId}", resourceid, moduleid);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpGet("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<Models.ResourceAvailability> Get(int id, int moduleid)
        {
            Models.ResourceAvailability availability = await _resourceAvailabilityService.GetAvailabilityAsync(id, moduleid);
            if (availability != null && IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                return availability;
            }

            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Availability Get Attempt {AvailabilityId} {ModuleId}", id, moduleid);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpPost("{moduleid}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<Models.ResourceAvailability> Post(int moduleid, [FromBody] Models.ResourceAvailability availability)
        {
            if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                return await _resourceAvailabilityService.AddAvailabilityAsync(availability, moduleid);
            }

            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Availability Post Attempt {Availability}", availability);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpPut("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<Models.ResourceAvailability> Put(int id, int moduleid, [FromBody] Models.ResourceAvailability availability)
        {
            if (ModelState.IsValid && availability.AvailabilityId == id && IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                return await _resourceAvailabilityService.UpdateAvailabilityAsync(availability, moduleid);
            }

            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Availability Put Attempt {Availability}", availability);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpDelete("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task Delete(int id, int moduleid)
        {
            if (IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                await _resourceAvailabilityService.DeleteAvailabilityAsync(id, moduleid);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Availability Delete Attempt {AvailabilityId} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
        }
    }
}
