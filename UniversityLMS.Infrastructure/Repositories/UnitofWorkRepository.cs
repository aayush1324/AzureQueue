using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Application.Interfaces;
using UniversityLMS.Application.Services;
using UniversityLMS.Infrastructure.DbContexts;

namespace UniversityLMS.Infrastructure.Repositories
{
    public class UnitofWorkRepository : IUnitofWorkRepository
    {
        private readonly AppDbContext _context;

        public IAssignmentRepository Assignments { get; private set; }
        public ISubmissionRepository Submissions { get; private set; }
        public IUserRepository Users { get; private set; }

        public IPlagiarism PlagiarismDetectionService { get; private set; }
        public IJudgeService Judge0Service { get; private set; }

        private IDbContextTransaction _currentTransaction;

        public UnitofWorkRepository(AppDbContext context,
                                      IAssignmentRepository assignmentRepository,
                                      ISubmissionRepository submissionRepository,
                                      IPlagiarism plagiarismDetectionService,
                                      IJudgeService judge0Service,
                                      IUserRepository userRepository)
        {
            _context = context;
            Assignments = assignmentRepository;
            Submissions = submissionRepository;
            PlagiarismDetectionService = plagiarismDetectionService;
            Judge0Service = judge0Service;
            Users = userRepository;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task BeginTransactionAsync()
        {
            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
        public void Dispose()
        {
            _context.Dispose();
        }
       
    }
}
