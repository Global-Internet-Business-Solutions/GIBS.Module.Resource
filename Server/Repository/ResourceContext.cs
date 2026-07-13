using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Infrastructure;
using Oqtane.Repository.Databases.Interfaces;

namespace GIBS.Module.Resource.Repository
{
    public class ResourceContext : DBContextBase, ITransientService, IMultiDatabase
    {
        public virtual DbSet<Models.Resource> Resource { get; set; }
        public virtual DbSet<Models.Reservation> Reservation { get; set; }
        public virtual DbSet<Models.RecurrencePattern> RecurrencePattern { get; set; }
        public virtual DbSet<Models.ResourceAvailability> ResourceAvailability { get; set; }

        public ResourceContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
        {
            // ContextBase handles multi-tenant database connections
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Models.Resource>().ToTable(ActiveDatabase.RewriteName("GIBSResource"));
            builder.Entity<Models.Reservation>().ToTable(ActiveDatabase.RewriteName("GIBSReservation"));
            builder.Entity<Models.RecurrencePattern>().ToTable(ActiveDatabase.RewriteName("GIBSRecurrencePattern"));
            builder.Entity<Models.ResourceAvailability>().ToTable(ActiveDatabase.RewriteName("GIBSResourceAvailability"));
        }
    }
}
