using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityLMS.Domain.Entities
{
    public class SubmitAssignment
    {
        public Guid Id { get; set; } // Primary key

        public Guid AssignmentId { get; set; } // Foreign key to Assignment
        public Assignment Assignment { get; set; }


        public Guid UserId { get; set; } // Foreign key to Student
        public User User { get; set; }


        public string LanguageCode { get; set; }

        //public string SourceCode { get; set; }

        public string BlobUri { get; set; }


        public DateTime SubmissionDate { get; set; }
    }
}
