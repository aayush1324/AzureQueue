using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityLMS.Application.DTOs
{
    public class ResultDTO
    {
        public int TestPassed { get; set; }

        public int TestFailed { get; set; }

        public double Result { get; set; }

        public string Grade { get; set; }

        public int Plagiarism { get; set; }
    }

}
