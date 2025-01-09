using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversityLMS.Application.DTOs;
using UniversityLMS.Application.Interfaces;
using UniversityLMS.Domain.Entities;

namespace UniversityLMS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionController : ControllerBase
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IUnitofWorkRepository _unitOfWork;
      


        public SubmissionController(ISubmissionRepository submissionRepository, IUnitofWorkRepository unitofwork)
        {
            _submissionRepository = submissionRepository;
            _unitOfWork = unitofwork;
            
        }


        //[HttpPost("submit-assignment")]
        //public async Task<IActionResult> SubmitAssignment([FromForm] SubmissionDTO submissionDTO)
        //{
        //    var result = await _submissionRepository.SubmitAssignmentAsync(submissionDTO);

        //    return Ok(new { message = "Assignment submitted successfully"});
        //}


        [HttpPost("submit-assignment")]
        public async Task<IActionResult> SubmitAssignment([FromForm] SubmissionDTO submissionDTO)
        {
            if (submissionDTO == null)
            {
                return BadRequest("Invalid submission data.");
            }

            try
            {
                // Begin a transaction
                await _unitOfWork.BeginTransactionAsync();

                // Save the submission details
                var result = await _unitOfWork.Submissions.SubmitAssignmentAsync(submissionDTO);

                // Commit the transaction if everything succeeds
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                return Ok(new { message = "Assignment submitted successfully" });
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of any error
                await _unitOfWork.RollbackTransactionAsync();
                return StatusCode(500, new { message = "An error occurred while submitting the assignment.", error = ex.Message });
            }
        }





        //[HttpGet("get-submission/{submissionId}")]
        //public async Task<IActionResult> GetSubmission(Guid submissionId)
        //{
        //    var submission = await _submissionRepository.GetSubmissionByIdAsync(submissionId);

        //    if (submission == null)
        //        return NotFound("Submission not found.");

        //    return Ok(submission);
        //}

        [HttpGet("get-submission/{submissionId}")]
        public async Task<IActionResult> GetSubmission(Guid submissionId)
        {
            try
            {
                var submission = await _unitOfWork.Submissions.GetSubmissionByIdAsync(submissionId);

                if (submission == null)
                    return NotFound("Submission not found.");

                return Ok(submission);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the submission.", error = ex.Message });
            }
        }





        //[HttpGet("get-submissionByStudent/{studentId}")]
        //public async Task<IActionResult> GetSubmissionByStudent(Guid studentId)
        //{
        //    var submission = await _submissionRepository.GetSubmissionByStudentIdAsync(studentId);
        //    if (submission == null)
        //        return NotFound("Student Submission not found.");

        //    return Ok(submission);
        //}

        [HttpGet("get-submissionByStudent/{studentId}")]
        public async Task<IActionResult> GetSubmissionByStudent(Guid studentId)
        {
            try
            {
                var submission = await _unitOfWork.Submissions.GetSubmissionByStudentIdAsync(studentId);

                if (submission == null)
                    return NotFound("Student Submission not found.");

                return Ok(submission);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the student's submissions.", error = ex.Message });
            }
        }






        //[HttpGet("get-submissions/{assignmentId}")]
        //public async Task<IActionResult> GetSubmissionsByAssignment(Guid assignmentId)
        //{
        //    var submissions = await _submissionRepository.GetSubmissionsByAssignmentIdAsync(assignmentId);
        //    return Ok(submissions);
        //}

        [HttpGet("get-submissions/{assignmentId}")]
        public async Task<IActionResult> GetSubmissionsByAssignment(Guid assignmentId)
        {
            try
            {
                var submissions = await _unitOfWork.Submissions.GetSubmissionsByAssignmentIdAsync(assignmentId);

                if (submissions == null || !submissions.Any())
                    return NotFound("No submissions found for this assignment.");

                return Ok(submissions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the submissions.", error = ex.Message });
            }
        }


        //[HttpGet("get-submissions/{userId}/{assignmentId}")]
        //public async Task<IActionResult> GetSubmissionsByUserIdAndAssignmentId(Guid userId, Guid assignmentId)
        //{
        //    var submissions = await _submissionRepository.GetSubmissionsByUserIdAndAssignmentIdAsync(userId, assignmentId);

        //    if (submissions == null || !submissions.Any())
        //        return NotFound("No submissions found for the specified user and assignment.");

        //    return Ok(submissions);
        //}


        [HttpGet("get-submissions/{userId}/{assignmentId}")]
        public async Task<IActionResult> GetSubmissionsByUserIdAndAssignmentId( Guid assignmentId)
        {
            try
            {
                // Begin transaction
                await _unitOfWork.BeginTransactionAsync();
                var userIdClaim = HttpContext.User?.FindFirst("UserID");
                bool val=Guid.TryParse(userIdClaim.Value, out Guid UserId);
                var userId = UserId;

                // Fetch submissions using the unit of work
                var submissions = await _unitOfWork.Submissions.GetSubmissionsByUserIdAndAssignmentIdAsync(userId, assignmentId);

                if (submissions == null || !submissions.Any())
                {
                    // Rollback if no submissions found
                    await _unitOfWork.RollbackTransactionAsync();
                    return NotFound("No submissions found for the specified user and assignment.");
                }

                // Commit transaction if successful
                await _unitOfWork.CommitTransactionAsync();

                return Ok(submissions);
            }
            catch (Exception ex)
            {
                // Rollback transaction in case of error
                await _unitOfWork.RollbackTransactionAsync();
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }


    }
}
