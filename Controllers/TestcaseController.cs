using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using WorkMate.Models;
using WorkMate.Services;
using WorkMate.ViewModels;
using System.Data.Entity;
using WorkMate.Helpers;

namespace WorkMate.Controllers
{
    public class TestcaseController : Controller
    {
        // GET: Testcase
        public ActionResult Index()
        {
            try
            {
                List<TaskModel> tasks = new List<TaskModel>();
                List<TestCaseModel> testCases = new List<TestCaseModel>();
                tasks = getAllTask();
                testCases = getAllTestcases();
                TestIndexViewModel testIndexViewModal = new TestIndexViewModel(tasks, testCases);
                return View(testIndexViewModal);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Used to download testcases
        /// </summary>
        /// <param name="SelectedTaskId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DownloadTestcase(int? SelectedTaskId)
        {
            try
            {
                if (!SelectedTaskId.HasValue || SelectedTaskId.Value <= 0)
                {
                    SetErrorToast("Select the task correctly", "Cannot download");
                    return RedirectToAction("Index"); // or return a View, depending on flow
                }

                List<TestCaseModel> testcases = getAllTestcases(SelectedTaskId.Value);

                var excelService = new ExcelExportService();
                var columnMappings = new Dictionary<string, Func<TestCaseModel, object>>
                {
                    { "Testcase Name", x => x.TestCaseName },
                    { "Preconditions", x => x.Preconditions },
                    { "Steps", x => x.Steps },
                    { "Expected Result", x => x.ExpectedResult },
                    { "Actual Result", x => "" },
                    { "Notes", x => x.Notes },
                    { "Status", x => x.TestCaseStatus.ToString() }
                };

                string sheetName = "Testcases";
                if (testcases.Any() && testcases.First().Task != null)
                {
                    sheetName = testcases.First().Task.TaskName;
                }

                var fileBytes = excelService.ExportToExcel(testcases, columnMappings, sheetName);
                var fileName = $"{sheetName}_{DateTime.Now:yyyyMMdd}.xlsx";

                return File(fileBytes,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Create(int id = 0, string testcaseName = "")
        {
            try
            {
                List<TaskModel> tasks = getAllTask();
                List<TestCaseModel> testcases = getAllTestcases(id);
                TestCaseViewModel model = new TestCaseViewModel(tasks, testcases);
                if (!string.IsNullOrEmpty(testcaseName))
                {
                    //model.TestCaseName = GetNextString(testcaseName);
                    model.TaskId = id;
                    ModelState.Remove("TaskId");
                    //ModelState.Remove("TestCaseName");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                SetErrorToast("Error: " + ex.Message, "Error!");
                return RedirectToAction("Index");
            }

        }

        /// <summary>
        /// This Action is used for Partical view of the table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreatePartial(int id = 0, string testcaseName = "")
        {
            try
            {
                List<TestCaseModel> testcases = getAllTestcases(id);
                List<TaskModel> tasks = new List<TaskModel>();
                TestCaseViewModel model = new TestCaseViewModel(tasks, testcases);
                return PartialView("~/Views/Shared/Testcase/_TestcaseTable.cshtml", model); //If the partial view is in saparate folder need to provide full path on return
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CreatePartial: {ex.Message}");
                throw new Exception(ex.Message);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TestCaseViewModel model)
        {
            // Check if the model is valid based on data annotations
            if (!ModelState.IsValid)
            {
                LogValidationErrors();
                return PopulateTestCaseModel(model);
            }

            // Model is valid, proceed with saving to the database
            try
            {
                string newTestcaseName = SaveNewTestcase(model);

                return RedirectToAction("Create", new { id = model.TaskId, testcaseName = newTestcaseName });
            }
            catch (Exception ex)
            {
                if (ex is System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Entity: {validationErrors.Entry.Entity.GetType().Name}, " +
                                                                $"Property: {validationError.PropertyName}, " +
                                                                $"Error: {validationError.ErrorMessage}");
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"General Exception: {ex.Message}");
                }

                throw;
            }
        }

        /// <summary>
        /// Saves a new test case to the database and handles the result.
        /// </summary>
        /// <param name="model">The TestCaseViewModel to save.</param>
        /// <returns>The generated name for the next test case, or string.Empty.</returns>
        private string SaveNewTestcase(TestCaseViewModel model)
        {
            using (var db = new AppDbContext())
            {
                TestCaseModel newTestcase = new TestCaseModel(model);
                db.TestCases.Add(newTestcase);
                int result = db.SaveChanges();

                if (result > 0)
                {
                    SetSuccessToast("Testcase created successfully!", "Success!");
                    if (!string.IsNullOrEmpty(model.TestCaseName) && IsTestCaseNameValid(model.TestCaseName))
                    {
                        return GetNextString(model.TestCaseName);
                    }
                }
                else
                {

                    SetErrorToast("Failed to save testcase. Please try again.", "Error!");
                }
                return string.Empty;
            }
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    TestCaseModel testCaseToDelete = db.TestCases.Find(id);

                    if (testCaseToDelete == null)
                    {
                        SetErrorToast("The test case was not found.", "Error!");
                        return View(ConvertToTestcaseViewModelForDelete(testCaseToDelete));
                    }

                    //Remove the entity from the context
                    db.TestCases.Remove(testCaseToDelete);
                    db.SaveChanges();
                    SetWarningToast("Test case deleted successfully!", "Warning!");
                    return RedirectToAction("Create", new { id = testCaseToDelete.TaskId, testcaseName = testCaseToDelete.TestCaseName });

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            try
            {
                int decryptedId = Convert.ToInt32(Encryptor.DecryptUrlSafe(id));
                if (decryptedId <= 0)
                {
                    return RedirectToAction("Index");
                }

                using (var db = new AppDbContext())
                {
                    TestCaseModel testcaseToEdit = db.TestCases
                                 .Include(tc => tc.Task)
                                 .FirstOrDefault(tc => tc.TestCaseId == decryptedId);

                    if (testcaseToEdit == null)
                    {
                        SetErrorToast("The test case was not found.", "Error!");
                        return RedirectToAction("Index"); // safer than redirecting with null values
                    }


                    // Convert to ViewModel
                    TestCaseViewModel editTestCaseVm = ConvertToTestcaseViewModelForEdit(testcaseToEdit);

                    return View(editTestCaseVm);
                }
            }
            catch (Exception ex)
            {
                // log exception if needed
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TestCaseViewModel model)
        {
            try
            {
                model.TaskId = Convert.ToInt32(Encryptor.DecryptUrlSafe(model.EncryptedTaskId.ToString()));
                model.TestCaseId = Convert.ToInt32(Encryptor.DecryptUrlSafe(model.EncryptedTestcaseId.ToString()));
                if (!ModelState.IsValid)
                {
                    // reload Task info in case validation fails
                    using (var db = new AppDbContext())
                    {
                        var testCaseWithTask = db.TestCases
                            .Include("Task")
                            .FirstOrDefault(tc => tc.TestCaseId == model.TestCaseId);

                        if (testCaseWithTask != null)
                        {
                            model.TaskName = testCaseWithTask.Task?.TaskName;
                            model.TaskDescription = testCaseWithTask.Task?.TaskDescription;
                        }
                    }

                    return View(model);
                }

                using (var db = new AppDbContext())
                {
                    var testCase = db.TestCases.FirstOrDefault(tc => tc.TestCaseId == model.TestCaseId);

                    if (testCase == null)
                    {
                        SetErrorToast("Test case not found.", "Error!");
                        return RedirectToAction("Index");
                    }

                    // Update only editable fields
                    testCase.TestCaseName = model.TestCaseName;
                    testCase.Preconditions = model.Preconditions;
                    testCase.Steps = model.Steps;
                    testCase.ExpectedResult = model.ExpectedResult;
                    testCase.TestCaseStatus = model.TestCaseStatus;
                    testCase.Priority = model.Priority;
                    testCase.Type = model.Type;
                    testCase.Notes = model.Notes;
                    testCase.ModifiedDate = DateTime.Now;

                    int result = db.SaveChanges();
                    if (result > 0)
                    {
                        SetSuccessToast("Test case updated successfully!", "Success");

                    }

                    return RedirectToAction("Create", new { id = testCase.TaskId, testcaseName = testCase.TestCaseName });
                }


            }
            catch (Exception ex)
            {
                // log if needed
                throw ex;
            }
        }



        private TestCaseViewModel ConvertToTestcaseViewModelForDelete(TestCaseModel testCaseModel)
        {
            try
            {
                List<TaskModel> tasks = getAllTask();
                List<TestCaseModel> testcases = getAllTestcases(testCaseModel.TaskId);
                TestCaseViewModel returnModel = new TestCaseViewModel(tasks, testcases);
                returnModel.TaskId = testCaseModel.TaskId;
                return returnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private TestCaseViewModel ConvertToTestcaseViewModelForEdit(TestCaseModel testCaseModel)
        {
            if (testCaseModel == null) return null;
            List<TestCaseModel> testcases = getAllTestcases(testCaseModel.TaskId);

            return new TestCaseViewModel(testCaseModel, testcases);

        }


        /// <summary>
        /// Used to fill and set the TestcaseViewModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ActionResult PopulateTestCaseModel(TestCaseViewModel model)
        {
            List<TaskModel> tasks = getAllTask();
            List<TestCaseModel> testcases = getAllTestcases(model.TaskId);
            TestCaseViewModel returnModel = new TestCaseViewModel(tasks, testcases);
            return View(returnModel);
        }

        /// <summary>
        /// Logs all validation errors from ModelState.
        /// </summary>
        private void LogValidationErrors()
        {
            foreach (var modelStateEntry in ModelState.Values)
            {
                foreach (var error in modelStateEntry.Errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
            }
        }

        /// <summary>
        /// Sets a success toast notification message.
        /// </summary>
        /// <param name="message">The message to display in the toast.</param>
        /// <param name="title">The title of the toast.</param>
        public void SetSuccessToast(string message, string title)
        {
            TempData["ToastMessage"] = message;
            TempData["ToastTitle"] = title;
            TempData["ToastType"] = "success";
        }

        /// <summary>
        /// Sets an error toast notification message.
        /// </summary>
        /// <param name="message">The message to display in the toast.</param>
        /// <param name="title">The title of the toast.</param>
        public void SetErrorToast(string message, string title)
        {
            TempData["ToastMessage"] = message;
            TempData["ToastTitle"] = title;
            TempData["ToastType"] = "error";
        }

        /// <summary>
        /// Sets a warning toast notification message.
        /// </summary>
        /// <param name="message">The message to display in the toast.</param>
        /// <param name="title">The title of the toast.</param>
        public void SetWarningToast(string message, string title)
        {
            TempData["ToastMessage"] = message;
            TempData["ToastTitle"] = title;
            TempData["ToastType"] = "warning";
        }

        /// <summary>
        /// Sets an informational toast notification message.
        /// </summary>
        /// <param name="message">The message to display in the toast.</param>
        /// <param name="title">The title of the toast.</param>
        public void SetInfoToast(string message, string title)
        {
            TempData["ToastMessage"] = message;
            TempData["ToastTitle"] = title;
            TempData["ToastType"] = "info";
        }


        /// <summary>
        /// Gets or returns the next string with prefix of _001, _002 etc
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetNextString(string input)
        {

            var match = Regex.Match(input, @"^(.*?)(?:_(\d{3}))?$");

            if (!match.Success)
                return input + "_001"; // fallback, shouldn't happen

            string baseText = match.Groups[1].Value;
            string numberPart = match.Groups[2].Value;

            if (string.IsNullOrEmpty(numberPart))
            {

                return $"{baseText}_001";
            }
            else
            {

                int number = int.Parse(numberPart);
                number++;
                return $"{baseText}_{number:D3}";
            }
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

        private List<TestCaseModel> getAllTestcases()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var lstTestcases = db.TestCases
                     .Include(t => t.Task)   // eager load navigation
                     .Where(t => !t.IsDeleted)
                     .OrderByDescending(t => t.CreatedDate)
                     .ToList();


                    return lstTestcases;
                }
            }
            catch
            {
                throw;
            }
        }


        private List<TestCaseModel> getAllTestcases(int taskId)
        {
            try
            {
                if (taskId == 0)
                    return new List<TestCaseModel>();

                using (var db = new AppDbContext())
                {
                    // Include Task via navigation property
                    var lstTestcases = db.TestCases
                     .Include(t => t.Task)   // eager load navigation
                     .Where(t => !t.IsDeleted && t.TaskId == taskId)
                     .OrderByDescending(t => t.CreatedDate)
                     .ToList();


                    return lstTestcases;
                }
            }
            catch
            {
                throw;
            }
        }




        /// <summary>
        /// Check testcasename if valid or not 
        /// </summary>
        /// <param name="testCaseName"></param>
        /// <returns></returns>
        public bool IsTestCaseNameValid(string testCaseName)
        {
            if (string.IsNullOrEmpty(testCaseName))
            {
                return false;
            }

            return Regex.IsMatch(testCaseName, @"^(.*?)(?:_(\d{3}))?$");
        }




    }
}