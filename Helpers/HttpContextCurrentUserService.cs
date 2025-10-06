using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkMate.Interface;

namespace WorkMate.Helpers
{
    public static class HttpContextCurrentUserService
    {
        public static string GetCurrentUser()
        {
            return HttpContext.Current?.User?.Identity?.Name ?? "System";
        }
    }
}