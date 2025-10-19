using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WorkMate.ViewModels;
using WorkMate.Models;
using WorkMate.Helpers;

namespace WorkMate.Controllers
{
    public class TaskController : Controller
    {
        // instance for trie-search fuzzy approach
        private static TrieSearch trieSearchInstance = new TrieSearch();
        private static bool isTrieBuilt = false;

        // GET: Task
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Index(int? selectedId, string searchString = "", string searchType = "exact")
        {
            try
            {
                var allTasks = getAllTask(searchString.ToLower().Trim(), searchType.ToLower().Trim());

                TaskModel selectedTaskModel = null;
                if (selectedId != null && int.TryParse(selectedId.ToString(), out int id))
                {
                    selectedTaskModel = getSelectedTask(id);
                }
                else
                {
                    selectedTaskModel = allTasks.FirstOrDefault();
                }

                // Server-side populate user list and pass to viewmodel constructor.
                var users = GetUserSelectList();

                var tasksCombinedViewModel = new TasksCombinedViewModel(allTasks, selectedTaskModel, users);

                return View(tasksCombinedViewModel);
            }
            catch
            {
               
                throw;
            }
        }

        // Helper: server-side user list for Select2 (simple SelectListItem collection)
        private IEnumerable<SelectListItem> GetUserSelectList()
        {
            using (var db = new AppDbContext())
            {
                return db.Users
                         .OrderBy(u => u.UserName)
                         .Select(u => new SelectListItem
                         {
                             Value = u.Id.ToString(),
                             Text = u.UserName
                         }).ToList();
            }
        }

