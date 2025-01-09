using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UniversityLMS.Application.DTOs
{
    public class PlagiarismRequest
    {
        [JsonPropertyName("percentPlagiarism")]
        public int PercentPlagiarism { get; set; }
    }
}
