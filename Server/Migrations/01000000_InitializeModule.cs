using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using GIBS.Module.Resource.Migrations.EntityBuilders;
using GIBS.Module.Resource.Repository;

namespace GIBS.Module.Resource.Migrations
{
    [DbContext(typeof(ResourceContext))]
    [Migration("GIBS.Module.Resource.01.00.00.00")]
    public class InitializeModule : MultiDatabaseMigration
    {
        public InitializeModule(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var resourceEntityBuilder = new ResourceEntityBuilder(migrationBuilder, ActiveDatabase);
            resourceEntityBuilder.Create();

            var reservationEntityBuilder = new ReservationEntityBuilder(migrationBuilder, ActiveDatabase);
            reservationEntityBuilder.Create();

            var recurrencePatternEntityBuilder = new RecurrencePatternEntityBuilder(migrationBuilder, ActiveDatabase);
            recurrencePatternEntityBuilder.Create();

            var resourceAvailabilityEntityBuilder = new ResourceAvailabilityEntityBuilder(migrationBuilder, ActiveDatabase);
            resourceAvailabilityEntityBuilder.Create();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var recurrencePatternEntityBuilder = new RecurrencePatternEntityBuilder(migrationBuilder, ActiveDatabase);
            recurrencePatternEntityBuilder.Drop();

            var resourceAvailabilityEntityBuilder = new ResourceAvailabilityEntityBuilder(migrationBuilder, ActiveDatabase);
            resourceAvailabilityEntityBuilder.Drop();

            var reservationEntityBuilder = new ReservationEntityBuilder(migrationBuilder, ActiveDatabase);
            reservationEntityBuilder.Drop();

            var resourceEntityBuilder = new ResourceEntityBuilder(migrationBuilder, ActiveDatabase);
            resourceEntityBuilder.Drop();
        }
    }
}
