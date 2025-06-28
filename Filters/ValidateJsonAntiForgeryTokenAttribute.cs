using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace WorkMate.Filters
{
    public class ValidateJsonAntiForgeryTokenAttribute
        : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                var http = filterContext.HttpContext;

                // Extract antiforgery cookie
                var cookieToken = http.Request.Cookies[AntiForgeryConfig.CookieName]?.Value;

                // Extract antiforgery header
                var headerToken = http.Request.Headers["X-CSRF-Token"];
              
                // Validate both
                AntiForgery.Validate(cookieToken, headerToken);
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }
    }
}