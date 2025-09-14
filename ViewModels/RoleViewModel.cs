using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkMate.ViewModels
{
    public class RoleViewModel
    {
        [Display(Name = "ID")]
        public int RoleId { get; set; }
        [Required]
        [Display(Name = "Name")]
        [MaxLength(25, ErrorMessage = "Role Name cannot exceed 25 characters.")]
        public string RoleName { get; set; }
        [Required]
        [Display(Name = "Description")]
        [MaxLength(100, ErrorMessage = "Role Description cannot exceed 100 characters.")]
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        public RoleViewModel()
        {

        }
    }

    public class RoleViewList
    {
        List<RoleViewModel> Roles = new List<RoleViewModel>();
        public RoleViewList() { }

    }
}