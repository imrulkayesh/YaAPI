using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Core.ResponseClasses
{

    public class EQResponse2<T>
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public T? Data { get; set; }
    }

    public class UserDivisionData
    {
        public List<DivisionDto> Divisions { get; set; }
        public List<UserTypeDto> UserTypes { get; set; }
    }

    public class DivisionDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class UserTypeDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
