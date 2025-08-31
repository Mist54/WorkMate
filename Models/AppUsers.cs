using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;


namespace WorkMate.Models
{
    public class AppUsers: IdentityUser<int, UserLogin, UserRole, UserClaim>
    {
        [NotMapped]
        public int UserId  { get; set; }

        [NotMapped]
        public string Username { get; set; }

        [StringLength(256)]
        public string PasswordSalt { get; set; }  

        public bool IsActive { get; set; } = true;

        [StringLength(50)]
        public string Designation { get; set; }    

        public DateTime? LastLoginAt { get; set; }
        [StringLength(45)]
        public string LastLoginIP { get; set; }
        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; }

       
        [MaxLength(256)]
        public string CreatedBy { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUsers, int> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            return userIdentity;
        }
    }
}
