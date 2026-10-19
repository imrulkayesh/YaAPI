using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Core.Models
{
    public class COMPLAIN
    {
        public int TICKETID { get; set; }
        public string TICKETCODE { get; set; }
        public string CUSTOMERNAME { get; set; }
        public string CONTACTNO { get; set; }
        public string LOCATION { get; set; }
        public int STATUSID { get; set; }
        public int COMPANYID { get; set; }
        public int TECHNICIANID { get; set; }
        public int SHOWROOM { get; set; }
        public int ISSENDFEEDBACK { get; set; }
        public int PROBLEMTYPEID { get; set; }
        public int ZONEID { get; set; }
    }
}
