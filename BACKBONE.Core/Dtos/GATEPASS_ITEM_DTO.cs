using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BACKBONE.Core.Dtos;

namespace BACKBONE.Core.Dtos
{
    public class GATEPASS_ITEM_DTO
    {
        public long Id { get; set; }
        public long DispatchId { get; set; }
        public string? ItemName { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
        public string? Notes { get; set; }
    }
}
