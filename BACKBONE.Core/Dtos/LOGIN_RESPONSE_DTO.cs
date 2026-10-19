using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Core.Dtos
{
    public class LOGIN_RESPONSE_DTO
    {
        public string USER_NAME { get; set; }
        public string MOBILE { get; set; }
        public string USER_ID { get; set; }
        public string USER_MAIL { get; set; }
        public string TOKEN { get; set; }
        public string REFRESH_TOKEN { get; set; }
        public string COMPANYID { get; set; }
        public string COMPANY_NAME { get; set; }
        public bool HAS_MULTIPLE_COMPANIES { get; set; }
        public List<COMPANY_DTO> COMPANIES { get; set; }
    }
}
