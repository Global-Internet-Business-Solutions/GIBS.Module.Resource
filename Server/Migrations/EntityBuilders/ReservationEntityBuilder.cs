using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace GIBS.Module.Resource.Migrations.EntityBuilders
{
    public class ReservationEntityBuilder : AuditableBaseEntityBuilder<ReservationEntityBuilder>
    {
        private const string _entityTableName = "GIBSReservation";
        private readonly PrimaryKey<ReservationEntityBuilder> _primaryKey = new("PK_GIBSReservation", x => x.ReservationId);
        private readonly ForeignKey<ReservationEntityBuilder> _resourceForeignKey = new("FK_GIBSReservation_GIBSResource", x => x.ResourceId, "GIBSResource", "ResourceId", ReferentialAction.Cascade);

        public ReservationEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_resourceForeignKey);
        }

        protected override ReservationEntityBuilder BuildTable(ColumnsBuilder table)
        {
            ReservationId = AddAutoIncrementColumn(table, "ReservationId");
            ResourceId = AddIntegerColumn(table, "ResourceId");
            UserId = AddIntegerColumn(table, "UserId");
            StartTime = AddDateTimeColumn(table, "StartTime");
            EndTime = AddDateTimeColumn(table, "EndTime");
            Status = AddIntegerColumn(table, "Status");
            Notes = AddMaxStringColumn(table, "Notes", true);
            AddAuditableColumns(table);
            return this;
        }

        public OperationBuilder<AddColumnOperation> ReservationId { get; set; }
        public OperationBuilder<AddColumnOperation> ResourceId { get; set; }
        public OperationBuilder<AddColumnOperation> UserId { get; set; }
        public OperationBuilder<AddColumnOperation> StartTime { get; set; }
        public OperationBuilder<AddColumnOperation> EndTime { get; set; }
        public OperationBuilder<AddColumnOperation> Status { get; set; }
        public OperationBuilder<AddColumnOperation> Notes { get; set; }
    }
}
