using BACKBONE.Application.Interfaces;
using BACKBONE.Core.Dtos;
using BACKBONE.Core.ResponseClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace YaAPI.API.Controllers.v1.Gatepass
{
    [Route("api/v1/data/gatepass")]
    [ApiController]
    public class GatepassController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GatepassController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET api/v1/data/gatepass/waiting-approval/{userId}
        [HttpGet("waiting-approval/{userId}")]
        public IActionResult GetApprovalData(string userId)
        {
            var check = ValidateUser(userId);
            if (check != null) return check;

            EQResponse<GATEPASS_PENDING_DTO> response =
                _unitOfWork.GatepassRepository.GetApprovalDataAsync(userId);

            return ToResult(response);
        }

        // GET api/v1/data/gatepass/count/{userId}
        [HttpGet("count/{userId}")]
        public IActionResult GetApprovalCount(string userId)
        {
            var check = ValidateUser(userId);
            if (check != null) return check;

            EQResponse<GATEPASS_COUNT_DTO> response =
                _unitOfWork.GatepassRepository.GetApprovalCountAsync(userId);

            return ToResult(response);
        }

        // GET api/v1/data/gatepass/items/{userId}/{dispatchId}
        [HttpGet("items/{userId}/{dispatchId:long}")]
        public IActionResult GetItems(string userId, long dispatchId)
        {
            var check = ValidateUser(userId);
            if (check != null) return check;

            EQResponse<GATEPASS_ITEM_DTO> response =
                _unitOfWork.GatepassRepository.GetItemsAsync(userId, dispatchId);

            return ToResult(response);
        }

        // POST api/v1/data/gatepass/approve   body: { "userId": "07092", "dispatchId": 123 }
        [HttpPost("approve")]
        public IActionResult Approve([FromBody] GATEPASS_ACTION_REQUEST_DTO request)
        {
            var check = ValidateAction(request);
            if (check != null) return check;

            EQResponse<GATEPASS_ACTION_RESULT_DTO> response =
                _unitOfWork.GatepassRepository.ApproveAsync(request.UserId, request.DispatchId);

            return ToResult(response);
        }

        // POST api/v1/data/gatepass/reject    body: { "userId": "07092", "dispatchId": 123 }
        [HttpPost("reject")]
        public IActionResult Reject([FromBody] GATEPASS_ACTION_REQUEST_DTO request)
        {
            var check = ValidateAction(request);
            if (check != null) return check;

            EQResponse<GATEPASS_ACTION_RESULT_DTO> response =
                _unitOfWork.GatepassRepository.RejectAsync(request.UserId, request.DispatchId);

            return ToResult(response);
        }

        // ---------- helpers ----------
        private IActionResult ToResult<T>(EQResponse<T> response)
        {
            if (response.Success == true)
                return Ok(response);

            return BadRequest(response);
        }

        private IActionResult? ValidateAction(GATEPASS_ACTION_REQUEST_DTO? request)
        {
            if (request == null || request.DispatchId <= 0)
                return BadRequest("DispatchId is required.");
            return ValidateUser(request.UserId);
        }

        private IActionResult? ValidateUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("UserId is required.");

            if (!_unitOfWork.GatepassRepository.IsValidUser(userId))
                return Unauthorized("Invalid user.");

            return null;
        }
    }
}