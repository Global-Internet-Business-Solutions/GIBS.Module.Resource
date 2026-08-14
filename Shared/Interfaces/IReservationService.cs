using System.Collections.Generic;
using System.Threading.Tasks;

namespace GIBS.Module.Resource.Services
{
    public interface IReservationService
    {
        Task<List<Models.Reservation>> GetReservationsAsync(int resourceId, int moduleId);

        Task<List<Models.ReservationUserOption>> GetReservationUsersAsync(int moduleId);

        Task<Models.Reservation> GetReservationAsync(int reservationId, int moduleId);

        Task<Models.Reservation> AddReservationAsync(Models.Reservation reservation, int moduleId);

        Task<Models.Reservation> UpdateReservationAsync(Models.Reservation reservation, int moduleId);

        Task DeleteReservationAsync(int reservationId, int moduleId);
    }
}
