using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BACKBONE.Core.Dtos;

namespace BACKBONE.Core.Dtos
{
    public class GATEPASS_PENDING_DTO
    {
        public long Id { get; set; }
        public string? TransactionId { get; set; }
        public string? IssueTypeName { get; set; }
        public string? SenderName { get; set; }
        public string? SendMobile { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverMobile { get; set; }
        public string? Location { get; set; }
        public string? CreatedUser { get; set; }
        public string? SenderDepartment { get; set; }
        public string? ReceiverDepartment { get; set; }
        public string? SenderLocation { get; set; }
        public string? ReceiverLocation { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
