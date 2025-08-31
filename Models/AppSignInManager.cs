using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace WorkMate.Models
{
    public class AppSignInManager : SignInManager<AppUsers, int>
    {
        public AppSignInManager(AppUserManager userManager, IAuthenticationManager authManager)
            : base(userManager, authManager)
        {
        }

        public static AppSignInManager Create(
            IdentityFactoryOptions<AppSignInManager> options,
            IOwinContext context)
        {
            return new AppSignInManager(
                context.GetUserManager<AppUserManager>(),
                context.Authentication);
        }
    }
}
