using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Core.Models
{
    public class USERINFO
    {
        public string USERCODE { get; set; }
        public string BU_CODE { get; set; }
        public string USERID { get; set; }
        public string USERNAME { get; set; }
        public string STAFFID { get; set; }
        public string USERTYPEID { get; set; }
        public string PASSWORD { get; set; }

        public string EMAIL { get; set; }
        public string CONTACTNO { get; set; }
        public string ADDRESS { get; set; }
        public int COMPANYID { get; set; }
        public int DEPARTMENTID { get; set; }
        public int DESIGNATIONID { get; set; }
        public int ZONEID { get; set; }


        public int DEPOTID { get; set; }
        public int DEPOACT { get; set; }
        public int ISNEW { get; set; }
        public int ISACTIVE { get; set; }
        public string SERVICEUSER { get; set; }
        public string DEVICEFCMTOKEN { get; set; }
    }
}
