using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkMate.Models
{
    public class UserRole : IdentityUserRole<int>
    {
        [NotMapped]
        public int UserRoleId { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUsers User { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}