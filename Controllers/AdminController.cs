using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WorkMate.Models;
using WorkMate.ViewModels;


namespace WorkMate.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        #region Usersection
        // GET: Admin
        public ActionResult UserIndex()
        {
            List<AppUsers> existingUsers = getAllUsers();
            AppUserViewModalList appUserViewModalList = new AppUserViewModalList(existingUsers);

            return View(appUserViewModalList);
        }

        public ActionResult UserCreate()
        {
            AppUserViewModel appUserViewModal = new AppUserViewModel();
            appUserViewModal.Designations = PopulateDesignations();
            return View(appUserViewModal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserCreate(AppUserViewModel newUser)
        {
            if (!ModelState.IsValid)
            {
                newUser.Designations = PopulateDesignations();
                return View(newUser);
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    var userStore = new UserStore<AppUsers, AppRole, int, UserLogin, UserRole, UserClaim>(db);
                    var userManager = new UserManager<AppUsers, int>(userStore);

                    AppUsers createNewUser = new AppUsers()
                    {
                        UserName = newUser.UserName,
                        Email = newUser.Email,
                        Designation = newUser.Designation,
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.UtcNow,
                        IsActive = false,
                        IsDeleted = false,
                    };
                    var result = await userManager.CreateAsync(createNewUser, newUser.Password);

                    // You should also check the result to handle success or failure
                    if (result.Succeeded)
                    {
                        try
                        {
                            var roleStore = new RoleStore<AppRole, int, UserRole>(db);
                            var roleManager = new RoleManager<AppRole, int>(roleStore);

                            // Assign role to user
                            await userManager.AddToRoleAsync(createNewUser.Id, newUser.Designation);

                            TempData["ToastMessage"] = "User created successfully!";
                            TempData["ToastType"] = "success";
                            TempData["ToastTitle"] = "Success!";
                            return RedirectToAction("UserIndex");

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }


                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                        newUser.Designations = PopulateDesignations();
                        return View(newUser);
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public ActionResult UserEdit(int id)
        {
            AppUsers appUser = getUserById(id);
            AppUserViewModel appUserViewModal = new AppUserViewModel()
            {
                Id = appUser.Id,
                UserName = appUser.UserName,
                Email = appUser.Email,
                Designation = appUser.Designation,
                Designations = PopulateDesignations(appUser.Designation),

            };
            return View(appUserViewModal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserEdit(AppUserViewModel appUserViewModel)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var userStore = new UserStore<AppUsers, AppRole, int, UserLogin, UserRole, UserClaim>(db);
                    var userManager = new UserManager<AppUsers, int>(userStore);

                    AppUsers existingUser = await userManager.FindByIdAsync(appUserViewModel.Id);
                    if (existingUser != null)
                    {
                        // Basic updates
                        existingUser.UserName = appUserViewModel.UserName;
                        existingUser.Email = appUserViewModel.Email;
                        existingUser.Designation = appUserViewModel.Designation;

                        // Created fields: preserve if already set
                        existingUser.CreatedBy = string.IsNullOrWhiteSpace(existingUser.CreatedBy)
                            ? HttpContext.User.Identity.Name
                            : existingUser.CreatedBy;

                        existingUser.CreatedDate = existingUser.CreatedDate == default(DateTime)
                            ? DateTime.UtcNow
                            : existingUser.CreatedDate;

                        // Modified fields
                        existingUser.ModifiedBy = HttpContext.User.Identity.Name;
                        existingUser.ModifiedDate = DateTime.UtcNow;

                        // Update user data
                        await userManager.UpdateAsync(existingUser);

                        // Remove all current roles (UserRole table entries)
                        var currentRoles = await userManager.GetRolesAsync(existingUser.Id);
                        if (currentRoles.Any())
                        {
                            await userManager.RemoveFromRolesAsync(existingUser.Id, currentRoles.ToArray());
                        }

                        // Add new role with audit fields manually
                        var role = db.Roles.SingleOrDefault(r => r.Name == existingUser.Designation.Trim());
                        if (role != null)
                        {
                            db.Set<UserRole>().Add(new UserRole
                            {
                                UserId = existingUser.Id,
                                RoleId = role.Id,
                                CreatedBy = HttpContext.User.Identity.Name,
                                CreatedDate = DateTime.UtcNow,
                                UpdatedBy = HttpContext.User.Identity.Name,
                                UpdatedDate = DateTime.UtcNow,
                                IsDeleted = false
                            });

                            await db.SaveChangesAsync();
                        }

                        TempData["ToastMessage"] = "User details edited successfully!";
                        TempData["ToastType"] = "success";
                        TempData["ToastTitle"] = "Success!";
                        return RedirectToAction("UserIndex");
                    }
                    else
                    {
                        TempData["ToastMessage"] = "Failed to save/edit. Please try again.";
                        TempData["ToastType"] = "error";
                        TempData["ToastTitle"] = "Error!";
                        return View(appUserViewModel);
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
                TempData["ToastMessage"] = "Validation failed. Check the input fields.";
                TempData["ToastType"] = "error";
                TempData["ToastTitle"] = "Validation Error!";

                return View(appUserViewModel);
            }
            catch (Exception ex)
            {
                throw ex ; // keep stack trace
            }
        }

        public ActionResult UserDelete(int Id)
        {
            AppUsers appUser = getUserById(Id);
            AppUserViewModel appUserViewModal = new AppUserViewModel()
            {
                Id = appUser.Id,
                UserName = appUser.UserName,
                Email = appUser.Email,
                Designation = appUser.Designation,
                Designations = PopulateDesignations(appUser.Designation),

            };
            return View(appUserViewModal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserDelete(AppUserViewModel model)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var userStore = new UserStore<AppUsers, AppRole, int, UserLogin, UserRole, UserClaim>(db);
                    var userManager = new UserManager<AppUsers, int>(userStore);

                    // Find the user
                    var user = userManager.FindById(model.Id);

                    if (user == null)
                    {
                        TempData["ErrorMessage"] = "User not found.";
                        return RedirectToAction("UserIndex");
                    }

                    // Delete the user
                    var result = userManager.Delete(user);

                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "User deleted successfully.";
                        return RedirectToAction("UserIndex");
                    }
                    else
                    {
                        // Collect errors if any
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                        return View(model); 
                    }
                }
            }
            catch (Exception ex)
            {
                // You can log exception here
                ModelState.AddModelError("", "An error occurred while deleting the user: " + ex.Message);
                return View(model);
            }
        }


        /// <summary>
        /// Retrives all users
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public List<AppUsers> getAllUsers(string searchQuery = null)
        {
            using (var db = new AppDbContext())
            {
                var query = db.Users.ToList();
                return query;
            }


        }
        /// <summary>
        /// gets perticular user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AppUsers getUserById(int id)
        {
            using (var db = new AppDbContext())
            {
                var query = db.Users.Find(id);
                return query;
            }


        }

        /// <summary>
        /// Populates the Designation dropdown 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SelectListItem> PopulateDesignations(string selectedDesignation = "")
        {
            using (var db = new AppDbContext())
            {
                var roles = db.Roles.ToList();

                return roles.Select(role => new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Name,
                    Selected = string.Equals(role.Name, selectedDesignation, StringComparison.OrdinalIgnoreCase)
                }).ToList();
            }
        }

        #endregion Usersection

        #region RoleSection
        public ActionResult RoleIndex()
        {
            List<AppRole> roles = GetUserRoles();
            AppRoleViewModelList appRoleViewModelList = new AppRoleViewModelList(roles);

            return View(appRoleViewModelList);
        }

        public ActionResult RoleCreate()
        {
            AppRoleViewModel appRoleViewModel = new AppRoleViewModel();
            return View(appRoleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RoleCreate(AppRoleViewModel appRoleViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(appRoleViewModel);
            }

            using (var db = new AppDbContext())
            {
                try
                {
                    var roleStore = new RoleStore<AppRole, int, UserRole>(db);
                    var roleManager = new RoleManager<AppRole, int>(roleStore);

                    // Check if role already exists
                    if (await roleManager.RoleExistsAsync(appRoleViewModel.Name))
                    {
                        ModelState.AddModelError("", "Role already exists.");
                        return View(appRoleViewModel);
                    }

                    var newRole = new AppRole
                    {
                        Name = appRoleViewModel.Name,
                        Description = appRoleViewModel.Description,
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedBy = User.Identity.Name,
                        UpdatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    var result = await roleManager.CreateAsync(newRole);

                    if (result.Succeeded)
                    {
                        TempData["ToastMessage"] = "Role created successfully!";
                        TempData["ToastType"] = "success";
                        TempData["ToastTitle"] = "Success!";
                        return RedirectToAction("RoleIndex");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                        return View(appRoleViewModel);
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                
            }
        }

        public ActionResult RoleEdit(int id)
        {
            var Role = getRoleById(id);
            AppRoleViewModel appRoleViewModel = new AppRoleViewModel(Role);
            return View(appRoleViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RoleEdit(AppRoleViewModel appRoleViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(appRoleViewModel);
            }

            using (var db = new AppDbContext())
            {
                var roleStore = new RoleStore<AppRole, int, UserRole>(db);
                var roleManager = new RoleManager<AppRole, int>(roleStore);

                var role = await roleManager.FindByIdAsync(appRoleViewModel.Id);
                if (role == null)
                {
                    TempData["ToastMessage"] = "Role not found.";
                    TempData["ToastType"] = "error";
                    TempData["ToastTitle"] = "Error!";
                    return RedirectToAction("RoleIndex");
                }

                role.Name = appRoleViewModel.Name;
                role.Description = appRoleViewModel.Description;
                role.UpdatedDate = DateTime.UtcNow;
                role.UpdatedBy = User.Identity.Name;

                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    TempData["ToastMessage"] = "Role updated successfully!";
                    TempData["ToastType"] = "success";
                    TempData["ToastTitle"] = "Success!";
                    return RedirectToAction("RoleIndex");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View(appRoleViewModel);
                }
            }
        }

        public ActionResult RoleDelete(int id)
        {
            var Role = getRoleById(id);
            AppRoleViewModel appRoleViewModel = new AppRoleViewModel(Role);
            return View(appRoleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleDelete(AppRoleViewModel model)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var roleStore = new RoleStore<AppRole, int, UserRole>(db);
                    var roleManager = new RoleManager<AppRole, int>(roleStore);
                    var userManager = new UserManager<AppUsers, int>(new UserStore<AppUsers, AppRole, int, UserLogin, UserRole, UserClaim>(db));

                    var role = roleManager.FindById(model.Id);
                    if (role == null)
                    {
                        TempData["ErrorMessage"] = "Role not found.";
                        return RedirectToAction("RoleIndex");
                    }

                    // Get all users assigned to this role
                    var usersInRole = userManager.Users
                                                 .Where(u => u.Roles.Any(r => r.RoleId == role.Id))
                                                 .ToList();

                    // Remove role from all users
                    foreach (var user in usersInRole)
                    {
                        userManager.RemoveFromRole(user.Id, role.Name);
                    }

                    // Delete the role
                    var result = roleManager.Delete(role);

                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "Role deleted successfully.";
                        return RedirectToAction("RoleIndex");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the role: " + ex.Message);
                return View(model);
            }
        }

        private List<AppRole> GetUserRoles()
        {
            using (var db = new AppDbContext())
            {
                var query = db.Roles.ToList();
                return query;
            }
        }
        private AppRole getRoleById(int id)
        {
            using (var db = new AppDbContext())
            {
                var query = db.Roles.Find(id);
                return query;
            }


        }

        #endregion RoleSection

    }
}