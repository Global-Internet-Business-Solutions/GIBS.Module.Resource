using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace GIBS.Module.Resource.Migrations.EntityBuilders
{
    public class RecurrencePatternEntityBuilder : AuditableBaseEntityBuilder<RecurrencePatternEntityBuilder>
    {
        private const string _entityTableName = "GIBSRecurrencePattern";
        private readonly PrimaryKey<RecurrencePatternEntityBuilder> _primaryKey = new("PK_GIBSRecurrencePattern", x => x.PatternId);
        private readonly ForeignKey<RecurrencePatternEntityBuilder> _reservationForeignKey = new("FK_GIBSRecurrencePattern_GIBSReservation", x => x.ReservationId, "GIBSReservation", "ReservationId", ReferentialAction.Cascade);

        public RecurrencePatternEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_reservationForeignKey);
        }

        protected override RecurrencePatternEntityBuilder BuildTable(ColumnsBuilder table)
        {
            PatternId = AddAutoIncrementColumn(table, "PatternId");
            ReservationId = AddIntegerColumn(table, "ReservationId");
            RRule = AddMaxStringColumn(table, "RRule");
            AddAuditableColumns(table);
            return this;
        }

        public OperationBuilder<AddColumnOperation> PatternId { get; set; }
        public OperationBuilder<AddColumnOperation> ReservationId { get; set; }
        public OperationBuilder<AddColumnOperation> RRule { get; set; }
    }
}
