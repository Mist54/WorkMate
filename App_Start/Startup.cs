using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using WorkMate.Models;   

[assembly: OwinStartup(typeof(WorkMate.App_Start.Startup))]

namespace WorkMate.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable application sign-in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"), // Redirect here if not logged in
                Provider = new CookieAuthenticationProvider
                {
                    // Security Stamp: re-validate the security stamp when user changes password/roles
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<AppUserManager, AppUsers, int>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentityAsync(manager),
                        getUserIdCallback: (claim) => int.Parse(claim.GetUserId()))
                }
            });
        }
    }
}
