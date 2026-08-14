using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using GIBS.Module.Resource.Repository;

namespace GIBS.Module.Resource.Migrations
{
    [DbContext(typeof(ResourceContext))]
    [Migration("GIBS.Module.Resource.01.00.01.00")]
    public class AddOptInToReservation : MultiDatabaseMigration
    {
        public AddOptInToReservation(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OptIn",
                table: "GIBSReservation",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OptIn",
                table: "GIBSReservation");
        }
    }
}
