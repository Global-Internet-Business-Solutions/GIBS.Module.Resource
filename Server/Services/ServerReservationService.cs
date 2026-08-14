using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Security;
using Oqtane.Shared;
using Oqtane.Repository;
using GIBS.Module.Resource.Repository;

namespace GIBS.Module.Resource.Services
{
    public class ServerReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IResourceRepository _resourceRepository;
        private readonly IResourceAvailabilityRepository _resourceAvailabilityRepository;
        private readonly IUserPermissions _userPermissions;
        private readonly ILogManager _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly Alias _alias;
        private readonly IUserRepository _userRepository;
        private readonly ITwilioSmsService _twilioSmsService;
        private readonly INotificationRepository _notificationRepository;

        public ServerReservationService(IReservationRepository reservationRepository, IResourceRepository resourceRepository, IResourceAvailabilityRepository resourceAvailabilityRepository, IUserPermissions userPermissions, ITenantManager tenantManager, ILogManager logger, IHttpContextAccessor accessor, IUserRepository userRepository, ITwilioSmsService twilioSmsService, INotificationRepository notificationRepository)
        {
            _reservationRepository = reservationRepository;
            _resourceRepository = resourceRepository;
            _resourceAvailabilityRepository = resourceAvailabilityRepository;
            _userPermissions = userPermissions;
            _logger = logger;
            _accessor = accessor;
            _alias = tenantManager.GetAlias();
            _userRepository = userRepository;
            _twilioSmsService = twilioSmsService;
            _notificationRepository = notificationRepository;
        }

        public Task<List<Models.Reservation>> GetReservationsAsync(int resourceId, int moduleId)
        {
            if (!IsAuthorized(moduleId, PermissionNames.View))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Reservation Get Attempt {ResourceId} {ModuleId}", resourceId, moduleId);
                return null;
            }

            var resource = _resourceRepository.GetResourceForModule(resourceId, moduleId);
            if (resource == null)
            {
                return Task.FromResult(new List<Models.Reservation>());
            }

            var reservations = _reservationRepository.GetReservations(resourceId).ToList();

            // Populate UserName for each reservation
            foreach (var reservation in reservations)
            {
                if (reservation.UserId > 0)
                {
                    var user = _userRepository.GetUser(reservation.UserId);
                    if (user != null)
                    {
                        reservation.UserName = user.DisplayName ?? user.Username;
                    }
                }
            }

