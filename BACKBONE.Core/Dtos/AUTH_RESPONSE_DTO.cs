using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BACKBONE.Core.Dtos
{
    public class AUTH_RESPONSE_DTO
    {
        [JsonPropertyName("TOKEN")]
        public string TOKEN { get; set; }

        [JsonPropertyName("REFRESH_TOKEN")]
        public string REFRESH_TOKEN { get; set; }
    }
}