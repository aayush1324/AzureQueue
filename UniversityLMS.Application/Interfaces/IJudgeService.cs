using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Application.DTOs;

namespace UniversityLMS.Application.Interfaces
{
    public interface IJudgeService
    {
        Task<string> EvaluateCode(string code, int language, string input);

        Task<int> AssignmentResult(string sourceCode, IEnumerable<TestCaseDTO> testCases, int LanguageCode);
    }
}
