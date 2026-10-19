using BACKBONE.Application.Interfaces;
using BACKBONE.Core.Dtos;
using BACKBONE.Core.ResponseClasses;
using BACKBONE.DB;
using static BACKBONE.Core.ApplicationConnectionString.ApplicationConnectionString;
using Oracle.ManagedDataAccess.Client;

namespace BACKBONE.Infrastructure
{
    public class GatepassRepository : IGatepassRepository
    {
        private const string ProcName = "PRO_API_WAITING_APPROVAL_DATA";

        // ---------- 1. Pending list ----------
        public EQResponse<GATEPASS_PENDING_DTO> GetApprovalDataAsync(string userId)
        {
            var response = new EQResponse<GATEPASS_PENDING_DTO>();
            try
            {
                //var rows = CallProc(new { VMENU = "GETPASS_DETAILS", VUSER = userId });
                var rows = CallProc("GETPASS_DETAILS", userId);

                var list = rows.Select(r => new GATEPASS_PENDING_DTO
                {
                    Id = ToLong(Get(r, "ID")),
                    TransactionId = ToStr(Get(r, "TRANSACTIONID")),
                    IssueTypeName = ToStr(Get(r, "ISSUETYPENAME")),
                    SenderName = ToStr(Get(r, "SENDER_NAME")),
                    SendMobile = ToStr(Get(r, "SENDMOBILE")),
                    ReceiverName = ToStr(Get(r, "RECEIVERNAME")),
                    ReceiverMobile = ToStr(Get(r, "RECEIVERMOBILE")),
                    Location = ToStr(Get(r, "LOCATION")),
                    CreatedUser = ToStr(Get(r, "CREATEDUSER")),
                    SenderDepartment = ToStr(Get(r, "SENDER_DEPARTMENT")),
                    ReceiverDepartment = ToStr(Get(r, "RECEIVERDEPARTMENT")),
                    SenderLocation = ToStr(Get(r, "SENDER_LOCATION")),
                    ReceiverLocation = ToStr(Get(r, "RECEIVER_LOCATION")),
                    CreateDate = ToDate(Get(r, "CREATEDATE"))
                }).ToList();

                response.Data = new EQResponseData<GATEPASS_PENDING_DTO> { ListValue = list };
                response.Success = true;
                response.Message = list.Count > 0
                    ? "Data retrieved successfully."
                    : "No pending gatepass for approval.";
            }
            catch (Exception ex)
            {
                Fail(response, "Error retrieving pending gatepass", ex);
            }
            return response;
        }

        // ---------- 2. Pending count ----------
        public EQResponse<GATEPASS_COUNT_DTO> GetApprovalCountAsync(string userId)
        {
            var response = new EQResponse<GATEPASS_COUNT_DTO>();
            try
            {
                //var rows = CallProc(new { VMENU = "GETPASS_COUNT", VUSER = userId });
                var rows = CallProc("GETPASS_COUNT", userId);
                var row = rows.FirstOrDefault();

                response.Data = new EQResponseData<GATEPASS_COUNT_DTO>
                {
                    SingleValue = new GATEPASS_COUNT_DTO
                    {
                        NotApprove = row == null ? 0 : (int)ToLong(Get(row, "NOTAPPROVE"))
                    }
                };
                response.Success = true;
                response.Message = "Data retrieved successfully.";
            }
            catch (Exception ex)
            {
                Fail(response, "Error retrieving pending count", ex);
            }
            return response;
        }

        // ---------- 3. Items of a gatepass ----------
        public EQResponse<GATEPASS_ITEM_DTO> GetItemsAsync(string userId, long dispatchId)
        {
            var response = new EQResponse<GATEPASS_ITEM_DTO>();
            try
            {
                if (!IsPendingForUser(userId, dispatchId))
                {
                    response.Success = false;
                    response.Message = "This gatepass is not pending for you.";
                    return response;
                }

                //var rows = CallProc(new
                //{
                //    VMENU = "GETPASS_ITEMS",
                //    VUSER = userId,
                //    VPARA1 = dispatchId.ToString()
                //});

                var rows = CallProc("GETPASS_ITEMS", userId, dispatchId.ToString());

                var list = rows.Select(r => new GATEPASS_ITEM_DTO
                {
                    Id = ToLong(Get(r, "ID")),
                    DispatchId = ToLong(Get(r, "DISPATCHID")),
                    ItemName = ToStr(Get(r, "ITEMNAME")),
                    Quantity = ToDecimal(Get(r, "QUANTITY")),
                    Unit = ToStr(Get(r, "UNIT")),
                    Notes = ToStr(Get(r, "NOTES"))
                }).ToList();

                response.Data = new EQResponseData<GATEPASS_ITEM_DTO> { ListValue = list };
                response.Success = true;
                response.Message = "Data retrieved successfully.";
            }
            catch (Exception ex)
            {
                Fail(response, "Error retrieving gatepass items", ex);
            }
            return response;
        }

        // ---------- 4. Approve ----------
        public EQResponse<GATEPASS_ACTION_RESULT_DTO> ApproveAsync(string userId, long dispatchId)
            => ExecuteAction(userId, dispatchId, "GETPASS_APPROVED", "Approved");

        // ---------- 5. Reject ----------
        public EQResponse<GATEPASS_ACTION_RESULT_DTO> RejectAsync(string userId, long dispatchId)
            => ExecuteAction(userId, dispatchId, "GETPASS_REJECT", "Rejected");

