using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Application.DTOs;
using UniversityLMS.Application.Interfaces;
using UniversityLMS.Application.Services;
using UniversityLMS.Domain.Entities;
using UniversityLMS.Infrastructure.DbContexts;
using static System.Formats.Asn1.AsnWriter;

namespace UniversityLMS.Infrastructure.Repositories
{
    public class SubmissionRepository : ISubmissionRepository
    {
        private readonly AppDbContext _context;
        private readonly IJudgeService _judgeService;
        private readonly IPlagiarism _plagiarism;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IOutboxMessageRepository outboxMessageRepository;

        public SubmissionRepository(AppDbContext context, IJudgeService judgeService, IPlagiarism plagiarism, IBlobStorageService blobStorageService, IOutboxMessageRepository outboxMessageRepository)
        {
            _context = context;
            _judgeService = judgeService;
            _plagiarism = plagiarism;
            _blobStorageService = blobStorageService;
            this.outboxMessageRepository = outboxMessageRepository;
        }


        public async Task<ResultDTO> SubmitAssignmentAsync(SubmissionDTO submission)
        {
            using var stream = submission.SourceCode.OpenReadStream();
            var blobName = $"{submission.UserId}{submission.AssignmentId}{DateTime.UtcNow.Ticks}.txt";
            var blobUri = await _blobStorageService.UploadBlobAsync(blobName, stream);

            var sourceCodetext = await ConvertFileToStringAsync(submission.SourceCode);

      

            var plagiarism = await _plagiarism.PlagiarismCodeAsync(sourceCodetext);

            if (plagiarism.PercentPlagiarism > 40)
                throw new InvalidOperationException("FAILED");

            var testCases = await _context.TestCases
                                        .Where(tc => tc.AssignmentId == submission.AssignmentId)
                                        .Select(tc => new TestCaseDTO
                                        {
                                            Input = tc.Input,
                                            ExpectedOutput = tc.ExpectedOutput,
                                        })
                                        .ToListAsync();

            var testcasesPassed = await _judgeService.AssignmentResult(sourceCodetext, testCases, submission.LanguageCode);
            var totalCases = testCases.Count();
            var testcasesFailed = totalCases - testcasesPassed;

            double result = totalCases > 0 ? (testcasesPassed / (double)totalCases) * 100 : 0;

            var grade = "F";

            if (result > 80)
            {
                 grade = "A";
            }
            else if (result > 60 && result < 80)
            {
                 grade = "B";
            }
            else if (result > 40 && result < 60)
            {
                 grade = "C";
            }
            else if (result > 20 && result < 40)
            {
                 grade = "D";
            }
            else if (result < 20)
            {
                 grade = "E";
            }



            // Create the submission object
            var submissionnew = new Submission
            {
                AssignmentId = submission.AssignmentId,
                UserId = submission.UserId,
                LanguageCode = submission.LanguageCode.ToString(),
                BlobUri = blobUri,
                TestPassed = testcasesPassed,
                TestFailed = testcasesFailed,
                Result = result,
                Grade = grade,
                Plagiarism = plagiarism.PercentPlagiarism,
            };

            await _context.Submissions.AddAsync(submissionnew);
            _context.SaveChanges();
            var outboxMessage = new OutboxMessage
            {
                FilePath = blobUri,
                AssignmentId = submission.AssignmentId,
                UserId = submission.UserId,
                LanguageId = submission.LanguageCode,
                CreatedAt = DateTime.UtcNow,
                IsProcessed = false,
                SubmissionId = submissionnew.Id,
            };
            await outboxMessageRepository.AddSubmission(outboxMessage);
            var queueMessage = await _blobStorageService.PushToQueue(outboxMessage);
            var finalresult = new ResultDTO
            {
                TestPassed = testcasesPassed,
                TestFailed = testcasesFailed,
                Result = result,
                Grade = grade,
                Plagiarism = plagiarism.PercentPlagiarism,

            };
            return finalresult;

        }



        private async Task<string> ConvertFileToStringAsync(IFormFile file)
        {
            using var reader = new StreamReader(file.OpenReadStream());
            return await reader.ReadToEndAsync();
        }






        public async Task<Submission> GetSubmissionByIdAsync(Guid submissionId)
        {
            // Use Entity Framework Core to retrieve the submission by ID
            return  await _context.Submissions
                                 .Include(s => s.Assignment) // Include related entities if necessary
                                 .Include(s => s.User)       // Example: Including user details
                                 .FirstOrDefaultAsync(s => s.Id == submissionId);
        }

        public async Task<IEnumerable<Submission>> GetSubmissionByStudentIdAsync(Guid studentId)
        {
            return await _context.Submissions
                                        .Where(s => s.UserId == studentId)
                                        .Include(s => s.Assignment) // Optional
                                        .ToListAsync();
        }

        public async Task<IEnumerable<Submission>> GetSubmissionsByAssignmentIdAsync(Guid assignmentId)
        {
            return await _context.Submissions
                                        .Where(s => s.AssignmentId == assignmentId)
                                        .Include(s => s.User) 
                                        .ToListAsync();

        }

        public async Task<IEnumerable<Submission>> GetSubmissionsByUserIdAndAssignmentIdAsync(Guid userId, Guid assignmentId)
        {
            return await _context.Submissions
                                 .Where(s => s.UserId == userId && s.AssignmentId == assignmentId)
                                 .Include(s => s.User) // Optional: Include related entities
                                 .ToListAsync();
        }



    }
}
