using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Application.Services;

namespace UniversityLMS.Application.Interfaces
{
    public interface IUnitofWorkRepository : IDisposable
    {
        IAssignmentRepository Assignments { get; }
        ISubmissionRepository Submissions { get; }     
        IPlagiarism PlagiarismDetectionService { get; }
        IJudgeService Judge0Service { get; }
        IUserRepository Users { get; }
      

        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
