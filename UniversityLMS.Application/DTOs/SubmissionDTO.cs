using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Domain.Entities;

namespace UniversityLMS.Application.DTOs
{
    public class SubmissionDTO
    {
        public Guid AssignmentId { get; set; } // Foreign key to Assignment

        public Guid UserId { get; set; } // Foreign key to Student

        public int LanguageCode { get; set; }

        public IFormFile SourceCode { get; set; }
    }
    public class SubmissionAssignmentDTO
    {
        public Guid AssignmentId { get; set; } // Foreign key to Assignment

        public Guid UserId { get; set; } // Foreign key to Student

        public int LanguageCode { get; set; }

        public string SourceCode { get; set; }
        public string FilePath { get; set; }
    }
}
