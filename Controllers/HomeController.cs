using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkMate.Models;
using WorkMate.ViewModels;

namespace WorkMate.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private string LoggedInUserId => User.Identity.GetUserId();
        private string LoggedInUserName => User.Identity.GetUserName();
        public ActionResult Index()
        {
            try
            {
                List<TaskModel> tasks = new List<TaskModel>();
                List<AppUsers> lstUsers = GetAllUsers();
                if (User.IsInRole("Admin") || User.IsInRole(""))
                {

                    tasks = GetAllTasks();
                }
                else
                {
                    tasks = GetAllTasks(userId: Convert.ToInt32(LoggedInUserId));
                }
                DashboardViewModel vm = new DashboardViewModel(tasks,lstUsers);
                return View(vm);
            }
            catch
            {
                throw;
            }
        }

        private List<TaskModel> GetAllTasks(string searchString = "", int userId = 0)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var query = db.Tasks
                                  .Where(t => !t.IsDeleted)
                                  .OrderByDescending(t => t.CreatedDate)
                                  .AsQueryable();

                    if (!string.IsNullOrWhiteSpace(searchString))
                    {
                        query = query.Where(t => t.TaskName.Contains(searchString) ||
                                                 t.TaskDescription.Contains(searchString));
                    }

                    if (userId > 0)
                    {
                        query = query.Where(t => t.AssignedToUserId == userId || t.AssignedByUserId == userId);
                    }

                    return query.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        private List<AppUsers> GetAllUsers(int userId = 0)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    if (userId == 0)
                    {
                        return db.Users.ToList();
                    }
                    else
                    {
                        return db.Users.ToList().Where(u=>u.Id == userId).ToList(); 
                    }
                   
                }
            }
            catch
            {
                throw;
            }
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
