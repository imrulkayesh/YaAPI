using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Core.ResponseClasses
{
    public class EQResponseDashboard<T>
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public T? Data { get; set; }
    }

    public class DashboardData
    {
        //public List<RegistrationUserDto> UserData { get; set; }
        //public List<InvoiceDto> InvoiceData { get; set; }
        public string TOTAL_ASSIGN { get; set; }
        public string TOTAL_COMPLETE { get; set; }
        public string TOTAL_CLOSED { get; set; }

    }
    public class RegistrationUserDto
    {
        public string USER_ID { get; set; }
        public string NAME { get; set; }
        public string MOBILE { get; set; }
        public string USER_IMAGE_PATH { get; set; }
        public DateTime? CDT { get; set; }
        public string DAYS_AGO { get; set; }
    }

    public class InvoiceDto
    {
        public string INVOICE_NUMBER { get; set; }
        public string TOTAL_AMOUNT { get; set; }
        public DateTime? CDT { get; set; }
        public string DAYS_AGO { get; set; }
    }
}
