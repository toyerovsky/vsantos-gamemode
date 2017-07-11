using Serverside.Admin.Enums;
using Serverside.Controllers;

namespace Serverside.Admin.Structs
{
    public struct ReportData
    {
        public AccountController Sender { get; set; }
        public AccountController Accused { get; set; }
        public string Content { get; set; }
        public ReportType Type { get; set; }
    }
}