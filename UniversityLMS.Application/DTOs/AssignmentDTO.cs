using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Domain.Entities;

namespace UniversityLMS.Application.DTOs
{
    public class AssignmentDTO
    {
        public string Title { get; set; }

        public string Description { get; set; }

    
        public DateTime DueDate { get; set; }


        public ICollection<TestCaseDTO> TestCases { get; set; }
    }



    public class TestCaseDTO
    {
        public string Input { get; set; }

        public string ExpectedOutput { get; set; }
    }
}
