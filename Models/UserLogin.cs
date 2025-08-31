using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkMate.Models
{
    public class UserLogin : IdentityUserLogin<int>
    {
        /* Custom class inhreited with IdentityUserLogin
         * Add properites only if required 
         Stores the Gmail login, OAuth login info in the DB */
    }
}