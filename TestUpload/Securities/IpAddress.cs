using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TestUpload.Securities
{
    public  class IpAddress
    {
        private readonly IHttpContextAccessor _accessor;
        public IpAddress(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
        public string GetIp()
        {
            return _accessor.HttpContext.Connection.RemoteIpAddress.ToString();        
        }
    }
}
