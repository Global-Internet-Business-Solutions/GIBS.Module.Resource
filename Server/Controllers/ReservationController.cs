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
    public class ReservationController : ModuleControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _reservationService = reservationService;
        }

        [HttpGet("resource/{resourceid}/{moduleid}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<IEnumerable<Models.Reservation>> GetByResource(int resourceid, int moduleid)
        {
            if (IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                return await _reservationService.GetReservationsAsync(resourceid, moduleid);
            }

            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Reservation Get Attempt {ResourceId} {ModuleId}", resourceid, moduleid);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpGet("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<Models.Reservation> Get(int id, int moduleid)
        {
            Models.Reservation reservation = await _reservationService.GetReservationAsync(id, moduleid);
            if (reservation != null && IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                return reservation;
            }

            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Reservation Get Attempt {ReservationId} {ModuleId}", id, moduleid);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpPost("{moduleid}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<Models.Reservation> Post(int moduleid, [FromBody] Models.Reservation reservation)
        {
            if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                return await _reservationService.AddReservationAsync(reservation, moduleid);
            }

            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Reservation Post Attempt {Reservation}", reservation);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpPut("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<Models.Reservation> Put(int id, int moduleid, [FromBody] Models.Reservation reservation)
        {
            if (ModelState.IsValid && reservation.ReservationId == id && IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                return await _reservationService.UpdateReservationAsync(reservation, moduleid);
            }

            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Reservation Put Attempt {Reservation}", reservation);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpDelete("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task Delete(int id, int moduleid)
        {
            if (IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                await _reservationService.DeleteReservationAsync(id, moduleid);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Reservation Delete Attempt {ReservationId} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
        }
    }
}
