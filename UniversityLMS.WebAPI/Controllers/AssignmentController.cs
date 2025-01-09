using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversityLMS.Application.DTOs;
using UniversityLMS.Application.Interfaces;
using UniversityLMS.Domain.Entities;

namespace UniversityLMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IUnitofWorkRepository _unitOfWork;


        public AssignmentsController(IAssignmentRepository assignmentRepository, IUnitofWorkRepository unitofwork)
        {
            _assignmentRepository = assignmentRepository;
            _unitOfWork = unitofwork;
        }


        //[Authorize(Policy = "RequireTeacherRole")]
        //[HttpPost]
        //public async Task<IActionResult> CreateAssignment([FromBody] AssignmentDTO assignmentDto)
        //{
        //    if (assignmentDto == null)
        //        throw new Exception("Assignment data is required.");


        //    await _assignmentRepository.AddAssignmentAsync(assignmentDto);

        //    return Ok(new { message = "Assignment created successfully." });
        //}

        [Authorize(Policy = "RequireTeacherRole")]
        [HttpPost]
        public async Task<IActionResult> CreateAssignment([FromBody] AssignmentDTO assignmentDto)
        {
            if (assignmentDto == null)
                return BadRequest(new { message = "Assignment data is required." });

            try
            {
                // Begin a transaction
                await _unitOfWork.BeginTransactionAsync();

                // Add assignment using Unit of Work
                await _unitOfWork.Assignments.AddAssignmentAsync(assignmentDto);

                // Commit the transaction
                await _unitOfWork.CommitTransactionAsync();

                return Ok(new { message = "Assignment created successfully." });
            }
            catch (Exception ex)
            {
                // Rollback the transaction on failure
                await _unitOfWork.RollbackTransactionAsync();
                return StatusCode(500, new { message = "An error occurred while creating the assignment.", error = ex.Message });
            }
        }



        //[Authorize(Policy = "RequireTeacherRole")]
        //[HttpGet]
        //public async Task<IActionResult> GetAllAssignments()
        //{
        //    var assignments = await _assignmentRepository.GetAllAssignmentsAsync();
        //    return Ok(assignments);
        //}

        [Authorize(Policy = "RequireTeacherRole")]
        [HttpGet]
        public async Task<IActionResult> GetAllAssignments()
        {
            try
            {
                // Fetch assignments using Unit of Work
                var assignments = await _unitOfWork.Assignments.GetAllAssignmentsAsync();

                if (assignments == null || !assignments.Any())
                    return NotFound(new { message = "No assignments found." });

                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving assignments.", error = ex.Message });
            }
        }




        //[HttpGet("{id:guid}")]
        //public async Task<IActionResult> GetAssignmentById(Guid id)
        //{
        //    var assignment = await _assignmentRepository.GetAssignmentByIdAsync(id);

        //    if (assignment == null)
        //        return NotFound($"Assignment with ID {id} not found.");

        //    return Ok(assignment);
        //}

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetAssignmentById(Guid id)
        {
            try
            {
                // Fetch the assignment using Unit of Work
                var assignment = await _unitOfWork.Assignments.GetAssignmentByIdAsync(id);

                if (assignment == null)
                    return NotFound(new { message = $"Assignment with ID {id} not found." });

                return Ok(assignment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the assignment.", error = ex.Message });
            }
        }


    }


}
