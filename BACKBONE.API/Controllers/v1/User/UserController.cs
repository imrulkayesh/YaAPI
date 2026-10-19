using BACKBONE.Application.Interfaces;
using BACKBONE.Core.Dtos;
using BACKBONE.Core.Models;
using BACKBONE.Core.ResponseClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BACKBONE.API.Controllers.v1.User
{
    /// <summary>
    /// Controller for managing user operations
    /// </summary>
    [Route("api/v1/user")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Gets user information without authentication
        /// </summary>
        /// <returns>User information</returns>
        [HttpGet("withoutauth")]
        [AllowAnonymous]
        public IActionResult GetUserWithoutAuth()
        {
            Console.WriteLine("Getting user information without authentication");
            
            var response = new EQResponse<object>
            {
                Message = "User information retrieved successfully",
                Success = true,
                Data = new EQResponseData<object>
                {
                    SingleValue = new
                    {
                        UserId = 1,
                        EmpCode = "EMP001",
                        FullName = "John Doe"
                    }
                }
            };

            Console.WriteLine("User information retrieved successfully");
            return Ok(response);
        }
        //[HttpPost("registration")]
        //[AllowAnonymous]
        //public async Task<IActionResult> CreateUserData(U_USER userData)
        //{
        //    Console.WriteLine("Creating new sample data");
        //    var response = await _unitOfWork.UserRepository.CreateUserDataAsync(userData);
        //    if (response.Success == true)
        //    {
        //        Console.WriteLine($"Successfully created user data with ID: {response.Data?.SingleValue?.ID}");
        //        return Ok(response);
        //    }
        //    Console.WriteLine($"Failed to create user data: {response.Message}");
        //    return BadRequest(response);
        //}
        [HttpPost("registration")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUserData([FromForm] USER_REGISTRATION_DTO userData)
        {
            string? imagePath = null;


            if (userData.USERIMAGE != null && userData.USERIMAGE.Length > 0)
            {
                // Generate unique file name
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(userData.USERIMAGE.FileName)}";

                // Folder to save images
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "UserImages");

                // Create folder if not exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Full path
                var fullPath = Path.Combine(folderPath, fileName);

                // Save file
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await userData.USERIMAGE.CopyToAsync(stream);
                }

                // Save relative path to DB
                imagePath = Path.Combine("UserImages", fileName);
            }

            var user = new U_USER
            {
                USER_ID = userData.MOBILE,
                PASSWORD = userData.PASSWORD,
                NAME = userData.NAME,
                //STAFF_ID = userData.STAFF_ID,
                MOBILE = userData.MOBILE,
                //EMAIL = userData.EMAIL,
                DIVISION_ID = userData.DIVISION_ID,
                DISTRICT_ID = userData.DISTRICT_ID,
                THANA_ID = userData.THANA_ID,
                //ADDRESS = userData.ADDRESS,
                NID = userData.NID,
                USER_TYPE_ID = userData.USER_TYPE_ID,
                USER_IMAGE_PATH = imagePath,
                FB_TOKEN = "663663",
                //APPROVE_BY = "N",
                GROUP_ID = "1",
                BU_ID = "1",
                DATEOFBIRTH = userData.DATEOFBIRTH
            };


            var response = await _unitOfWork.UserRepository.CreateUserDataAsync(user);

            if (response.Success == true)
            {
                Console.WriteLine($"Successfully created user data with ID: {response.Data?.SingleValue?.ID}");
                return Ok(response);
            }
            Console.WriteLine($"Failed to create user data: {response.Message}");
            return BadRequest(response);

        }

    }
}