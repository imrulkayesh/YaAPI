﻿﻿﻿using BACKBONE.Application.Interfaces;
using BACKBONE.Application.Interfaces.SecurityInterface;
using BACKBONE.Core.Dtos;
using BACKBONE.Core.Models;
using BACKBONE.Core.ResponseClasses;
using System.Collections.Generic;
using System.Text.Json;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace BACKBONE.API.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HashSet<string> _allowedPaths;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;

            // Initialize the list of allowed paths
            _allowedPaths = new HashSet<string>
            {
                "/api/v1/auth/login",
                "/api/v1/auth/refresh-token",
                "/api/v1/auth/public-settings",
                "/api/v1/user/withoutauth",
                "/api/v1/user/registration",
                "/api/v1/data/common/user-type",
                "/FeedbackImages",
                "/api/v1/auth/select-company",
                "/api/v1/data/gatepass",
                //"/api/v1/data/approval/approval",
                // Add more paths here as needed
            };
        }

        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine($"Request path: {context.Request.Path.Value}");
            
            // Skip middleware for allowed endpoints
            if (IsPathAllowed(context.Request.Path.Value ?? string.Empty))
            {
                Console.WriteLine($"Path is allowed, skipping middleware");
                await _next(context);
                return;
            }

            var unitOfWork = context.RequestServices.GetRequiredService<IUnitOfWork>();

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                // No token provided in the request
                Console.WriteLine("No token provided in the request");
                await RespondWithJson(context, "No token provided. Unauthorized access.", false);
                return;
            }

            Console.WriteLine($"Validating JWT token: {token.Substring(0, Math.Min(token.Length, 20))}...");
            var isValidToken = unitOfWork.JwtTokenService.ValidateToken(token);

            if (isValidToken == null) // Invalid or expired JWT token
            {
                Console.WriteLine("JWT token validation failed - token is invalid or expired");
                var refreshToken = context.Request.Headers["RefreshToken"].FirstOrDefault();

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    Console.WriteLine("Attempting to use refresh token for re-authentication");
                    var isValidRefreshToken = await unitOfWork.RefreshTokenService.ValidateRefreshTokenAsync(refreshToken);

                    if (isValidRefreshToken)
                    {
                        var result = await unitOfWork.RefreshTokenService.GetRefreshTokenAsync(refreshToken);
                        if (result != null)
                        {
                            // Fetch user details from database using USER_ID from refresh token through UserRepository
                            var user = await unitOfWork.UserRepository.GetJwtUserByIdAsync(result.USERID);

                            if (user != null)
                            {
                                // Generate new JWT token
                                var newJwtToken = unitOfWork.JwtTokenService.GenerateToken(user);
                                var newRefreshToken = unitOfWork.RefreshTokenService.GenerateRefreshTokenAsync();

                                // Revoke old refresh token and save new one
                                await unitOfWork.RefreshTokenService.RevokeRefreshTokenAsync(refreshToken);
                                await unitOfWork.RefreshTokenService.SaveRefreshTokenAsync(new REFRESH_TOKEN
                                {
                                    USERID = result.USERID,
                                    TOKEN = newRefreshToken,
                                    EXPIRATION_DATE = DateTime.Now.AddDays(7)
                                });

                                context.Response.Headers["New-JWT-Token"] = newJwtToken;
                                context.Response.Headers["New-Refresh-Token"] = newRefreshToken;

                                // Continue with the request using new tokens
                                await _next(context);
                                return;
                            }
                            else
                            {
                                await RespondWithJson(context, "User not found or inactive.", false);
                                return;
                            }
                        }
                        else
                        {
                            await RespondWithJson(context, "Failed to retrieve user from refresh token.", false);
                            return;
                        }
                    }
                    else
                    {
                        await RespondWithJson(context, "Invalid refresh token. Please login again.", false);
                        return;
                    }
                }
                else
                {
                    await RespondWithJson(context, "JWT token expired. Please login again.", false);
                    return;
                }
            }

            // Continue to the next middleware if token is valid
            await _next(context);
        }

        private bool IsPathAllowed(string path)
        {
            Console.WriteLine($"Checking if path is allowed: {path}");
            foreach (var allowedPath in _allowedPaths)
            {
                Console.WriteLine($"Comparing with allowed path: {allowedPath}");
                if (path.StartsWith(allowedPath, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Path matches allowed path: {allowedPath}");
                    return true;
                }
            }
            Console.WriteLine("Path is not allowed");
            return false;
        }

        //private async Task RespondWithJson(HttpContext context, string message, bool success)
        //{
        //    var response = new EQResponse<string>
        //    {
        //        Message = message,
        //        Success = success
        //    };

        //    context.Response.ContentType = "application/json";
        //    context.Response.StatusCode = 401;
        //    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        //}
        private async Task RespondWithJson(HttpContext context, string message, bool success)
        {
            var response = new
            {
                message = message,
                success = success,
                data = (object)null
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }

    }
}