            return Task.FromResult(reservations);
        }

        public Task<List<Models.ReservationUserOption>> GetReservationUsersAsync(int moduleId)
        {
            if (!IsAuthorized(moduleId, PermissionNames.Edit))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Reservation Users Get Attempt {ModuleId}", moduleId);
                return null;
            }

            var allUsers = _userRepository.GetUsers().ToList();
         //   _logger.Log(LogLevel.Information, this, LogFunction.Read, "Total users in database: {Count}", allUsers.Count);

            var users = allUsers
                .Where(user => 
                {
                    var isHost = user.Username == Oqtane.Shared.UserNames.Host;
                    var isDeleted = user.IsDeleted;
                    var isValid = !isDeleted && !isHost;

                    if (!isValid)
                    {
                        //_logger.Log(LogLevel.Information, this, LogFunction.Read, 
                        //    "Filtering out user '{Username}' (UserId={UserId}): IsDeleted={IsDeleted}, IsHost={IsHost}", 
                        //    user.Username, user.UserId, isDeleted, isHost);
                    }
                    else
                    {
                        //_logger.Log(LogLevel.Information, this, LogFunction.Read, 
                        //    "Including user '{Username}' (UserId={UserId})", 
                        //    user.Username, user.UserId);
                    }

                    return isValid;
                })
                .OrderBy(user => string.IsNullOrWhiteSpace(user.DisplayName) ? user.Username : user.DisplayName)
                .Select(user => new Models.ReservationUserOption
                {
                    UserId = user.UserId,
                    DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? user.Username : user.DisplayName
                })
                .ToList();

         //   _logger.Log(LogLevel.Information, this, LogFunction.Read, "Returning {Count} users", users.Count);

            return Task.FromResult(users);
        }

        public Task<Models.Reservation> GetReservationAsync(int reservationId, int moduleId)
        {
            if (!IsAuthorized(moduleId, PermissionNames.View))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Reservation Get Attempt {ReservationId} {ModuleId}", reservationId, moduleId);
                return null;
            }

            var reservation = _reservationRepository.GetReservation(reservationId);
            if (reservation == null)
            {
                return Task.FromResult<Models.Reservation>(null);
            }

            var resource = _resourceRepository.GetResourceForModule(reservation.ResourceId, moduleId);
            if (resource == null)
            {
                return Task.FromResult<Models.Reservation>(null);
            }

            // Populate UserName if UserId is valid
            if (reservation.UserId > 0)
            {
                var user = _userRepository.GetUser(reservation.UserId);
                if (user != null)
                {
                    reservation.UserName = user.DisplayName ?? user.Username;
                }
            }

            return Task.FromResult(reservation);
        }

        public async Task<Models.Reservation> AddReservationAsync(Models.Reservation reservation, int moduleId)
        {
            if (!IsAuthorized(moduleId, PermissionNames.View))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Reservation Add Attempt {Reservation}", reservation);
                return null;
            }

            var resource = _resourceRepository.GetResourceForModule(reservation.ResourceId, moduleId, true);
            if (!IsReservationValid(reservation, resource, 0))
            {
                return null;
            }

            reservation = _reservationRepository.AddReservation(reservation);
            _logger.Log(LogLevel.Information, this, LogFunction.Create, "Reservation Added {Reservation}", reservation);

            // Send notifications before returning so failures are captured reliably.
            if (reservation != null)
            {
                try
                {
                    var user = _userRepository.GetUser(reservation.UserId);
                    var userName = user?.DisplayName ?? user?.Username ?? "Unknown";

                    var smsSent = await _twilioSmsService.SendReservationNotificationAsync(reservation, resource, userName, moduleId);
                    if (!smsSent)
                    {
                        _logger.Log(LogLevel.Information, this, LogFunction.Other, "Twilio SMS was not sent for reservation {ReservationId}.", reservation.ReservationId);
                    }

                    await SendReservationEmailAsync(reservation, resource, isNewReservation: true);
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, this, LogFunction.Other, ex, "Error sending notifications for reservation {ReservationId}", reservation.ReservationId);
                }
            }

            return reservation;
        }

        public async Task<Models.Reservation> UpdateReservationAsync(Models.Reservation reservation, int moduleId)
        {
            if (!IsAuthorized(moduleId, PermissionNames.Edit))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Reservation Update Attempt {Reservation}", reservation);
                return null;
            }

            var existing = _reservationRepository.GetReservation(reservation.ReservationId);
            if (existing == null)
            {
                return null;
            }

            var resource = _resourceRepository.GetResourceForModule(reservation.ResourceId, moduleId, true, true);
            if (!IsReservationValid(reservation, resource, reservation.ReservationId))
            {
                return null;
            }

            // Check if status is changing from Pending to Confirmed
            bool statusChangedToConfirmed = existing.Status == Models.ReservationStatus.Pending && 
                                           reservation.Status == Models.ReservationStatus.Confirmed;

            reservation = _reservationRepository.UpdateReservation(reservation);
            _logger.Log(LogLevel.Information, this, LogFunction.Update, "Reservation Updated {Reservation}", reservation);

            // Send email notification if status changed to Confirmed
            if (statusChangedToConfirmed && reservation != null)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await SendReservationEmailAsync(reservation, resource, isNewReservation: false);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogLevel.Error, this, LogFunction.Other, ex, "Error sending confirmation email for reservation {ReservationId}", reservation.ReservationId);
                    }
                });
            }

            return reservation;
        }

        public Task DeleteReservationAsync(int reservationId, int moduleId)
        {
            if (!IsAuthorized(moduleId, PermissionNames.Edit))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Reservation Delete Attempt {ReservationId} {ModuleId}", reservationId, moduleId);
                return Task.CompletedTask;
            }

            var reservation = _reservationRepository.GetReservation(reservationId);
            if (reservation == null)
            {
                return Task.CompletedTask;
            }

            var resource = _resourceRepository.GetResourceForModule(reservation.ResourceId, moduleId);
            if (resource == null)
            {
                return Task.CompletedTask;
            }

            _reservationRepository.DeleteReservation(reservationId);
            _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Reservation Deleted {ReservationId}", reservationId);
            return Task.CompletedTask;
        }

        private bool IsAuthorized(int moduleId, string permissionName)
        {
            return _userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, permissionName);
        }

        private bool IsReservationValid(Models.Reservation reservation, Models.Resource resource, int excludeReservationId)
        {
            if (reservation == null || resource == null)
            {
                return false;
            }

            if (reservation.StartTime >= reservation.EndTime)
            {
                _logger.Log(LogLevel.Warning, this, LogFunction.Create, "Reservation validation failed due to invalid time range {Reservation}", reservation);
                return false;
            }

            if (!_resourceAvailabilityRepository.IsWithinAvailability(resource.ResourceId, reservation.StartTime, reservation.EndTime))
            {
                _logger.Log(LogLevel.Warning, this, LogFunction.Create, "Reservation validation failed due to availability policy {Reservation}", reservation);
                return false;
            }

            if (_reservationRepository.HasOverlappingReservation(resource.ResourceId, reservation.StartTime, reservation.EndTime, excludeReservationId, resource.BufferBeforeMinutes, resource.BufferAfterMinutes))
            {
                _logger.Log(LogLevel.Warning, this, LogFunction.Create, "Reservation validation failed due to overlap {Reservation}", reservation);
                return false;
            }

            return true;
        }

        private async Task SendReservationEmailAsync(Models.Reservation reservation, Models.Resource resource, bool isNewReservation)
        {
            var user = _userRepository.GetUser(reservation.UserId);
            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                _logger.Log(LogLevel.Warning, this, LogFunction.Other, "Cannot send reservation email - user not found or has no email for reservation {ReservationId}", reservation.ReservationId);
                return;
            }

            var startTime = reservation.StartTime.ToString("g");
            var endTime = reservation.EndTime.ToString("t");

            string subject;
            string heading;
            string message;

            if (isNewReservation)
            {
                // New reservation email
                subject = $"Reservation {(reservation.Status == Models.ReservationStatus.Confirmed ? "Confirmed" : "Received")} - {resource.Name}";
                heading = reservation.Status == Models.ReservationStatus.Confirmed 
                    ? "Your Reservation Has Been Confirmed!" 
                    : "Your Reservation Has Been Received!";
                message = reservation.Status == Models.ReservationStatus.Confirmed
                    ? "Your reservation has been confirmed with the following details:"
                    : "Your reservation has been received and is pending confirmation. Details:";
            }
            else
            {
                // Status change to confirmed email
                subject = $"Reservation Confirmed - {resource.Name}";
                heading = "Your Reservation Has Been Confirmed!";
                message = "Your reservation status has been updated to confirmed. Details:";
            }

            var body = $@"<html>
