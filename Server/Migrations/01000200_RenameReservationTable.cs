using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using GIBS.Module.Resource.Repository;

namespace GIBS.Module.Resource.Migrations
{
    [DbContext(typeof(ResourceContext))]
    [Migration("GIBS.Module.Resource.01.00.02.00")]
    public class RenameReservationTable : MultiDatabaseMigration
    {
        public RenameReservationTable(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "GIBSReservation",
                newName: "GIBSResourceReservation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "GIBSResourceReservation",
                newName: "GIBSReservation");
        }
    }
}
