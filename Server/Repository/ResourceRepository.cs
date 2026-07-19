using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;

namespace GIBS.Module.Resource.Repository
{
    public interface IResourceRepository
    {
        IEnumerable<Models.Resource> GetResources(int ModuleId);
        Models.Resource GetResource(int ResourceId);
        Models.Resource GetResource(int ResourceId, bool tracking);
        Models.Resource GetResourceForModule(int ResourceId, int ModuleId, bool activeOnly = false, bool tracking = false);
        Models.Resource AddResource(Models.Resource Resource);
        Models.Resource UpdateResource(Models.Resource Resource);
        void DeleteResource(int ResourceId);
    }

    public class ResourceRepository : IResourceRepository, ITransientService
    {
        private readonly IDbContextFactory<ResourceContext> _factory;

        public ResourceRepository(IDbContextFactory<ResourceContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<Models.Resource> GetResources(int ModuleId)
        {
            using var db = _factory.CreateDbContext();
            return db.Resource.Where(item => item.ModuleId == ModuleId).ToList();
        }

        public Models.Resource GetResource(int ResourceId)
        {
            return GetResource(ResourceId, true);
        }

        public Models.Resource GetResource(int ResourceId, bool tracking)
        {
            using var db = _factory.CreateDbContext();
            if (tracking)
            {
                return db.Resource.Find(ResourceId);
            }
            else
            {
                return db.Resource.AsNoTracking().FirstOrDefault(item => item.ResourceId == ResourceId);
            }
        }

        public Models.Resource GetResourceForModule(int ResourceId, int ModuleId, bool activeOnly = false, bool tracking = false)
        {
            using var db = _factory.CreateDbContext();
            var query = tracking ? db.Resource : db.Resource.AsNoTracking();
            return query.FirstOrDefault(item =>
                item.ResourceId == ResourceId &&
                item.ModuleId == ModuleId &&
                (!activeOnly || item.IsActive));
        }

        public Models.Resource AddResource(Models.Resource Resource)
        {
            using var db = _factory.CreateDbContext();
            db.Resource.Add(Resource);
            db.SaveChanges();
            return Resource;
        }

        public Models.Resource UpdateResource(Models.Resource Resource)
        {
            using var db = _factory.CreateDbContext();
            db.Entry(Resource).State = EntityState.Modified;
            db.SaveChanges();
            return Resource;
        }

        public void DeleteResource(int ResourceId)
        {
            using var db = _factory.CreateDbContext();
            Models.Resource Resource = db.Resource.Find(ResourceId);
            db.Resource.Remove(Resource);
            db.SaveChanges();
        }
    }
}
