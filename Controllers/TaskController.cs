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
                if (selectedId != null && int.TryParse(selectedId.ToString(), out int id))
                {
                    tasksCombinedViewModel.SelectedTask = getSelectedTask(id);
                }
                else
                {
                    tasksCombinedViewModel.SelectedTask = tasksCombinedViewModel.AllTaskList.FirstOrDefault();
                }


            }
            catch (Exception ex)
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
                    AllTasks = db.Tasks.Where(t => !t.IsDeleted).OrderByDescending(t => t.CreatedDate).ToList();
                    //AllTasks = db.Tasks.OrderBy(t => t.TaskId).ToList();

                }

                return AllTasks;
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TasksCombinedViewModel model)
        {
            try
            {
                if (ModelState.IsValid && !string.IsNullOrEmpty(model.NewTask.TaskName.Trim()) && 
                    !string.IsNullOrEmpty(model.NewTask.TaskDescription.Trim()))
                {
                    TaskModel createTask = new TaskModel(model.NewTask.TaskName.Trim(),
                        model.NewTask.TaskDescription.Trim());
                    using (var db = new AppDbContext())
                    {
                        db.Tasks.Add(createTask);
                        int result = db.SaveChanges();
                        if (result > 0)
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
        public ActionResult SaveChanges(TasksCombinedViewModel vmChanges)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new AppDbContext())
                    {
                        TaskModel ExsistingTask = db.Tasks.Find(vmChanges.SelectedTask.TaskId);
                        if (ExsistingTask != null)
                        {
                            ExsistingTask.TaskName = vmChanges.SelectedTask.TaskName;
                            ExsistingTask.TaskDescription = vmChanges.SelectedTask.TaskDescription;
                            ExsistingTask.ModifiedDate = DateTime.Now;
                            db.SaveChanges();

                            TempData["ToastMessage"] = $"Task '{ExsistingTask.TaskName}' updated successfully!";
                            TempData["ToastType"] = "success";
                            TempData["ToastTitle"] = "Success!";
                        }
                        else
                        {
                            TempData["ToastMessage"] = "Task not found.";
                            TempData["ToastType"] = "error";
                            TempData["ToastTitle"] = "Error";
                        }
                    }
                }
                else
                {
                    TempData["ToastMessage"] = "Validation failed.";
                    TempData["ToastType"] = "error";
                    TempData["ToastTitle"] = "Validation Error";
                }

                vmChanges.AllTaskList = getAllTask();
                return View("Index", vmChanges);
                //return View(vmChanges);
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = "Unexpected error occurred: " + ex.Message;
                TempData["ToastType"] = "error";
                TempData["ToastTitle"] = "Fatal Error";

                vmChanges.AllTaskList = getAllTask();
                return View("Index", vmChanges);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int taskId)
        {
            TasksCombinedViewModel combinedViewModel = new TasksCombinedViewModel();
           
            try
            {
                using (var db = new AppDbContext())
                {
                    TaskModel ExsistingTask = db.Tasks.Find(taskId);
                    if (ExsistingTask != null)
                    {
                        ExsistingTask.IsDeleted = true;
                        ExsistingTask.ModifiedDate = DateTime.Now;
                        db.SaveChanges();
                        TempData["ToastMessage"] = $"Task '{ExsistingTask.TaskName}' deleted successfully!";
                        TempData["ToastType"] = "success";
                        TempData["ToastTitle"] = "Success!";

                    }
                    else
                    {
                        TempData["ToastMessage"] = "No tasks found to delete.";
                        TempData["ToastType"] = "error";
                        TempData["ToastTitle"] = "Fatal Error";
                    }



                }
               
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = "Unexpected error occurred: " + ex.Message;
                TempData["ToastType"] = "error";
                TempData["ToastTitle"] = "Fatal Error";
               
            }
            finally
            {
                combinedViewModel.AllTaskList = getAllTask();
                TaskModel selectedTask = combinedViewModel.AllTaskList.FirstOrDefault();
                combinedViewModel.SelectedTask = selectedTask != null ? getSelectedTask(selectedTask.TaskId) : null;
               
            }
            return View("Index", combinedViewModel);

        }


    }
}