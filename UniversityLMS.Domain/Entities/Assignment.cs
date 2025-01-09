using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityLMS.Domain.Entities
{
    public class Assignment
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime CreatedBy { get; set; }



        public ICollection<TestCase> TestCases { get; set; } // Navigation Property
        public ICollection<Submission> Submissions { get; set; } // Navigation Property
    }

}
