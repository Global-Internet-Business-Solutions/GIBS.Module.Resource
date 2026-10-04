using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Oqtane.Repository;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace GIBS.Module.Resource.Services
{
    public interface ITwilioSmsService
    {
        Task<bool> SendReservationNotificationAsync(Models.Reservation reservation, Models.Resource resource, string userName, int moduleId);
    }

    public class TwilioSmsService : ITwilioSmsService
    {
        private readonly ISettingRepository _settingRepository;
        private readonly ILogger<TwilioSmsService> _logger;

        public TwilioSmsService(ISettingRepository settingRepository, ILogger<TwilioSmsService> logger)
        {
            _settingRepository = settingRepository;
            _logger = logger;
        }

        public async Task<bool> SendReservationNotificationAsync(Models.Reservation reservation, Models.Resource resource, string userName, int moduleId)
        {
            try
            {
                var settings = _settingRepository.GetSettings(Oqtane.Shared.EntityNames.Module, moduleId).ToDictionary(s => s.SettingName, s => s.SettingValue);

                if (!settings.TryGetValue("EnableTwilio", out var enableTwilioValue) || !bool.TryParse(enableTwilioValue, out var enableTwilio) || !enableTwilio)
                {
                    _logger.LogInformation("Twilio SMS is disabled for module {ModuleId}", moduleId);
                    return false;
                }

                if (!settings.TryGetValue("TwilioAccountSid", out var accountSid) || string.IsNullOrWhiteSpace(accountSid))
                {
                    _logger.LogWarning("TwilioAccountSid is not configured");
                    return false;
                }

                if (!settings.TryGetValue("TwilioAuthToken", out var authToken) || string.IsNullOrWhiteSpace(authToken))
                {
                    _logger.LogWarning("TwilioAuthToken is not configured");
                    return false;
                }

                if (!settings.TryGetValue("TwilioSendToNumber", out var toPhoneNumber) || string.IsNullOrWhiteSpace(toPhoneNumber))
                {
                    _logger.LogWarning("TwilioSendToNumber is not configured");
                    return false;
                }

                if (!settings.TryGetValue("TwilioPhoneNumber", out var fromPhoneNumber) || string.IsNullOrWhiteSpace(fromPhoneNumber))
                {
                    _logger.LogWarning("TwilioPhoneNumber is not configured");
                    return false;
                }

                accountSid = accountSid.Trim();
                authToken = authToken.Trim();
                fromPhoneNumber = fromPhoneNumber.Trim();
                toPhoneNumber = toPhoneNumber.Trim();

                var messageBody = BuildReservationMessage(reservation, resource, userName);
                _logger.LogInformation("Sending Twilio SMS for reservation {ReservationId} from {FromPhoneNumber} to {ToPhoneNumber}.", reservation.ReservationId, fromPhoneNumber, toPhoneNumber);

                TwilioClient.Init(accountSid, authToken);

                var message = await MessageResource.CreateAsync(
                    to: new PhoneNumber(toPhoneNumber),
                    from: new PhoneNumber(fromPhoneNumber),
                    body: messageBody);

                var sid = message.Sid;
                var status = message.Status?.ToString();
                var errorCode = message.ErrorCode?.ToString();
                var errorMessage = message.ErrorMessage;

                if (!string.IsNullOrWhiteSpace(errorCode))
                {
                    _logger.LogWarning("Twilio API returned error for reservation {ReservationId}. SID: {MessageSid}, Status: {Status}, ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}", reservation.ReservationId, sid, status, errorCode, errorMessage);
                    return false;
                }

                _logger.LogInformation("Twilio SMS sent successfully. SID: {MessageSid}, Status: {Status}", sid, status);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Twilio SMS for reservation {ReservationId}", reservation.ReservationId);
                return false;
            }
        }

        private string BuildReservationMessage(Models.Reservation reservation, Models.Resource resource, string userName)
        {
            var startTime = AsLocalTime(reservation.StartTime).ToString("g");
            var endTime = AsLocalTime(reservation.EndTime).ToString("t");

            return $"New Reservation Booked!\n" +
                   $"Resource: {resource.Name}\n" +
                   $"User: {userName}\n" +
                   $"When: {startTime} - {endTime}\n" +
                   $"Status: {reservation.Status}";
        }

        private static DateTime AsLocalTime(DateTime value)
        {
            var utcValue = value.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
                : value.ToUniversalTime();

            return utcValue.ToLocalTime();
        }
    }
}
