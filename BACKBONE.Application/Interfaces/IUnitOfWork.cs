using BACKBONE.Application.Interfaces.SecurityInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Application.Interfaces
{
    public interface IUnitOfWork
    {
        ITokenRepository TokenRepositor { get; }
        IRefreshTokenService RefreshTokenService { get; }
        IJwtTokenService JwtTokenService { get; }
        IUserRepository UserRepository { get; }
        IHrisAuthService HrisAuthService { get; }
        IGatepassRepository GatepassRepository { get; }
    }
}