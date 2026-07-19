using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Oqtane.Models;

namespace GIBS.Module.Resource.Models
{
    [Table("GIBSReservation")]
    public class Reservation : ModelBase
    {
        [Key]
        public int ReservationId { get; set; }
        public int ResourceId { get; set; }
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ReservationStatus Status { get; set; }
        public string Notes { get; set; }

        [NotMapped]
        public string UserName { get; set; }

    }
}
