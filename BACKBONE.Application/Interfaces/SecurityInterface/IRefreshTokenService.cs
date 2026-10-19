using BACKBONE.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Application.Interfaces.SecurityInterface
{
    public interface IRefreshTokenService
    {
        string GenerateRefreshTokenAsync();
        Task<bool> ValidateRefreshTokenAsync(string refreshToken);
        Task<REFRESH_TOKEN?> GetRefreshTokenAsync(string refreshToken);  
        Task SaveRefreshTokenAsync(REFRESH_TOKEN refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);

        bool ValidateRefreshToken(string refreshToken);
        Task SaveDeviceFCMTokenAsync(USERINFO deviceToken);
    }
}