using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Oqtane.Models;

namespace GIBS.Module.Resource.Models
{
    [Table("GIBSResourceAvailability")]
    public class ResourceAvailability : ModelBase
    {
        [Key]
        public int AvailabilityId { get; set; }
        public int ResourceId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
