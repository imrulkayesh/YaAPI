using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Core.Models
{
    public class REFRESH_TOKEN
    {
        public int TOKEN_ID { get; set; }
        public string USERID { get; set; }
        public string TOKEN { get; set; }
        public DateTime EXPIRATION_DATE { get; set; }
    }
}