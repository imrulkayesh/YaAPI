using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Core.Models
{
    public class TNDR_USER
    {
        public string USER_ID { get; set; }
        public string USER_PASSWD { get; set; }
        public string USER_NAME { get; set; }
        public string DESIGNATION { get; set; }
        public string MOBILE_NO { get; set; }
        public string USER_MAIL { get; set; }
        public string ROLE_ID { get; set; }
        public int IS_ACTIVE { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public string CREATE_USER { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string UPDATE_USER { get; set; }
        public int HRIS_USER { get; set; }
        public string USER_TYPE { get; set; }
    }
}