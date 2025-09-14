using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkMate.Models
{
    /// <summary>
    /// This class is used to handle and add rules.
    /// </summary>
    public class AppUserManager : UserManager<AppUsers, int>
    {
        public AppUserManager(AppUserStore store) : base(store)
        {
            // Password validation rules
            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonLetterOrDigit = true
            };
            this.UserValidator = new UserValidator<AppUsers, int>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // This represent the Rules 
            this.UserLockoutEnabledByDefault = true;
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(15);
            this.MaxFailedAccessAttemptsBeforeLockout = 5;
        }

        public static AppUserManager Create()
        {
            var context = AppDbContext.Create();
            var manager = new AppUserManager(new AppUserStore(context));

            // Configure 2FA with email or SMS if needed 
            //Need to configure Email sending Service manually 
            // manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<AppUsers, int>());

            return manager;
        }
    }
}