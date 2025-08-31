using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace WorkMate.Models
{
    public class AppSignInManager : SignInManager<AppUsers, int>
    {
        public AppSignInManager(AppUserManager userManager, IAuthenticationManager authManager)
           : base(userManager, authManager)
        {
        }
        public static AppSignInManager Create(AppUserManager userManager, IAuthenticationManager authManager)
        {
            return new AppSignInManager(userManager, authManager);
        }


    }
}