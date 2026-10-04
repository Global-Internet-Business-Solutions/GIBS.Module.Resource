using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;

namespace GIBS.Module.Resource.Repository
{
    public interface IReservationRepository
    {
        IEnumerable<Models.Reservation> GetReservations(int resourceId);
        Models.Reservation GetReservation(int reservationId);
        Models.Reservation AddReservation(Models.Reservation reservation);
        Models.Reservation UpdateReservation(Models.Reservation reservation);
        void DeleteReservation(int reservationId);
        bool HasOverlappingReservation(int resourceId, DateTime startTime, DateTime endTime, int excludeReservationId, int bufferBeforeMinutes, int bufferAfterMinutes);
    }

    public class ReservationRepository : IReservationRepository, ITransientService
    {
        private readonly IDbContextFactory<ResourceContext> _factory;

        public ReservationRepository(IDbContextFactory<ResourceContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<Models.Reservation> GetReservations(int resourceId)
        {
            using var db = _factory.CreateDbContext();
            return db.Reservation
                .Where(item => item.ResourceId == resourceId)
                .OrderBy(item => item.StartTime)
                .ToList();
        }

        public Models.Reservation GetReservation(int reservationId)
        {
            using var db = _factory.CreateDbContext();
            return db.Reservation.FirstOrDefault(item => item.ReservationId == reservationId);
        }

        public Models.Reservation AddReservation(Models.Reservation reservation)
        {
            using var db = _factory.CreateDbContext();
            NormalizeReservationDateTimes(reservation);
            db.Reservation.Add(reservation);
            db.SaveChanges();
            return reservation;
        }

        public Models.Reservation UpdateReservation(Models.Reservation reservation)
        {
            using var db = _factory.CreateDbContext();
            var current = db.Reservation.Find(reservation.ReservationId);
            if (current == null)
            {
                return null;
            }

            current.ResourceId = reservation.ResourceId;
            current.UserId = reservation.UserId;
            current.StartTime = AsUtcKind(reservation.StartTime);
            current.EndTime = AsUtcKind(reservation.EndTime);
            current.Status = reservation.Status;
            current.Notes = reservation.Notes;
            current.OptIn = reservation.OptIn;

            db.SaveChanges();
            return current;
        }

        public void DeleteReservation(int reservationId)
        {
            using var db = _factory.CreateDbContext();
            var reservation = db.Reservation.Find(reservationId);
            if (reservation != null)
            {
                db.Reservation.Remove(reservation);
                db.SaveChanges();
            }
        }

        public bool HasOverlappingReservation(int resourceId, DateTime startTime, DateTime endTime, int excludeReservationId, int bufferBeforeMinutes, int bufferAfterMinutes)
        {
            using var db = _factory.CreateDbContext();
            return db.Reservation.Any(item =>
                item.ResourceId == resourceId &&
                item.ReservationId != excludeReservationId &&
                item.Status != Models.ReservationStatus.Cancelled &&
                item.StartTime.AddMinutes(-bufferBeforeMinutes) < endTime &&
                item.EndTime.AddMinutes(bufferAfterMinutes) > startTime);
        }

        private static void NormalizeReservationDateTimes(Models.Reservation reservation)
        {
            reservation.StartTime = AsUtcKind(reservation.StartTime);
            reservation.EndTime = AsUtcKind(reservation.EndTime);

            if (reservation.CreatedOn != default)
            {
                reservation.CreatedOn = AsUtcKind(reservation.CreatedOn);
            }

            if (reservation.ModifiedOn != default)
            {
                reservation.ModifiedOn = AsUtcKind(reservation.ModifiedOn);
            }
        }

        private static DateTime AsUtcKind(DateTime value)
        {
            return value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };
        }
    }
}
