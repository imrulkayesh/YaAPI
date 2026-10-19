using BACKBONE.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Application.Interfaces.SecurityInterface
{
    public interface IJwtTokenService 
    {
        string GenerateToken(JWT_USER_DTO user);
        ClaimsPrincipal? ValidateToken(string token);
    }
}