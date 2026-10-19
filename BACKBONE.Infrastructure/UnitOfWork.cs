﻿using BACKBONE.Application.Interfaces;
using BACKBONE.Application.Interfaces.SecurityInterface;
using BACKBONE.Infrastructure.SecurityRepo;
using System.Windows.Input;

namespace BACKBONE.Infrastructure
{
    public partial class UnitOfWork : IUnitOfWork
    {
        public ITokenRepository TokenRepositor { get; }    
        public IRefreshTokenService RefreshTokenService { get; }    
        public IJwtTokenService JwtTokenService { get; }    
        public IUserRepository UserRepository { get; }
        public IHrisAuthService HrisAuthService { get; }
        public IGatepassRepository GatepassRepository { get; }

        public UnitOfWork(
            ITokenRepository iTokenRepository, 
            IRefreshTokenService refreshTokenService, 
            IJwtTokenService jwtTokenService, 
            IUserRepository userRepository,
            IHrisAuthService hrisAuthService,
            IGatepassRepository gatepassRepository)
        {
            TokenRepositor = iTokenRepository;
            RefreshTokenService = refreshTokenService;
            JwtTokenService = jwtTokenService;
            UserRepository = userRepository;
            HrisAuthService = hrisAuthService;
            GatepassRepository = gatepassRepository;
        }
    }
}