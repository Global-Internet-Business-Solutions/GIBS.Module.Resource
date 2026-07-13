using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace GIBS.Module.Resource.Migrations.EntityBuilders
{
    public class ResourceAvailabilityEntityBuilder : AuditableBaseEntityBuilder<ResourceAvailabilityEntityBuilder>
    {
        private const string _entityTableName = "GIBSResourceAvailability";
        private readonly PrimaryKey<ResourceAvailabilityEntityBuilder> _primaryKey = new("PK_GIBSResourceAvailability", x => x.AvailabilityId);
        private readonly ForeignKey<ResourceAvailabilityEntityBuilder> _resourceForeignKey = new("FK_GIBSResourceAvailability_GIBSResource", x => x.ResourceId, "GIBSResource", "ResourceId", ReferentialAction.Cascade);

        public ResourceAvailabilityEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_resourceForeignKey);
        }

        protected override ResourceAvailabilityEntityBuilder BuildTable(ColumnsBuilder table)
        {
            AvailabilityId = AddAutoIncrementColumn(table, "AvailabilityId");
            ResourceId = AddIntegerColumn(table, "ResourceId");
            DayOfWeek = AddIntegerColumn(table, "DayOfWeek");
            StartTime = table.Column<TimeSpan>(name: "StartTime", nullable: false);
            EndTime = table.Column<TimeSpan>(name: "EndTime", nullable: false);
            AddAuditableColumns(table);
            return this;
        }

        public OperationBuilder<AddColumnOperation> AvailabilityId { get; set; }
        public OperationBuilder<AddColumnOperation> ResourceId { get; set; }
        public OperationBuilder<AddColumnOperation> DayOfWeek { get; set; }
        public OperationBuilder<AddColumnOperation> StartTime { get; set; }
        public OperationBuilder<AddColumnOperation> EndTime { get; set; }
    }
}
