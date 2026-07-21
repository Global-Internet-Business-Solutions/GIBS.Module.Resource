using System;

namespace GIBS.Module.Resource.Models
{
    public class SchedulePdfRequest
    {
        public int ModuleId { get; set; }
        public string ViewType { get; set; } // "daily" or "weekly"
        public DateTime SelectedDate { get; set; }
        public DateTime WeekStartDate { get; set; }
        public int ResourceId { get; set; } // 0 means all resources (daily only)
    }
}
