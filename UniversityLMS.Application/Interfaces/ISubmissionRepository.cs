using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Application.DTOs;
using UniversityLMS.Domain.Entities;

namespace UniversityLMS.Application.Interfaces
{
    public interface ISubmissionRepository
    {
        Task<ResultDTO> SubmitAssignmentAsync(SubmissionDTO submission);


        Task<Submission> GetSubmissionByIdAsync(Guid submissionId);
        Task<IEnumerable<Submission>> GetSubmissionsByAssignmentIdAsync(Guid assignmentId);
        Task<IEnumerable<Submission>> GetSubmissionByStudentIdAsync(Guid studentId);

        Task<IEnumerable<Submission>> GetSubmissionsByUserIdAndAssignmentIdAsync(Guid userId, Guid assignmentId);

    }
}
