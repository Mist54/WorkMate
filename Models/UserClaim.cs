using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkMate.Models
{
    public class UserClaim: IdentityUserClaim<int>
    {
        /*Extended class for Claims like 2FA etc*/
    }
}