<body>
<h2>{heading}</h2>
<p>Hello {user.DisplayName ?? user.Username},</p>
<p>{message}</p>
<ul>
    <li><strong>Resource:</strong> {resource.Name}</li>
    <li><strong>When:</strong> {startTime} - {endTime}</li>
    <li><strong>Status:</strong> {reservation.Status}</li>
</ul>
{(!string.IsNullOrWhiteSpace(reservation.Notes) ? $"<p><strong>Notes:</strong> {reservation.Notes}</p>" : "")}
<p>Thank you for using our reservation system!</p>
</body>
</html>";

            var notification = new Notification
            {
                SiteId = _alias.SiteId,
                FromUserId = null,
                FromDisplayName = "Reservation System",
                FromEmail = _alias.Name, // Site email
                ToUserId = reservation.UserId,
                ToDisplayName = user.DisplayName ?? user.Username,
                ToEmail = user.Email,
                Subject = subject,
                Body = body,
                CreatedOn = DateTime.UtcNow,
                IsDelivered = false,
                IsRead = false,
                SendOn = DateTime.UtcNow
            };

            _notificationRepository.AddNotification(notification);
            _logger.Log(LogLevel.Information, this, LogFunction.Other, "Reservation email queued for reservation {ReservationId} to user {UserId}", reservation.ReservationId, reservation.UserId);
        }
    }
}
