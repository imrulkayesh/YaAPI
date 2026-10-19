using BACKBONE.Application.Interfaces.SecurityInterface;
using BACKBONE.Core.Dtos;
using BACKBONE.Core.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Infrastructure.SecurityRepo
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JWT_SETTINGS _jwtSettings;

        public JwtTokenService(JWT_SETTINGS jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }       

        public string GenerateToken(JWT_USER_DTO user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

                // Generate a numeric ID from the string USER_ID
                int userId = Math.Abs(user.USERID.GetHashCode());

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),  // User ID
                    new Claim(ClaimTypes.Sid, user.USER_MAIL ?? string.Empty),              // Email
                    new Claim(ClaimTypes.Name, user.USER_NAME ?? string.Empty)              // Full Name
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _jwtSettings.Issuer,
                    Audience = _jwtSettings.Audience
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                
                Console.WriteLine($"JWT token generated successfully for user {user.USERID}");
                return tokenString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating JWT token for user {user.USERID}: {ex.Message}");
                throw;
            }
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero // Expire tokens exactly at expiry time
                }, out SecurityToken validatedToken);

                Console.WriteLine("JWT token validated successfully");
                return principal;
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                Console.WriteLine($"Token validation error: {ex.Message}");
                return null; // Invalid token
            }
        }
    }
}