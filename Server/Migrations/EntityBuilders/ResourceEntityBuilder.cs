using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace GIBS.Module.Resource.Migrations.EntityBuilders
{
    public class ResourceEntityBuilder : AuditableBaseEntityBuilder<ResourceEntityBuilder>
    {
        private const string _entityTableName = "GIBSResource";
        private readonly PrimaryKey<ResourceEntityBuilder> _primaryKey = new("PK_GIBSResource", x => x.ResourceId);
        private readonly ForeignKey<ResourceEntityBuilder> _moduleForeignKey = new("FK_GIBSResource_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public ResourceEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override ResourceEntityBuilder BuildTable(ColumnsBuilder table)
        {
            ResourceId = AddAutoIncrementColumn(table,"ResourceId");
            ModuleId = AddIntegerColumn(table,"ModuleId");
            Name = AddMaxStringColumn(table,"Name");
            Description = AddMaxStringColumn(table,"Description", true);
            ResourceType = AddMaxStringColumn(table,"ResourceType", true);
            IsActive = AddBooleanColumn(table,"IsActive");
            MaxCapacity = AddIntegerColumn(table,"MaxCapacity");
            BufferBeforeMinutes = AddIntegerColumn(table,"BufferBeforeMinutes");
            BufferAfterMinutes = AddIntegerColumn(table,"BufferAfterMinutes");
            AddAuditableColumns(table);
            return this;
        }

        public OperationBuilder<AddColumnOperation> ResourceId { get; set; }
        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
        public OperationBuilder<AddColumnOperation> Name { get; set; }
        public OperationBuilder<AddColumnOperation> Description { get; set; }
        public OperationBuilder<AddColumnOperation> ResourceType { get; set; }
        public OperationBuilder<AddColumnOperation> IsActive { get; set; }
        public OperationBuilder<AddColumnOperation> MaxCapacity { get; set; }
        public OperationBuilder<AddColumnOperation> BufferBeforeMinutes { get; set; }
        public OperationBuilder<AddColumnOperation> BufferAfterMinutes { get; set; }
    }
}
