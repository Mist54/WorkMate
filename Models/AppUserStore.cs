using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkMate.Models
{
    /// <summary>
    /// This class is resposnible for read/write using user and roles
    /// </summary>
    public class AppUserStore: UserStore<AppUsers, AppRole, int, UserLogin, UserRole, UserClaim>
    {
        public AppUserStore(AppDbContext context) : base(context)
        {
            /*It handles the CRUD (Create, Read, Update, Delete) operations for users, 
             * roles, logins, and claims. Instead of writing custom database queries, 
             * you can use the methods provided by this class to, for example, 
             * create a new user or add a user to a role.
             */
        }
    }
}