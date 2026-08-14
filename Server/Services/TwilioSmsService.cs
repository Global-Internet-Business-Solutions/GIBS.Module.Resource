using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Oqtane.Repository;
using Twilio;
using Twilio.Exceptions;
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

                TwilioClient.Init(accountSid, authToken);

                var messageBody = BuildReservationMessage(reservation, resource, userName);
                _logger.LogInformation("Sending Twilio SMS for reservation {ReservationId} from {FromPhoneNumber} to {ToPhoneNumber}.", reservation.ReservationId, fromPhoneNumber, toPhoneNumber);

                var message = await MessageResource.CreateAsync(
                    body: messageBody,
                    from: new PhoneNumber(fromPhoneNumber),
                    to: new PhoneNumber(toPhoneNumber)
                );

                if (message.ErrorCode.HasValue)
                {
                    _logger.LogWarning("Twilio API returned error for reservation {ReservationId}. SID: {MessageSid}, Status: {Status}, ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}", reservation.ReservationId, message.Sid, message.Status, message.ErrorCode, message.ErrorMessage);
                    return false;
                }

                _logger.LogInformation("Twilio SMS sent successfully. SID: {MessageSid}, Status: {Status}", message.Sid, message.Status);
                return true;
            }
            catch (RestException ex)
            {
                _logger.LogError(ex, "Twilio REST error sending SMS for reservation {ReservationId}. Status: {Status}, Code: {Code}, MoreInfo: {MoreInfo}", reservation.ReservationId, ex.Status, ex.Code, ex.MoreInfo);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Twilio SMS for reservation {ReservationId}", reservation.ReservationId);
                return false;
            }
        }

        private string BuildReservationMessage(Models.Reservation reservation, Models.Resource resource, string userName)
        {
            var startTime = reservation.StartTime.ToString("g");
            var endTime = reservation.EndTime.ToString("t");

            return $"New Reservation Booked!\n" +
                   $"Resource: {resource.Name}\n" +
                   $"User: {userName}\n" +
                   $"When: {startTime} - {endTime}\n" +
                   $"Status: {reservation.Status}";
        }
    }
}
