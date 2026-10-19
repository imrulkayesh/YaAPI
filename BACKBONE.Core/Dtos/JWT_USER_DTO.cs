using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Core.Dtos
{
    public class JWT_USER_DTO
    {
        public string USERID { get; set; }
        public string USER_MAIL { get; set; }
        public string USER_NAME { get; set; }
        public string MOBILE { get; set; }
        public string USER_TYPE_ID { get; set; }
        public string NID { get; set; }
        public string USER_IMAGE_PATH { get; set; }
        public DateTime? DATEOFBIRTH { get; set; }
        public int ACT { get; set; }
        public int COMPANYID { get; set; }
    }
}