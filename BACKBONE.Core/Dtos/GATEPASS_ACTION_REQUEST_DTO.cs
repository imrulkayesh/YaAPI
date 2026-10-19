using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BACKBONE.Core.Dtos;

namespace BACKBONE.Core.Dtos
{
    public class GATEPASS_ACTION_REQUEST_DTO
    {
        public string UserId { get; set; } = string.Empty;
        public long DispatchId { get; set; }
    }
}
