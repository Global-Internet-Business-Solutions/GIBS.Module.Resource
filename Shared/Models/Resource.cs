using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Oqtane.Models;

namespace GIBS.Module.Resource.Models
{
    [Table("GIBSResource")]
    public class Resource : ModelBase
    {
        [Key]
        public int ResourceId { get; set; }
        public int ModuleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ResourceType { get; set; }
        public bool IsActive { get; set; }
        public int MaxCapacity { get; set; }
        public int BufferBeforeMinutes { get; set; }
        public int BufferAfterMinutes { get; set; }
    }
}
