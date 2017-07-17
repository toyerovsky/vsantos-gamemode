/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

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