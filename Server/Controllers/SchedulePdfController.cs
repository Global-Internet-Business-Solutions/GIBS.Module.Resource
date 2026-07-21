using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using GIBS.Module.Resource.Services;
using GIBS.Module.Resource.Models;
using Oqtane.Controllers;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace GIBS.Module.Resource.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class SchedulePdfController : ModuleControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IResourceService _resourceService;
        private readonly ISchedulePdfService _schedulePdfService;

        public SchedulePdfController(
            IReservationService reservationService,
            IResourceService resourceService,
            ISchedulePdfService schedulePdfService,
            ILogManager logger,
            IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _reservationService = reservationService;
            _resourceService = resourceService;
            _schedulePdfService = schedulePdfService;
        }

        [HttpPost]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<IActionResult> Generate([FromBody] SchedulePdfRequest request)
        {
            if (!IsAuthorizedEntityId(EntityNames.Module, request.ModuleId))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Schedule PDF Generation Attempt {ModuleId}", request.ModuleId);
                return StatusCode((int)HttpStatusCode.Forbidden);
            }

            try
            {
                var reservations = new List<Reservation>();
                string resourceName;
                Dictionary<int, string> resourceNames = null;

                if (request.ViewType == "daily")
                {
                    if (request.ResourceId == 0)
                    {
                        // All resources
                        resourceName = "All Resources";
                        var resources = await _resourceService.GetResourcesAsync(request.ModuleId);
                        resourceNames = resources.Where(r => r.IsActive).ToDictionary(r => r.ResourceId, r => r.Name);

                        foreach (var resource in resources.Where(r => r.IsActive))
                        {
                            var resourceReservations = await _reservationService.GetReservationsAsync(resource.ResourceId, request.ModuleId);
                            if (resourceReservations != null)
                            {
                                var dayReservations = resourceReservations
                                    .Where(r => r.StartTime.Date == request.SelectedDate.Date && r.Status != ReservationStatus.Cancelled)
                                    .ToList();
                                reservations.AddRange(dayReservations);
                            }
                        }
                    }
                    else
                    {
                        // Single resource
                        var resource = await _resourceService.GetResourceAsync(request.ResourceId, request.ModuleId);
                        resourceName = resource?.Name ?? "Unknown Resource";

                        var resourceReservations = await _reservationService.GetReservationsAsync(request.ResourceId, request.ModuleId);
                        if (resourceReservations != null)
                        {
                            reservations = resourceReservations
                                .Where(r => r.StartTime.Date == request.SelectedDate.Date && r.Status != ReservationStatus.Cancelled)
                                .ToList();
                        }
                    }

                    reservations = reservations.OrderBy(r => r.StartTime).ToList();
                    var pdfBytes = _schedulePdfService.GenerateDailySchedulePdf(request.SelectedDate, resourceName, reservations, resourceNames);
                    return File(pdfBytes, "application/pdf", $"Schedule_{request.SelectedDate:yyyy-MM-dd}.pdf");
                }
                else // weekly
                {
                    var resource = await _resourceService.GetResourceAsync(request.ResourceId, request.ModuleId);
                    resourceName = resource?.Name ?? "Unknown Resource";

                    var weekEnd = request.WeekStartDate.AddDays(7);
                    var resourceReservations = await _reservationService.GetReservationsAsync(request.ResourceId, request.ModuleId);
                    if (resourceReservations != null)
                    {
                        reservations = resourceReservations
                            .Where(r => r.StartTime.Date >= request.WeekStartDate.Date &&
                                       r.StartTime.Date < weekEnd.Date &&
                                       r.Status != ReservationStatus.Cancelled)
                            .OrderBy(r => r.StartTime)
                            .ToList();
                    }

                    var pdfBytes = _schedulePdfService.GenerateWeeklySchedulePdf(request.WeekStartDate, resourceName, reservations);
                    return File(pdfBytes, "application/pdf", $"Schedule_Week_{request.WeekStartDate:yyyy-MM-dd}.pdf");
                }
            }
            catch (System.Exception ex)
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Read, ex, "Error Generating Schedule PDF {ModuleId}", request.ModuleId);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error generating PDF");
            }
        }
    }
}
