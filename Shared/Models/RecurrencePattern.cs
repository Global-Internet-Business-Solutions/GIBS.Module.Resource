using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Oqtane.Models;

namespace GIBS.Module.Resource.Models
{
    [Table("GIBSRecurrencePattern")]
    public class RecurrencePattern : ModelBase
    {
        [Key]
        public int PatternId { get; set; }
        public int ReservationId { get; set; }
        public string RRule { get; set; }
    }
}
