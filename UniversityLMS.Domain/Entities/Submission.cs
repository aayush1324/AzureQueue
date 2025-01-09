using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityLMS.Domain.Entities
{
    public class Submission
    {
        public Guid Id { get; set; } // Primary key

        public Guid AssignmentId { get; set; } // Foreign key to Assignment
        public Assignment Assignment { get; set; }


        public Guid UserId { get; set; } // Foreign key to Student
        public User User { get; set; }


        public string LanguageCode { get; set; }

        //public string SourceCode { get; set; }

        public string BlobUri { get; set; }

        public int TestPassed { get; set; }

        public int TestFailed { get; set; }

        public double Result { get; set; }

        public string Grade { get; set; }

        public int Plagiarism { get; set; }

        public DateTime SubmissionDate { get; set; }

    }

}
