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
    public class ResourceController : ModuleControllerBase
    {
        private readonly IResourceService _ResourceService;

        public ResourceController(IResourceService ResourceService, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _ResourceService = ResourceService;
        }

        // GET: api/<controller>?moduleid=x
        [HttpGet]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<IEnumerable<Models.Resource>> Get(string moduleid)
        {
            int ModuleId;
            if (int.TryParse(moduleid, out ModuleId) && IsAuthorizedEntityId(EntityNames.Module, ModuleId))
            {
                return await _ResourceService.GetResourcesAsync(ModuleId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Resource Get Attempt {ModuleId}", moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }
        }

        // GET api/<controller>/5
        [HttpGet("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<Models.Resource> Get(int id, int moduleid)
        {
            Models.Resource Resource = await _ResourceService.GetResourceAsync(id, moduleid);
            if (Resource != null && IsAuthorizedEntityId(EntityNames.Module, Resource.ModuleId))
            {
                return Resource;
            }
            else
            { 
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Resource Get Attempt {ResourceId} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<Models.Resource> Post([FromBody] Models.Resource Resource)
        {
            if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, Resource.ModuleId))
            {
                Resource = await _ResourceService.AddResourceAsync(Resource);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Resource Post Attempt {Resource}", Resource);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Resource = null;
            }
            return Resource;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<Models.Resource> Put(int id, [FromBody] Models.Resource Resource)
        {
            if (ModelState.IsValid && Resource.ResourceId == id && IsAuthorizedEntityId(EntityNames.Module, Resource.ModuleId))
            {
                Resource = await _ResourceService.UpdateResourceAsync(Resource);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Resource Put Attempt {Resource}", Resource);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Resource = null;
            }
            return Resource;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task Delete(int id, int moduleid)
        {
            Models.Resource Resource = await _ResourceService.GetResourceAsync(id, moduleid);
            if (Resource != null && IsAuthorizedEntityId(EntityNames.Module, Resource.ModuleId))
            {
                await _ResourceService.DeleteResourceAsync(id, Resource.ModuleId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Resource Delete Attempt {ResourceId} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
        }
    }
}
