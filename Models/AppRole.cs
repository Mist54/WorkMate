using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkMate.Models
{
    public class AppRole : IdentityRole<int, UserRole>
    {
        [NotMapped]
        public int RoleId { get; set; }

        [Required, StringLength(50)]
        [NotMapped] // map to IdentityRole.Name
        public string RoleName
        {
            get => Name;
            set => Name = value;
        }

        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
