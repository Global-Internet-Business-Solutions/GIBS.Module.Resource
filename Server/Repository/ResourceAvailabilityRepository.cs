using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;

namespace GIBS.Module.Resource.Repository
{
    public interface IResourceAvailabilityRepository
    {
        IEnumerable<Models.ResourceAvailability> GetAvailabilities(int resourceId);
        Models.ResourceAvailability GetAvailability(int availabilityId);
        Models.ResourceAvailability AddAvailability(Models.ResourceAvailability availability);
        Models.ResourceAvailability UpdateAvailability(Models.ResourceAvailability availability);
        void DeleteAvailability(int availabilityId);
        bool IsWithinAvailability(int resourceId, DateTime startTime, DateTime endTime);
    }

    public class ResourceAvailabilityRepository : IResourceAvailabilityRepository, ITransientService
    {
        private readonly IDbContextFactory<ResourceContext> _factory;

        public ResourceAvailabilityRepository(IDbContextFactory<ResourceContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<Models.ResourceAvailability> GetAvailabilities(int resourceId)
        {
            using var db = _factory.CreateDbContext();
            return db.ResourceAvailability
                .Where(item => item.ResourceId == resourceId)
                .OrderBy(item => item.DayOfWeek)
                .ThenBy(item => item.StartTime)
                .ToList();
        }

        public Models.ResourceAvailability GetAvailability(int availabilityId)
        {
            using var db = _factory.CreateDbContext();
            return db.ResourceAvailability.FirstOrDefault(item => item.AvailabilityId == availabilityId);
        }

        public Models.ResourceAvailability AddAvailability(Models.ResourceAvailability availability)
        {
            using var db = _factory.CreateDbContext();
            db.ResourceAvailability.Add(availability);
            db.SaveChanges();
            return availability;
        }

        public Models.ResourceAvailability UpdateAvailability(Models.ResourceAvailability availability)
        {
            using var db = _factory.CreateDbContext();
            db.Entry(availability).State = EntityState.Modified;
            db.SaveChanges();
            return availability;
        }

        public void DeleteAvailability(int availabilityId)
        {
            using var db = _factory.CreateDbContext();
            var availability = db.ResourceAvailability.Find(availabilityId);
            if (availability != null)
            {
                db.ResourceAvailability.Remove(availability);
                db.SaveChanges();
            }
        }

        public bool IsWithinAvailability(int resourceId, DateTime startTime, DateTime endTime)
        {
            var dayOfWeek = startTime.DayOfWeek;
            var start = startTime.TimeOfDay;
            var end = endTime.TimeOfDay;

            using var db = _factory.CreateDbContext();
            var availabilities = db.ResourceAvailability
                .Where(item => item.ResourceId == resourceId && item.DayOfWeek == dayOfWeek)
                .ToList();

            if (!availabilities.Any())
            {
                return true;
            }

            return availabilities.Any(item => item.StartTime <= start && item.EndTime >= end);
        }
    }
}
