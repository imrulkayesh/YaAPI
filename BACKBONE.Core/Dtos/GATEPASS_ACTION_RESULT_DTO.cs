using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BACKBONE.Core.Dtos;

namespace BACKBONE.Core.Dtos
{
    public class GATEPASS_ACTION_RESULT_DTO
    {
        public long DispatchId { get; set; }
        public string Action { get; set; } = string.Empty;
        // Rows returned by the procedure (dispatch + items + employee info),
        // useful for sending notifications/emails after approve/reject.
        public List<Dictionary<string, object?>> Rows { get; set; } = new();
    }
}
