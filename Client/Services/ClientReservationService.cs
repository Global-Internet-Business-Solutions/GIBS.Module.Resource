using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Services;
using Oqtane.Shared;

namespace GIBS.Module.Resource.Services
{
    public class ClientReservationService : ServiceBase, IReservationService
    {
        public ClientReservationService(HttpClient http, SiteState siteState) : base(http, siteState)
        {
        }

        private string Apiurl => CreateApiUrl("Reservation");

        public async Task<List<Models.Reservation>> GetReservationsAsync(int resourceId, int moduleId)
        {
            var reservations = await GetJsonAsync<List<Models.Reservation>>(
                CreateAuthorizationPolicyUrl($"{Apiurl}/resource/{resourceId}/{moduleId}", EntityNames.Module, moduleId),
                Enumerable.Empty<Models.Reservation>().ToList());

            return reservations.OrderBy(item => item.StartTime).ToList();
        }

        public async Task<Models.Reservation> GetReservationAsync(int reservationId, int moduleId)
        {
            return await GetJsonAsync<Models.Reservation>(
                CreateAuthorizationPolicyUrl($"{Apiurl}/{reservationId}/{moduleId}", EntityNames.Module, moduleId));
        }

        public async Task<Models.Reservation> AddReservationAsync(Models.Reservation reservation, int moduleId)
        {
            return await PostJsonAsync<Models.Reservation>(
                CreateAuthorizationPolicyUrl($"{Apiurl}/{moduleId}", EntityNames.Module, moduleId),
                reservation);
        }

        public async Task<Models.Reservation> UpdateReservationAsync(Models.Reservation reservation, int moduleId)
        {
            return await PutJsonAsync<Models.Reservation>(
                CreateAuthorizationPolicyUrl($"{Apiurl}/{reservation.ReservationId}/{moduleId}", EntityNames.Module, moduleId),
                reservation);
        }

        public async Task DeleteReservationAsync(int reservationId, int moduleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{reservationId}/{moduleId}", EntityNames.Module, moduleId));
        }
    }
}