        private List<TaskModel> getAllTask(string searchString = "", string searchType = "exact")
        {
            try
            {
                List<TaskModel> AllTasks = new List<TaskModel>();
                using (var db = new AppDbContext())
                {
                    if (string.IsNullOrWhiteSpace(searchString))
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
            catch (Exception)
            {
                throw;
            }
        }

        private List<TaskModel> search(string searchString, string searchType)
        {
            try
            {
                List<TaskModel> allTaskList = getAllTask(); // default: returns all non-deleted tasks
                List<TaskModel> filteredSearchList = new List<TaskModel>();

                switch (searchType.ToLowerInvariant().Trim())
                {
                    case "exact":
                        {
                            var taskDict = allTaskList.ToDictionary(t => t.TaskName.ToLowerInvariant().Trim());
                            if (taskDict.TryGetValue(searchString.ToLowerInvariant().Trim(), out TaskModel result) && result != null)
                            {
                                filteredSearchList.Add(result);
                            }
                            break;
                        }
                    case "partial":
                        filteredSearchList = allTaskList.Where(t => t.TaskName.ToLowerInvariant().Trim().Contains(searchString.ToLowerInvariant().Trim())).ToList();
                        break;
                    case "prefix":
                        filteredSearchList = allTaskList.Where(t => t.TaskName.ToLowerInvariant().Trim().StartsWith(searchString.ToLowerInvariant().Trim())).ToList();
                        break;
                    case "fuzzy":
                        if (!isTrieBuilt)
                        {
                            trieSearchInstance.BuildIndex(allTaskList);
                            isTrieBuilt = true;
                        }
                        var results = trieSearchInstance.SearchFuzzy(searchString.ToLowerInvariant().Trim(), 2);
                        if (results != null && results.Any())
                        {
                            filteredSearchList.AddRange(results);
                        }

                        if (filteredSearchList.Count == 0)
                        {
                            SimpleFuzzySearch fuzzySearch = new SimpleFuzzySearch();
                            fuzzySearch.SetTasks(allTaskList);

                            var simpleFuzzResults = fuzzySearch.SearchFuzzy(searchString.ToLowerInvariant().Trim(), 2);
                            if (simpleFuzzResults != null && simpleFuzzResults.Any())
                            {
                                filteredSearchList.AddRange(simpleFuzzResults);
                            }
                        }
                        break;
                    default:
                        filteredSearchList = allTaskList;
                        break;
                }

                return filteredSearchList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private TaskModel getSelectedTask(int selectedId)
        {
            try
            {
                TaskModel selectedTask = null;
                if (selectedId > 0)
                {
                    using (var db = new AppDbContext())
                    {
                        selectedTask = db.Tasks.Find(selectedId);
                    }
                }
                return selectedTask;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TasksCombinedViewModel model)
        {
            try
            {
                if (ModelState.IsValid && !string.IsNullOrEmpty(model.NewTask.TaskName?.Trim()) &&
                    !string.IsNullOrEmpty(model.NewTask.TaskDescription?.Trim()))
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

                            // Re-populate select lists before returning view
                            var users = GetUserSelectList();
                            model.NewTask.AssignedToList = users;
                            model.NewTask.AssignedByList = users;
                            var allTasksFallback = getAllTask();
                            var vmFallback = new TasksCombinedViewModel(allTasksFallback, null, users)
                            {
                                NewTask = model.NewTask
                            };
                            return View("Index", vmFallback);
                        }

                        // If the task name is unique, proceed with creation
                        TaskModel createTask = new TaskModel(model.NewTask.TaskName.Trim(),
                            model.NewTask.TaskDescription.Trim())
                        {
                            CreatedBy = User.Identity.Name,
                            AssignedToUserId = model.NewTask.AssignedToUserId ?? 0,
                            AssignedByUserId = model.NewTask.AssignedByUserId ?? 0,
                            StartDate = model.NewTask.StartDate,
                            EndDate = model.NewTask.EndDate,
                            Priority = model.NewTask.Priority,
                            Status = model.NewTask.Status
                        };

                        // If TaskModel defines a Status property as string or enum, attempt to set it
                        var statusProp = createTask.GetType().GetProperty("Status");
                        if (statusProp != null)
                        {
                            try
                            {
                                // If Status is enum type, parse; otherwise set string
                                if (statusProp.PropertyType.IsEnum)
                                {
                                    var enumVal = Enum.Parse(statusProp.PropertyType, model.NewTask.Status.ToString());
                                    statusProp.SetValue(createTask, enumVal);
                                }
                                else
                                {
                                    statusProp.SetValue(createTask, model.NewTask.Status.ToString());
                                }
                            }
                            catch
                            {
                                // ignore mapping errors and continue
                            }
                        }

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
                        return RedirectToAction("Index", new { selectedId = createTask.TaskId });
                    }
                }
            }
            catch (Exception ex)
            {
                // prefer logging here
                TempData["ToastMessage"] = "Unexpected error occurred: " + ex.Message;
                TempData["ToastType"] = "error";
                TempData["ToastTitle"] = "Fatal Error";
            }

            // If we reach here re-populate lists and return index with the posted values
            var allTasks = getAllTask();
            var userList = GetUserSelectList();
            var vm = new TasksCombinedViewModel(allTasks, null, userList);
            // copy posted NewTask values into vm.NewTask so user doesn't lose entered values
            vm.NewTask.TaskName = model.NewTask.TaskName;
            vm.NewTask.TaskDescription = model.NewTask.TaskDescription;
            vm.NewTask.AssignedToUserId = model.NewTask.AssignedToUserId;
            vm.NewTask.AssignedByUserId = model.NewTask.AssignedByUserId;
            vm.NewTask.StartDate = model.NewTask.StartDate;
            vm.NewTask.EndDate = model.NewTask.EndDate;
            vm.NewTask.Priority = model.NewTask.Priority;
            vm.NewTask.Status = model.NewTask.Status;

            return View("Index", vm);
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
                        TaskModel existingTask = db.Tasks.Find(vmChanges.SelectedTask.TaskId);
                        if (existingTask != null)
                        {
                            existingTask.TaskName = vmChanges.SelectedTask.TaskName;
                            existingTask.TaskDescription = vmChanges.SelectedTask.TaskDescription;
                            existingTask.AssignedToUserId = vmChanges.SelectedTask.AssignedToUserId ?? existingTask.AssignedToUserId;
                            existingTask.AssignedByUserId = vmChanges.SelectedTask.AssignedByUserId ?? existingTask.AssignedByUserId;
                            existingTask.StartDate = vmChanges.SelectedTask.StartDate;
                            existingTask.EndDate = vmChanges.SelectedTask.EndDate;
                            existingTask.Priority = vmChanges.SelectedTask.Priority;

                            // If TaskModel has Status property, update it safely
                            var statusProp = existingTask.GetType().GetProperty("Status");
                            if (statusProp != null)
                            {
                                try
                                {
                                    if (statusProp.PropertyType.IsEnum)
                                    {
                                        var enumVal = Enum.Parse(statusProp.PropertyType, vmChanges.SelectedTask.Status.ToString());
                                        statusProp.SetValue(existingTask, enumVal);
                                    }
                                    else
                                    {
                                        statusProp.SetValue(existingTask, vmChanges.SelectedTask.Status.ToString());
                                    }
                                }
                                catch
                                {
                                    // ignore mapping errors
                                }
                            }

                            existingTask.ModifiedBy = User.Identity.Name;
                            existingTask.ModifiedDate = DateTime.Now;
                            db.SaveChanges();

                            TempData["ToastMessage"] = $"Task '{existingTask.TaskName}' updated successfully!";
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

                // Rebuild viewmodel for Index before returning so select lists are populated
                var allTasks = getAllTask();
                var users = GetUserSelectList();
                var vm = new TasksCombinedViewModel(allTasks, getSelectedTask(vmChanges.SelectedTask.TaskId), users);
                return View("Index", vm);
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = "Unexpected error occurred: " + ex.Message;
                TempData["ToastType"] = "error";
                TempData["ToastTitle"] = "Fatal Error";

                var allTasks = getAllTask();
                var users = GetUserSelectList();
                var vm = new TasksCombinedViewModel(allTasks, null, users);
                return View("Index", vm);
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
                    TaskModel existingTask = db.Tasks.Find(decryptId);
                    if (existingTask != null)
                    {
                        existingTask.IsDeleted = true;
                        existingTask.ModifiedDate = DateTime.Now;

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
                        TempData["ToastMessage"] = $"Task '{existingTask.TaskName}' deleted successfully!";
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
                var allTasks = getAllTask();
                var users = GetUserSelectList();
                var selectedTask = allTasks.FirstOrDefault();
                combinedViewModel = new TasksCombinedViewModel(allTasks, selectedTask, users);
            }
            return View("Index", combinedViewModel);
        }
    }
}