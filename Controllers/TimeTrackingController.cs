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
    public class TimeTrackingController : Controller
    {
       
        // GET: TimeTracking
        public ActionResult Index()
        {
            List<TaskModel> tasks = getAllTask();
            TimeTrackingListViewModel newTimeTracking = new TimeTrackingListViewModel(tasks);
            return View(newTimeTracking);
        }

        private List<TaskModel> getAllTask()
        {
            try
            {
                List<TaskModel> AllTasks = new List<TaskModel>();
                using (var db = new AppDbContext())
                {

                    AllTasks = db.Tasks.Where(t => !t.IsDeleted).OrderByDescending(t => t.CreatedDate).ToList();
                }

                return AllTasks;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}