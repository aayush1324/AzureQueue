using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Application.DTOs;

namespace UniversityLMS.Application.Interfaces
{
    public interface IPlagiarism
    {
        Task<PlagiarismRequest> PlagiarismCodeAsync(string code);
    }
}
