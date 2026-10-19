
using BACKBONE.Application.Interfaces;
using BACKBONE.Core.Dtos;
using BACKBONE.Core.Models;
using BACKBONE.Core.ResponseClasses;
using BACKBONE.DB;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BACKBONE.Core.ApplicationConnectionString.ApplicationConnectionString;

namespace BACKBONE.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        public async Task<USERINFO?> GetUserByEmailAsync(string userid)
        {
            try
            {
                var connectionString = GetConnectionString(1);
                IDBHelper _db = new OracleDbHelper(connectionString);

                //string sql = @"SELECT U.USERID, U.USERNAME, U.USERTYPEID,UC.CompanyId,U.ZoneId, U.STAFFID,
                //U.PASSWORD,U.EMAIL,U.CONTACTNO 
                //FROM USERINFO U , USERCOMPANY UC
                //WHERE U.USERID = UC.USERID
                //AND U.USERID = :USERID AND U.ISACTIVE = '1'";

                string sql = @"SELECT U.USERID, U.USERNAME, U.USERTYPEID,U.ZoneId, U.STAFFID,
                U.PASSWORD,U.EMAIL,U.CONTACTNO 
                FROM USERINFO U 
                WHERE U.USERID = :USERID AND U.ISACTIVE = '1'";


                return await _db.QuerySingleOrDefaultAsync<USERINFO>(sql, new { USERID = userid });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user by user id: {ex.Message}");
                throw;
            }
        }


        public async Task<JWT_USER_DTO?> GetJwtUserByIdAsync(string userId)
        {
            try
            {
                var connectionString = GetConnectionString(1);
                IDBHelper _db = new OracleDbHelper(connectionString);
                
                string sql = @"SELECT USER_ID, EMAIL as USER_MAIL, NAME as USER_NAME 
                              FROM PRIOJON.U_USERS 
                              WHERE USER_ID=:USER_ID AND ACT=1";
                              
                var user = await _db.QuerySingleOrDefaultAsync<U_USER>(sql, new { USER_ID = userId });
                
                if (user != null)
                {
                    return new JWT_USER_DTO
                    {
                        USERID = user.USER_ID,
                        USER_MAIL = user.EMAIL,
                        USER_NAME = user.NAME
                    };
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving JWT user by ID: {ex.Message}");
                throw;
            }
        }

        public async Task<EQResponse<U_USER>> CreateUserDataAsync(U_USER UserData)
        {
            var response = new EQResponse<U_USER>();

            try
            {
                var connectionString = GetConnectionString(1);
                IDBHelper _db = new OracleDbHelper(connectionString);

                // Prepare parameters including OUTPUT
                var parameters = new DynamicParameters(); // Assuming you're using Dapper-based IDBHelper

                parameters.Add("p_user_id", UserData.MOBILE);
                parameters.Add("p_password", UserData.PASSWORD);
                parameters.Add("p_name", UserData.NAME);
                parameters.Add("p_staff_id", UserData.STAFF_ID);
                parameters.Add("p_mobile", UserData.MOBILE);
                parameters.Add("p_email", UserData.EMAIL);
                parameters.Add("p_division_id", UserData.DIVISION_ID);
                parameters.Add("p_district_id", UserData.DISTRICT_ID);
                //parameters.Add("p_thaha_id", UserData.THANA_ID);
                parameters.Add("p_address", UserData.ADDRESS);
                parameters.Add("p_nid", UserData.NID);
                parameters.Add("p_user_type_id", UserData.USER_TYPE_ID);
                parameters.Add("p_user_image_path", UserData.USER_IMAGE_PATH);
                parameters.Add("p_fb_token", UserData.FB_TOKEN);
                parameters.Add("p_approve_by", UserData.APPROVE_BY);
                parameters.Add("p_group_id", UserData.GROUP_ID);
                parameters.Add("p_bu_id", UserData.BU_ID);
                if (string.IsNullOrWhiteSpace(UserData.DATEOFBIRTH))
                {
                    parameters.Add("p_date_of_birth", DBNull.Value, DbType.Date);
                }
                else
                {
                    parameters.Add("p_date_of_birth", DateTime.Parse(UserData.DATEOFBIRTH), DbType.Date);
                }
                // Output parameter
                parameters.Add("p_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Execute the stored procedure
                await _db.ExecuteAsync("SP_INSERT_USER_DATA", parameters, commandType: CommandType.StoredProcedure);

                int newId = parameters.Get<int>("p_id");


                // Retrieve the created user data (optional)
                //var createdUser = await GetUserDataByIdAsync(newId);


                //if (createdUser.Success)
                //{
                      response.Success = true;
                      response.Message = "User registration successful.";
                //    response.Data = createdUser.Data;
                //}
                //else
                //{
                    //response.Success = false;
                    //response.Message = "User registered, but could not retrieve the data.";
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating sample data: {ex.Message}");

                string userMessage = $"Error creating sample data: {ex.Message}";

                if (ex.Message.Contains("ORA-00001") && ex.Message.Contains("U_USERS_NID"))
                {
                    userMessage = "Same NID already exists.";
                }
                else if (ex.Message.Contains("ORA-00001") && ex.Message.Contains("U_USER_USER_ID"))
                {
                    userMessage = "Same Mobile Number already exists.";
                }
                else if (ex.Message.Contains("ORA-00001"))
                {
                    userMessage = "A record with the same unique field already exists.";
                }

                response.Success = false;
                response.Message = userMessage;
            }

            return response;
        }

        public EQResponse<U_USER> GetUserDataByIdAsync(int id)
        {
            var response = new EQResponse<U_USER>();

            try
            {
                var connectionString = GetConnectionString(1);
                IDBHelper _db = new OracleDbHelper(connectionString);

                var parameters = new { p_id = id };
                var result = _db.ExecuteStoredProcedureWithRefCursor("SP_GET_SAMPLE_DATA_BY_ID", "o_cursor", parameters);

                var UserDataList = new List<U_USER>();
                foreach (var row in result)
                {
                    UserDataList.Add(new U_USER
                    {
                        ID = Convert.ToInt32(row["ID"]),
                        NAME = row["NAME"]?.ToString() ?? string.Empty,
                        MOBILE = row["MOBILE"]?.ToString() ?? string.Empty
                     
                    });
                }

                if (UserDataList.Count > 0)
                {
                    response.Success = true;
                    response.Message = "Sample data retrieved successfully.";
                    response.Data = new EQResponseData<U_USER>
                    {
                        SingleValue = UserDataList[0]
                    };
                }
                else
                {
                    response.Success = false;
                    response.Message = "User data not found.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user data by ID: {ex.Message}");
                response.Success = false;
                response.Message = $"Error retrieving user data: {ex.Message}";
            }

            return response;
        }

        public EQResponse<U_USER> GetSampleDataByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<COMPANY_DTO>> GetUserCompaniesAsync(string userid)
        {
            var connectionString = GetConnectionString(1);
            IDBHelper db = new OracleDbHelper(connectionString);

            string sql = @"
            SELECT
                C.COMPANYID,
                C.COMPANYNAME
            FROM USERCOMPANY UC
            INNER JOIN COMPANY C
                ON UC.COMPANYID=C.COMPANYID
            WHERE UC.USERID=:USERID
            ORDER BY C.COMPANYNAME";

            var result = await db.QueryAsync<COMPANY_DTO>(sql, new { USERID = userid });

            return result.ToList();
        }

        public async Task<bool> UserBelongsCompany(string userid, int companyId)
        {
            var connectionString = GetConnectionString(1);
            IDBHelper db = new OracleDbHelper(connectionString);

            string sql = @"
        SELECT COUNT(*)
        FROM USERCOMPANY
        WHERE USERID = :USERID
          AND COMPANYID = :COMPANYID";

            int count = db.ExecuteScalar(sql, new
            {
                USERID = userid,
                COMPANYID = companyId
            });

            return await Task.FromResult(count > 0);
        }

    }
}