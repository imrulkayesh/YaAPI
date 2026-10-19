using BACKBONE.Core.Dtos;
using BACKBONE.Core.ResponseClasses;

namespace BACKBONE.Application.Interfaces
{
    public interface IGatepassRepository
    {
        // List results -> Data.ListValue
        EQResponse<GATEPASS_PENDING_DTO> GetApprovalDataAsync(string userId);
        EQResponse<GATEPASS_ITEM_DTO> GetItemsAsync(string userId, long dispatchId);

        // Single results -> Data.SingleValue
        EQResponse<GATEPASS_COUNT_DTO> GetApprovalCountAsync(string userId);
        EQResponse<GATEPASS_ACTION_RESULT_DTO> ApproveAsync(string userId, long dispatchId);
        EQResponse<GATEPASS_ACTION_RESULT_DTO> RejectAsync(string userId, long dispatchId);
        bool IsValidUser(string userId);
    }
}