using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace NetCore.Web.Commons
{
    public class TokenInfo
    {
        public string Token { get; set; }

        public IEnumerable<Claim> Claims { get; set; }
    }
}
