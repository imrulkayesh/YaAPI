
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BACKBONE.Core.Models;
using BACKBONE.Core.ResponseClasses;
using Microsoft.AspNetCore.Http;

namespace BACKBONE.Core.Dtos
{
    public class USER_REGISTRATION_DTO
    {
        public string NAME { get; set; }
        //public string STAFF_ID { get; set; }
        public string MOBILE { get; set; }
        //public string EMAIL { get; set; }
        public string DIVISION_ID { get; set; }
        public string DISTRICT_ID { get; set; }
        public string THANA_ID { get; set; }
        //public string ADDRESS { get; set; }
        public string NID { get; set; }
        public string USER_TYPE_ID { get; set; }
        public string? DATEOFBIRTH { get; set; }
        public string PASSWORD { get; set; }
        public IFormFile? USERIMAGE { get; set; }
    }
}
