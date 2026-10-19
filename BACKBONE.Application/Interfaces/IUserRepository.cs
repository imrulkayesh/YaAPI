using BACKBONE.Core.Dtos;
using BACKBONE.Core.Models;
using BACKBONE.Core.ResponseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<USERINFO?> GetUserByEmailAsync(string userid);
        Task<JWT_USER_DTO?> GetJwtUserByIdAsync(string userId);
        Task<EQResponse<U_USER>> CreateUserDataAsync(U_USER UserData);
        EQResponse<U_USER> GetSampleDataByIdAsync(int id);
        Task<List<COMPANY_DTO>> GetUserCompaniesAsync(string userid);
        Task<bool> UserBelongsCompany(string userid, int companyId);
    }
}