using BACKBONE.Application.Interfaces;
using BACKBONE.Application.Interfaces.SecurityInterface;
using BACKBONE.Core.Dtos;
using BACKBONE.Core.Models;
using BACKBONE.Core.ResponseClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BACKBONE.API.Controllers.v1.Auth
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JWT_SETTINGS _jwtSettings;

        public AuthController(
            IUnitOfWork unitOfWork,
            JWT_SETTINGS jwtSettings)
        {
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtSettings;
        }

        [HttpGet("public-settings")]
        [AllowAnonymous]
        public IActionResult GetPublicJwtSettings()
        {
            Console.WriteLine("Public JWT settings requested");
            return Ok(new {
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                ExpiryMinutes = _jwtSettings.ExpiryMinutes,
                KeyLength = _jwtSettings.Key?.Length
            });
        }

        [HttpGet("settings")]
        public IActionResult GetJwtSettings()
        {
            Console.WriteLine("JWT settings requested");
            return Ok(new {
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                ExpiryMinutes = _jwtSettings.ExpiryMinutes
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LOGIN_DTO loginDto)
        {
            var response = new EQResponse<object>();

            if (loginDto == null || string.IsNullOrEmpty(loginDto.USERID) || string.IsNullOrEmpty(loginDto.PASSWORD))
            {
                response.Success = false;
                response.Message = "Invalid login data";
                response.Data = new EQResponseData<object> { SingleValue = null, ListValue = null, DataTableValue = null };
                return BadRequest(response);
            }

            try
            {
                Console.WriteLine($"Login attempt for user: {loginDto.USERID}");
                var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(loginDto.USERID);

                if (user != null)
                {
                    bool isAuthenticated = (user.PASSWORD == loginDto.PASSWORD);
                    //new code 
                    

                    if (isAuthenticated)
                    {
                        var companies = await _unitOfWork.UserRepository
                       .GetUserCompaniesAsync(user.USERID);

                        if (companies.Count == 0)
                        {
                            response.Success = false;
                            response.Message = "No company assigned.";

                            return Unauthorized(response);
                        }
                        if (companies.Count == 1)
                        {
                            var company = companies.First();

                            var jwtUserDto = new JWT_USER_DTO
                            {
                                USERID = user.USERID,
                                USER_MAIL = user.EMAIL,
                                USER_NAME = user.USERNAME,
                                COMPANYID = company.COMPANYID
                            };

                            var token = _unitOfWork.JwtTokenService.GenerateToken(jwtUserDto);

                            var refreshToken = _unitOfWork.RefreshTokenService.GenerateRefreshTokenAsync();

                            await _unitOfWork.RefreshTokenService.SaveRefreshTokenAsync(
                                new REFRESH_TOKEN
                                {
                                    USERID = user.USERID,
                                    TOKEN = refreshToken,
                                    EXPIRATION_DATE = DateTime.Now.AddDays(7)
                                });

                            response.Success = true;
                            response.Message = "Login successful";

                            response.Data = new EQResponseData<object>
                            {
                                SingleValue = new
                                {
                                    USER_NAME = user.USERNAME,
                                    USER_ID = user.USERID,
                                    COMPANY_ID = company.COMPANYID,
                                    COMPANY_NAME = company.COMPANYNAME,
                                    TOKEN = token,
                                    REFRESH_TOKEN = refreshToken
                                }
                            };

                            return Ok(response);
                        }
                        else
                        {
                            response.Success = true;
                            response.Message = "Multiple companies found. Please select a company.";

                            response.Data = new EQResponseData<object>
                            {
                                SingleValue = new
                                {
                                    USER_ID = user.USERID,
                                    USER_NAME = user.USERNAME,
                                    USER_MAIL = user.EMAIL,
                                    MOBILE = user.CONTACTNO,
                                    COMPANY_SELECTION_REQUIRED = true
                                },

                                ListValue = companies
                                .Select(c => (object)new
                                {
                                    COMPANYID = c.COMPANYID,
                                    COMPANYNAME = c.COMPANYNAME
                                })
                                .ToList(),

                                DataTableValue = null
                            };

                            return Ok(response);
                        }
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Invalid credentials";
                        response.Data = new EQResponseData<object> { SingleValue = null, ListValue = null, DataTableValue = null };
                        return Unauthorized(response);
                    }

                }
                else
                {
                    response.Success = false;
                    response.Message = "User not found";
                    response.Data = new EQResponseData<object> { SingleValue = null, ListValue = null, DataTableValue = null };
                    return Unauthorized(response);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error during authentication: {ex.Message}";
                response.Data = new EQResponseData<object> { SingleValue = null, ListValue = null, DataTableValue = null };
                return StatusCode(500, response);
            }
        }


        [HttpPost("select-company")]
        public async Task<IActionResult> SelectCompany([FromBody] COMPANY_LOGIN_DTO model)
        {
            var response = new EQResponse<object>();

            var user = await _unitOfWork.UserRepository
                        .GetUserByEmailAsync(model.USERID);

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";

                return Unauthorized(response);
            }

            bool validCompany = await _unitOfWork.UserRepository
                .UserBelongsCompany(model.USERID, model.COMPANYID);

            if (!validCompany)
            {
                response.Success = false;
                response.Message = "Invalid company.";

                return Unauthorized(response);
            }

            var jwt = new JWT_USER_DTO
            {
                USERID = user.USERID,
                USER_NAME = user.USERNAME,
                USER_MAIL = user.EMAIL,
                COMPANYID = model.COMPANYID
            };

            var token = _unitOfWork.JwtTokenService.GenerateToken(jwt);

            var refreshToken = _unitOfWork.RefreshTokenService.GenerateRefreshTokenAsync();

            await _unitOfWork.RefreshTokenService.SaveRefreshTokenAsync(
                new REFRESH_TOKEN
                {
                    USERID = user.USERID,
                    TOKEN = refreshToken,
                    EXPIRATION_DATE = DateTime.Now.AddDays(7)
                });

            await _unitOfWork.RefreshTokenService.SaveDeviceFCMTokenAsync(
                new USERINFO
                {
                    USERID = model.USERID,
                    DEVICEFCMTOKEN = model.DEVICEFCMTOKEN
                });

            response.Success = true;
            response.Message = "Login successful.";

            response.Data = new EQResponseData<object>
            {
                SingleValue = new
                {
                    TOKEN = token,
                    REFRESH_TOKEN = refreshToken,
                    COMPANY_ID = model.COMPANYID,
                    USER_ID = user.USERID,
                    USER_NAME = user.USERNAME
                }
            };

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            Console.WriteLine("Refresh token request received");
            
            if (string.IsNullOrEmpty(refreshToken))
            {
                Console.WriteLine("Refresh token is required");
                return BadRequest("Refresh token is required");
            }

            try
            {
                var isValid = await _unitOfWork.RefreshTokenService.ValidateRefreshTokenAsync(refreshToken);
                if (!isValid)
                {
                    Console.WriteLine("Invalid refresh token provided");
                    return Unauthorized("Invalid refresh token");
                }

                var storedToken = await _unitOfWork.RefreshTokenService.GetRefreshTokenAsync(refreshToken);
                if (storedToken == null)
                {
                    Console.WriteLine("Refresh token not found");
                    return Unauthorized("Refresh token not found");
                }

                // Fetch user details from database through UserRepository
                var user = await _unitOfWork.UserRepository.GetJwtUserByIdAsync(storedToken.USERID);

                if (user == null)
                {
                    Console.WriteLine("User not found or inactive for refresh token");
                    return Unauthorized("User not found or inactive");
                }

                // Generate new JWT token
                var newJwtToken = _unitOfWork.JwtTokenService.GenerateToken(user);
                var newRefreshToken = _unitOfWork.RefreshTokenService.GenerateRefreshTokenAsync();

                // Revoke old refresh token and save new one
                await _unitOfWork.RefreshTokenService.RevokeRefreshTokenAsync(refreshToken);
                await _unitOfWork.RefreshTokenService.SaveRefreshTokenAsync(new REFRESH_TOKEN
                {
                    USERID = storedToken.USERID,
                    TOKEN = newRefreshToken,
                    EXPIRATION_DATE = DateTime.Now.AddDays(7)
                });

                Console.WriteLine($"Tokens refreshed successfully for user {user.USERID}");

                // Return new tokens
                return Ok(new AUTH_RESPONSE_DTO 
                { 
                    TOKEN = newJwtToken, 
                    REFRESH_TOKEN = newRefreshToken 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during token refresh: {ex.Message}");
                return StatusCode(500, "Error during token refresh: " + ex.Message);
            }
        }

        //[HttpPost("registration")]
        //[AllowAnonymous]
        //public async Task<IActionResult> CreateUserData(USER_REGISTRATION_DTO userData)
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
    }
}