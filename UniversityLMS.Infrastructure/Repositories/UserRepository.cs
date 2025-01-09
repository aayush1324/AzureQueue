using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UniversityLMS.Application.DTOs;
using UniversityLMS.Application.Interfaces;
using UniversityLMS.Domain.Entities;
using UniversityLMS.Infrastructure.DbContexts;

namespace UniversityLMS.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _authContext;
        private readonly IConfiguration _configuration;
        

        public UserRepository(AppDbContext context, IConfiguration configuration)
        {
            _authContext = context;
            _configuration = configuration;
        }
        

        public async Task<IActionResult> AddUser([FromBody] User userObj)
        {
            if (userObj == null)
                return new BadRequestObjectResult("Invalid user");

            // Check if the email already exists
            if (await CheckEmailExistAsync(userObj.Email))
                return new BadRequestObjectResult("Email Already Exist");

            // Check password strength
            var passMessage = CheckPasswordStrength(userObj.Password);
            if (!string.IsNullOrEmpty(passMessage))
                return new BadRequestObjectResult(passMessage);

            // Hash the password
            userObj.Password = PasswordHasher.HashPassword(userObj.Password);

            // Set user role and generate JWT token
            userObj.Role = userObj.Role;


            // Add the user to the database
            await _authContext.Users.AddAsync(userObj);
            await _authContext.SaveChangesAsync();

            return new OkObjectResult(new
            {
                Status = 200,
                Message = "Student Enrolled!",
            });
        }


        public async Task<IActionResult> Authenticate([FromBody] LoginDTO loginRequest)
        {
            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return new BadRequestObjectResult(new { Message = "Invalid input. Email and Password are required." });
            }
            
            var user = await _authContext.Users.FirstOrDefaultAsync(x => x.Email == loginRequest.Email);
            var token = CreateJwt(user);

            if (user == null)
            {
                return new NotFoundObjectResult(new { Message = "User not found." });
            }

            if (!PasswordHasher.VerifyPassword(loginRequest.Password, user.Password))
            {
                return new UnauthorizedObjectResult(new { Message = "Password is incorrect." });
            }

            // Optional: Update the user's last login time or other data
            _authContext.Entry(user).State = EntityState.Modified;
            await _authContext.SaveChangesAsync();

            return new OkObjectResult(new
            {
                Message = "Login success!",
                Token=token,

            });
        }


        public async Task<IActionResult> GetUsers()
        {
            var userList = await _authContext.Users
                                                .Select(user => new UserDTO
                                                {
                                                    Id = user.Id,
                                                    Email = user.Email,
                                                    FirstName = user.FirstName,
                                                    LastName = user.LastName,
                                                    UniversityID = user.UniversityID,
                                                    Phone = user.Phone,
                                                    Role = user.Role,
                                                })
                                                .ToListAsync();

            if (userList == null || userList.Count == 0)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(userList);
        }



        private static string CheckPasswordStrength(string pass)
        {
            StringBuilder sb = new StringBuilder();
            if (pass.Length < 9)
                sb.Append("Minimum password length should be 8" + Environment.NewLine);
            if (!(Regex.IsMatch(pass, "[a-z]") && Regex.IsMatch(pass, "[A-Z]") && Regex.IsMatch(pass, "[0-9]")))
                sb.Append("Password should be AlphaNumeric" + Environment.NewLine);
            if (!Regex.IsMatch(pass, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                sb.Append("Password should contain special character" + Environment.NewLine);
            return sb.ToString();
        }


        public async Task<bool> CheckEmailExistAsync(string email)
        {
            var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                return true;
            }
            return false;
        }


        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryveryveryveryveryverysceret.....");

            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim("UserID", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Email", user.Email), // Include user email in the token payload
                                                
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }













        public async Task<GoogleAddUserResult> GoogleAddUser([FromBody] User userObj)
        {
            //// Check if the email already exists
            if (await CheckEmailExistAsync(userObj.Email))
            {
                return new GoogleAddUserResult
                {
                    Message = "Email Already Exist",
                    Status = 400
                };
            }

            // Set user role and generate JWT token
            userObj.Role = "User";


            // Add the user to the database
            await _authContext.Users.AddAsync(userObj);
            await _authContext.SaveChangesAsync();

            return new GoogleAddUserResult
            {
                Message = "Student Enrolled!",
                Status = 200,
            };
        }



        public async Task<IActionResult> GoogleAuthenticate(string email)
        {
            if (email == null)
                return new OkObjectResult(new { Message = "Not Input" });

            var user = await _authContext.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
                return new OkObjectResult(new { Message = "User Not Found" });

           

            // Set the Created_at datetime

            _authContext.Entry(user).State = EntityState.Modified;
            await _authContext.SaveChangesAsync();

            return new OkObjectResult(new
            {
                Message = "Login Success!"
            });
        }
    }


    internal class PasswordHasher
    {
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        private static readonly int SaltSize = 16;
        private static readonly int HashSize = 20;
        private static readonly int Iterations = 10000;

        public static string HashPassword(string password)
        {
            byte[] salt;
            rngCsp.GetBytes(salt = new byte[SaltSize]);

            var key = new Rfc2898DeriveBytes(password, salt, Iterations);
            var hash = key.GetBytes(HashSize);

            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            var base64Hash = Convert.ToBase64String(hashBytes);

            return base64Hash;
        }

        public static bool VerifyPassword(string password, string base64Hash)
        {
            var hashBytes = Convert.FromBase64String(base64Hash);

            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            var key = new Rfc2898DeriveBytes(password, salt, Iterations);
            byte[] hash = key.GetBytes(HashSize);

            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                    return false;
            }
            return true;
        }
    }
}
