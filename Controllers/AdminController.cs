using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkMate.Models;
using WorkMate.ViewModels;

namespace WorkMate.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
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
        public ActionResult UserEdit(AppUserViewModel appUserViewModel)
        {
            try
            {
                using(var db = new AppDbContext())
                {
                    AppUsers ExisitingUser = db.Users.Find(appUserViewModel.Id);
                    if (ExisitingUser != null)
                    {
                        ExisitingUser.UserName = appUserViewModel.UserName;
                        ExisitingUser.Email = appUserViewModel.Email;
                        ExisitingUser.Designation = appUserViewModel.Designation;
                        ExisitingUser.ModifiedBy =  HttpContext.User.Identity.Name;
                        ExisitingUser.ModifiedDate = DateTime.Now;
                        db.SaveChanges();
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
            catch (Exception ex)
            {
                throw ex;
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
        private IEnumerable<SelectListItem> PopulateDesignations(string Designation = "")
        {
            string designationsString = ConfigurationManager.AppSettings["Designations"];

            var designationList = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(designationsString))
            {
                string[] designationsArray = designationsString.Split(',');

                foreach (string designation in designationsArray)
                {
                    string trimmedDesignation = designation.Trim();

                    designationList.Add(new SelectListItem
                    {
                        Text = trimmedDesignation,
                        Value = trimmedDesignation,
                        Selected = string.Equals(trimmedDesignation, Designation, StringComparison.OrdinalIgnoreCase)
                    });
                }
            }

            return designationList;
        }
    }
}