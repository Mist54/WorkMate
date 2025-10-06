using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkMate.ViewModels;
using WorkMate.Models;
using WorkMate.Helpers;

namespace WorkMate.Controllers
{
    public class TaskController : Controller
    {
        //intance for trisearch fuzzy approch
        private static TrieSearch trieSearchInstance = new TrieSearch();
        private static bool isTrieBuilt = false;

        // GET: Task
        public ActionResult Index(int? selectedId,string searchString = "", string searchType= "exact")
        {
            TasksCombinedViewModel tasksCombinedViewModel = new TasksCombinedViewModel();

            try
            {
                
                tasksCombinedViewModel.AllTaskList = getAllTask(searchString.ToLower().Trim(),searchType.ToLower().Trim());
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

        private List<TaskModel> getAllTask(string searchString="", string searchType="exact")
        {
            try
            {
                List<TaskModel> AllTasks = new List<TaskModel>();
                using (var db = new AppDbContext())
                {
                    if (searchString == string.Empty)
                    {
                        AllTasks = db.Tasks.Where(t => !t.IsDeleted).OrderByDescending(t => t.CreatedDate).ToList();
                    }
                    else
                    {
                        AllTasks = search(searchString, searchType);
                        
                    }
                   

                }

                return AllTasks;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private List<TaskModel> search(string searchString, string searchType)
        {
            try
            {
                List<TaskModel> AllTasKList = new List<TaskModel>();
                List<TaskModel> filterdSearchList = new List<TaskModel>();
                AllTasKList = getAllTask();

                if (searchType.ToLower().Trim() == "exact")
                {
                    Dictionary<string, TaskModel> TaskList = AllTasKList.ToDictionary(t => t.TaskName.ToLowerInvariant().Trim());
                    if (TaskList.TryGetValue(searchString.ToLowerInvariant().Trim(), out TaskModel result) && result != null)
                    {
                        filterdSearchList.Add(result);
                    }

                }
                else if (searchType.ToLower().Trim() == "partial")
                {
                    filterdSearchList = AllTasKList.Where(t => t.TaskName.ToLowerInvariant().Trim().Contains(searchString.ToLowerInvariant().Trim())).ToList();
                }
                else if (searchType.ToLower().Trim() == "prefix")
                {
                    filterdSearchList = AllTasKList.Where(t => t.TaskName.ToLowerInvariant().Trim().StartsWith(searchString.ToLowerInvariant().Trim())).ToList();
                }
                else if (searchType.ToLower().Trim() == "fuzzy")
                {
                    if (!isTrieBuilt)
                    {
                        trieSearchInstance.BuildIndex(AllTasKList);
                        isTrieBuilt = true;
                    }
                    List<TaskModel> results = trieSearchInstance.SearchFuzzy(searchString.ToLowerInvariant().Trim(), 2);
                    if (results != null && results.Any())
                    {
                        filterdSearchList.AddRange(results);
                    }

                    if (filterdSearchList.Count == 0)
                    {
                        SimpleFuzzySearch fuzzySearch = new SimpleFuzzySearch();
                        fuzzySearch.SetTasks(AllTasKList);

                        List<TaskModel> SimpleFuzzSearchresults = fuzzySearch.SearchFuzzy(searchString.ToLowerInvariant().Trim(), 2);
                        if (SimpleFuzzSearchresults != null && SimpleFuzzSearchresults.Any())
                        {
                            filterdSearchList.AddRange(results);
                        }
                    }
                }

                return filterdSearchList;
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
                    using (var db = new AppDbContext())
                    {
                        // Check if a task with the same name already exists
                        bool taskExists = db.Tasks.Any(t => t.TaskName.ToLower().Trim() == model.NewTask.TaskName.ToLower().Trim());

                        if (taskExists)
                        {
                            TempData["ToastMessage"] = "A task with this name already exists. Please use a different name.";
                            TempData["ToastType"] = "warning";
                            TempData["ToastTitle"] = "Duplicate Task Name!";
                            // Return the view to show the error message and let the user correct it
                            return View(model);
                        }

                        // If the task name is unique, proceed with creation
                        TaskModel createTask = new TaskModel(model.NewTask.TaskName.Trim(),
                            model.NewTask.TaskDescription.Trim());
                        createTask.CreatedBy = User.Identity.Name;
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
                        return RedirectToAction("Index", new { selectedTaskId = createTask.TaskId });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(model);
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
                            ExsistingTask.ModifiedBy = User.Identity.Name;
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
        public ActionResult Delete(string taskId)
        {
            TasksCombinedViewModel combinedViewModel = new TasksCombinedViewModel();
           
            try
            {
                int decryptId = Convert.ToInt32(Encryptor.DecryptUrlSafe(taskId));

                using (var db = new AppDbContext())
                {
                    TaskModel ExsistingTask = db.Tasks.Find(taskId);
                    if (ExsistingTask != null)
                    {
                        ExsistingTask.IsDeleted = true;
                        ExsistingTask.ModifiedDate = DateTime.Now;

                        // Soft delete related testcases
                        List<TestCaseModel> relatedTestCases = db.TestCases
                                                 .Where(tc => tc.TaskId == decryptId && !tc.IsDeleted)
                                                 .ToList();
                        if (relatedTestCases.Any())
                        {
                            foreach (var tc in relatedTestCases)
                            {
                                tc.IsDeleted = true;
                                tc.ModifiedDate = DateTime.Now;
                            }
                        }

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