using System;
using System.ComponentModel.DataAnnotations;

namespace WorkMate.Models
{
    public abstract class AuditableEntity
    {
        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; } = false; 

    }
}