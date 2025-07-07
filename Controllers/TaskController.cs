using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkMate.ViewModels;
using WorkMate.Models;

namespace WorkMate.Controllers
{
    public class TaskController : Controller
    {
        // GET: Task
        public ActionResult Index(int? selectedId)
        {
            TasksCombinedViewModel tasksCombinedViewModel = new TasksCombinedViewModel();

            try
            {
                
                tasksCombinedViewModel.AllTaskList = getAllTask();
                if(selectedId != null && int.TryParse(selectedId.ToString(),out int id))
                {
                    tasksCombinedViewModel.SelectedTask = getSelectedTask(id);
                }
                else
                {
                    tasksCombinedViewModel.SelectedTask = tasksCombinedViewModel.AllTaskList.FirstOrDefault();
                }
                

            }
            catch(Exception ex)
            {
                throw ex;
            }
            return View(tasksCombinedViewModel);
        }

        private List<TaskModel> getAllTask()
        {
            try
            {
                List<TaskModel> AllTasks = new List<TaskModel>();
                using (var db = new AppDbContext())
                {
                    AllTasks = db.Tasks.OrderBy(t => t.TaskId).ToList();
                    
                }

                return AllTasks;
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }

        private TaskModel getSelectedTask(int selectedId)
        {
            try
            {
                TaskModel selectedTask = new TaskModel();
                if (selectedId > 0)
                {
                    using (var db = new AppDbContext())
                    {
                        selectedTask = db.Tasks.Find(selectedId);
                    }

                }
                return selectedTask;
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TasksCombinedViewModel newTask)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TaskModel createTask = new TaskModel(newTask.SelectedTask.TaskName.Trim(),
                        newTask.SelectedTask.TaskDescription.Trim());
                    using (var db = new AppDbContext())
                    {
                        db.Tasks.Add(createTask);
                        int result = db.SaveChanges();
                        if(result > 0)
                        {
                            TempData["ToastMessage"] = "Task created successfully!";
                            TempData["ToastType"] = "success"; 
                            TempData["ToastTitle"] = "Success!";
                        }
                        else
                        {
                            TempData["ToastMessage"] = "Failed to save task. Please try again.";
                            TempData["ToastType"] = "error"; 
                            TempData["ToastTitle"] = "Error!";
                        }
                        return RedirectToAction("Index", new { selectedTaskId = result });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveChanges(TaskViewModel vmChanges)
        {
            return View();
        }

    }
}