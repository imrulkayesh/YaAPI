using BACKBONE.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Application.Interfaces.SecurityInterface
{
    public interface ITokenRepository
    {
        Task SaveRefreshTokenAsync(REFRESH_TOKEN refreshToken);
        Task<REFRESH_TOKEN?> GetRefreshTokenAsync(string token);
        REFRESH_TOKEN? GetRefreshToken(string token);
        Task RevokeRefreshTokenAsync(string token);
        Task SaveFCMDeviceTokenAsync(USERINFO deviceToken);
    }
}