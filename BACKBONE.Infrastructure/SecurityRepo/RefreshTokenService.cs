using BACKBONE.Application.Interfaces.SecurityInterface;
using BACKBONE.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Infrastructure.SecurityRepo
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ITokenRepository _tokenRepository;

        public RefreshTokenService(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }        

        public string GenerateRefreshTokenAsync()
        {
            try
            {
                var randomBytes = new byte[64];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomBytes);
                    var token = Convert.ToBase64String(randomBytes);
                    Console.WriteLine("Refresh token generated successfully");
                    return token;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating refresh token: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
        {
            try
            {
                var storedToken = await _tokenRepository.GetRefreshTokenAsync(refreshToken);

                if (storedToken != null && storedToken.EXPIRATION_DATE > DateTime.Now)
                {
                    Console.WriteLine("Refresh token validated successfully");
                    return true;
                }
                
                Console.WriteLine("Refresh token validation failed - token not found or expired");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating refresh token: {ex.Message}");
                throw;
            }
        }

        public async Task SaveRefreshTokenAsync(REFRESH_TOKEN refreshToken)
        {
            try
            {
                await _tokenRepository.SaveRefreshTokenAsync(refreshToken);
                Console.WriteLine($"Refresh token saved successfully for user {refreshToken.USERID}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving refresh token for user {refreshToken.USERID}: {ex.Message}");
                throw;
            }
        }

        public async Task SaveDeviceFCMTokenAsync(USERINFO deviceToken)
        {
            try
            {
                await _tokenRepository.SaveFCMDeviceTokenAsync(deviceToken);
                Console.WriteLine($"Refresh token saved successfully for user {deviceToken.USERID}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving refresh token for user {deviceToken.USERID}: {ex.Message}");
                throw;
            }
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            try
            {
                await _tokenRepository.RevokeRefreshTokenAsync(refreshToken);
                Console.WriteLine("Refresh token revoked successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error revoking refresh token: {ex.Message}");
                throw;
            }
        }

        public async Task<REFRESH_TOKEN?> GetRefreshTokenAsync(string refreshToken)
        {
            try
            {
                var token = await _tokenRepository.GetRefreshTokenAsync(refreshToken);
                if (token != null)
                {
                    Console.WriteLine("Refresh token retrieved successfully");
                }
                else
                {
                    Console.WriteLine("Refresh token not found");
                }
                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving refresh token: {ex.Message}");
                throw;
            }
        }

        public bool ValidateRefreshToken(string refreshToken)
        {
            try
            {
                var storedToken = _tokenRepository.GetRefreshToken(refreshToken);
                var isValid = storedToken != null && storedToken.EXPIRATION_DATE > DateTime.Now;
                
                if (isValid)
                {
                    Console.WriteLine("Refresh token validated successfully");
                }
                else
                {
                    Console.WriteLine("Refresh token validation failed - token not found or expired");
                }
                
                return isValid;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating refresh token: {ex.Message}");
                throw;
            }
        }
    }
}