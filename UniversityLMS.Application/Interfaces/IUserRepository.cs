using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Application.DTOs;
using UniversityLMS.Domain.Entities;

namespace UniversityLMS.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<IActionResult> AddUser([FromBody] User userObj);

        Task<IActionResult> Authenticate([FromBody] LoginDTO loginRequest);

        Task<IActionResult> GetUsers();



        Task<GoogleAddUserResult> GoogleAddUser([FromBody] User userObj);


        Task<IActionResult> GoogleAuthenticate(string email);
       

    }
}
