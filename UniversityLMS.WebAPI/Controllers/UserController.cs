using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversityLMS.Application.DTOs;
using UniversityLMS.Application.Interfaces;
using UniversityLMS.Domain.Entities;

namespace UniversityLMS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
       
        private readonly IUnitofWorkRepository _unitOfWork;


        public UserController(IConfiguration configuration, IUnitofWorkRepository unitofWork)
        {
            _configuration = configuration;
            
            _unitOfWork = unitofWork;
        }



        //[HttpPost("register")]
        //public async Task<IActionResult> AddUser([FromBody] User userObj)
        //{
        //    if (userObj == null)
        //    {
        //        return BadRequest();
        //    }

        //    return await _userRepository.AddUser(userObj);
        //}

        [HttpPost("register")]
        public async Task<IActionResult> AddUser([FromBody] User userObj)
        {
            await _unitOfWork.BeginTransactionAsync();
            if (userObj == null)
            {
                return BadRequest();
            }
                try
                {
                    var result = await _unitOfWork.Users.AddUser(userObj);
                    await _unitOfWork.CompleteAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    return Ok(result);
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
        

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginDTO loginRequest)
        {
            if (loginRequest == null)
            {
                return BadRequest();
            }

            return await _unitOfWork.Users.Authenticate(loginRequest);
        }


        [HttpGet("getUser")]
        public async Task<IActionResult> GetUser()
        {
            return await _unitOfWork.Users.GetUsers();
        }







        //[HttpPost("registerGoogle")]
        //public async Task<IActionResult> GoogleAddUser([FromBody] User userObj)
        //{
        //    if (userObj == null)
        //    {
        //        return BadRequest("Invalid request: Email is required.");
        //    }

        //    // Add the user to the database
        //    var addUserResult = await _userRepository.GoogleAddUser(userObj);

        //    if (addUserResult.Message != "User Added!")
        //    {
        //        // Return a BadRequest with the result's message
        //        return BadRequest(new { Message = addUserResult.Message });
        //    }


        //    var authResult = await _userRepository.GoogleAuthenticate(userObj.Email);

        //    if (authResult == null)
        //    {
        //        return NotFound("User not found.");
        //    }

        //    return Ok(new
        //    {
        //        Message = addUserResult.Message,
        //        Token = addUserResult.Token
        //    });

        //}


       
         


        //[HttpPost("authenticateGoogle")]
        //public async Task<IActionResult> GoogleAuthenticate([FromBody] GoogleEmailDTO googleEmailDTO)
        //{
        //    if (googleEmailDTO == null || string.IsNullOrEmpty(googleEmailDTO.Email))
        //    {
        //        return BadRequest("Invalid request: Email is required.");
        //    }

        //    var result = await _userRepository.GoogleAuthenticate(googleEmailDTO.Email);

        //    if (result == null)
        //    {
        //        return NotFound("User not found.");
        //    }

        //    return Ok(result); // Assuming result contains the necessary user data or token
        //}






    }
}
