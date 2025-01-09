using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityLMS.Domain.Entities
{
    public class TestCase
    {
        public Guid Id { get; set; }

        public Guid AssignmentId { get; set; } 
        public Assignment Assignment { get; set; } 


        public string Input { get; set; } 

        public string ExpectedOutput { get; set; }

    }
}
