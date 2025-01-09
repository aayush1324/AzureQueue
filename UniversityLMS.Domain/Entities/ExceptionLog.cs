using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityLMS.Domain.Entities
{
    public class ExceptionLog
    {
        public int Id { get; set; } // Primary key
        public string ExceptionMessage { get; set; } = string.Empty; // Exception message
        public string? StackTrace { get; set; } // Stack trace, nullable
        public string? Source { get; set; } // Exception source, nullable
        public DateTime LoggedAt { get; set; } = DateTime.UtcNow; // Default to current time

        public string RequestPath { get; set; }
        public string RequestMethod { get; set; }
        public string? UserId { get; set; }
    }
}
