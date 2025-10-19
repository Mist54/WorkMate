using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Web.Mvc; 
using WorkMate.Models;

namespace WorkMate.ViewModels
{
    public class AppUserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [Display(Name = "Username")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Designation is required.")]
        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }

        // This property is used to populate the dropdown list for designations.
        public IEnumerable<SelectListItem> Designations { get; set; }

        public AppUserViewModel() {
          

        }
    }

    public class AppUserViewModalList
    {
        public List<AppUserViewModel> UserList = new List<AppUserViewModel>();
        public AppUserViewModalList() { }

        public AppUserViewModalList(List<AppUsers> userList)
        {
            FillUserList(userList);
        }

        private void FillUserList(List<AppUsers> userList)
        {
            foreach (AppUsers user in userList)
            {
                AppUserViewModel ExsitingUser = new AppUserViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Password = user.PasswordHash,
                    Email = user.Email,
                    Designation = user.Designation,
                    IsActive = user.IsActive
                };
                UserList.Add(ExsitingUser);
            }
        }
    }

    public class AppRoleViewModel
    {
        public int Id { get;set; }

        [Required]
        public string Name { get; set; }

        [Required,MaxLength(150)]
        public string Description { get; set; }

        public string CreatedBy { get;set; }

        public DateTime CreatedDate { get; set; }

        public AppRoleViewModel()
        {

        }

        public AppRoleViewModel(AppRole appRole)
        {
            Id = appRole.Id;
            Name = appRole.Name;
            Description = appRole.Description;
            CreatedBy = appRole.CreatedBy;
            CreatedDate = appRole.CreatedDate;

        }
    }

    public  class AppRoleViewModelList
    {
        public List<AppRoleViewModel> RolesList = new List<AppRoleViewModel>();

        public AppRoleViewModelList() { }

        public AppRoleViewModelList(List<AppRole> roles)
        {
            FillRolesList(roles);
        }

        private void FillRolesList(List<AppRole> roles)
        {
            foreach(var role in roles)
            {
                AppRoleViewModel appRoleViewModel = new AppRoleViewModel()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    CreatedBy = role.CreatedBy,
                    CreatedDate = role.CreatedDate

                };
                RolesList.Add(appRoleViewModel);

            }
        }

    }
}