        // ================= private helpers =================

        private EQResponse<GATEPASS_ACTION_RESULT_DTO> ExecuteAction(
            string userId, long dispatchId, string vmenu, string actionName)
        {
            var response = new EQResponse<GATEPASS_ACTION_RESULT_DTO>();
            try
            {
                // The procedure does not check that VUSER is the approver of this gatepass,
                // so we verify it here before allowing any update.
                if (!IsPendingForUser(userId, dispatchId))
                {
                    response.Success = false;
                    response.Message = "This gatepass is not pending for your approval.";
                    return response;
                }
                var rows = CallProc(vmenu, userId, dispatchId.ToString());

                // The procedure returns (ID=0, MSG='FAILD') from its EXCEPTION block
                var first = rows.FirstOrDefault();
                if (first != null && string.Equals(ToStr(Get(first, "MSG")), "FAILD",
                        StringComparison.OrdinalIgnoreCase))
                {
                    response.Success = false;
                    response.Message = $"Gatepass {actionName.ToLower()} failed.";
                    return response;
                }

                response.Data = new EQResponseData<GATEPASS_ACTION_RESULT_DTO>
                {
                    SingleValue = new GATEPASS_ACTION_RESULT_DTO
                    {
                        DispatchId = dispatchId,
                        Action = actionName,
                        Rows = rows.Select(ToDictionary).ToList()
                    }
                };
                response.Success = true;
                response.Message = $"Gatepass {actionName.ToLower()} successfully.";
            }
            catch (Exception ex)
            {
                Fail(response, $"Error during gatepass {actionName.ToLower()}", ex);
            }
            return response;
        }

        //private bool IsPendingForUser(string userId, long dispatchId)
        //{
        //    var rows = CallProc(new { VMENU = "GETPASS_DETAILS", VUSER = userId });
        //    return rows.Any(r => ToLong(Get(r, "ID")) == dispatchId);
        //}
        private bool IsPendingForUser(string userId, long dispatchId)
        {
            var rows = CallProc("GETPASS_DETAILS", userId);
            return rows.Any(r => ToLong(Get(r, "ID")) == dispatchId);
        }

        private List<IDictionary<string, object>> CallProc(object parameters)
        {
            var connectionString = GetConnectionString(1);
            IDBHelper db = new OracleDbHelper(connectionString);

            var result = db.ExecuteStoredProcedureWithRefCursor(ProcName, "o_cursor", parameters);

            var list = new List<IDictionary<string, object>>();
            if (result != null)
            {
                foreach (var row in result)
                {
                    // Case-insensitive keys (Oracle returns UPPERCASE column names)
                    var src = (IDictionary<string, object>)row;
                    list.Add(new Dictionary<string, object>(src, StringComparer.OrdinalIgnoreCase));
                }
            }
            return list;
        }

        private static object? Get(IDictionary<string, object> row, string key)
            => row.TryGetValue(key, out var v) && v != DBNull.Value ? v : null;

        private static Dictionary<string, object?> ToDictionary(IDictionary<string, object> row)
            => row.ToDictionary(k => k.Key, k => k.Value == DBNull.Value ? null : (object?)k.Value);

        private static string? ToStr(object? v) => v?.ToString();
        private static long ToLong(object? v) => v == null ? 0 : Convert.ToInt64(v);
        private static decimal? ToDecimal(object? v) => v == null ? null : Convert.ToDecimal(v);
        private static DateTime? ToDate(object? v) => v == null ? null : Convert.ToDateTime(v);

        private static void Fail<T>(EQResponse<T> response, string msg, Exception ex)
        {
            Console.WriteLine($"{msg}: {ex.Message}");
            response.Success = false;
            response.Message = $"{msg}: {ex.Message}";
        }

        private List<IDictionary<string, object>> CallProc(string vmenu, string userId, string? para1 = null)
        {
            var connectionString = GetConnectionString(1);
            IDBHelper db = new OracleDbHelper(connectionString);

            // Order MUST match the procedure: VMENU, VUSER, VPARA1..VPARA8, then o_cursor
            var parameters = new
            {
                VMENU = vmenu,
                VUSER = userId,
                VPARA1 = para1 ?? string.Empty,
                VPARA2 = string.Empty,
                VPARA3 = string.Empty,
                VPARA4 = string.Empty,
                VPARA5 = string.Empty,
                VPARA6 = string.Empty,
                VPARA7 = string.Empty,
                VPARA8 = string.Empty
            };

            var result = db.ExecuteStoredProcedureWithRefCursor(ProcName, "o_cursor", parameters);

            var list = new List<IDictionary<string, object>>();
            if (result != null)
            {
                foreach (var row in result)
                {
                    var src = (IDictionary<string, object>)row;
                    list.Add(new Dictionary<string, object>(src, StringComparer.OrdinalIgnoreCase));
                }
            }
            return list;
        }
        // Checks that the user exists in UC_USERS (USERNAME = userId)
        public bool IsValidUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return false;

            var connectionString = GetConnectionString(1);

            using var conn = new OracleConnection(connectionString);
            conn.Open();

            using var cmd = new OracleCommand(
                "SELECT COUNT(1) FROM UC_USERS WHERE USERNAME = :p_username", conn);
            cmd.BindByName = true;
            cmd.Parameters.Add(new OracleParameter("p_username", OracleDbType.Varchar2)
            {
                Value = userId
            });

            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
    }
}