using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Core.Dtos
{
    public class U_USER_DTO
    {
        public string USER_ID { get; set; }
        public string NAME { get; set; }
        public string MOBILE { get; set; }
        public string EMAIL { get; set; }
        public string ADDRESS { get; set; }
        public string DIVISION_NAME { get; set; }
        public string DISTRICT_NAME { get; set; }
        public string THANA_NAME { get; set; }
        public string NID { get; set; }
        //public string USER_TYPE_ID { get; set; }
        public string DATEOFBIRTH { get; set; }
        public string USER_IMAGE_PATH { get; set; }
        public string STAFF_ID { get; set; }
        //public string FB_TOKEN { get; set; }
        //public string APPROVE_STATUS { get; set; }
        //public string APPROVE_BY { get; set; }

        //public DateTime? UDT { get; set; }
        //public string CDU { get; set; }
        //public string UDU { get; set; }
        //public string GROUP_ID { get; set; }
        //public int ACT { get; set; }
        public string BU_ID { get; set; }
        public string BASE_NAME { get; set; }
        public string ZONE_NAME { get; set; }
        public string DAYS_AGO { get; set; }
        public DateTime? CDT { get; set; }
    }
}
