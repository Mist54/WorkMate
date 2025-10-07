using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WorkMate.Models;
using WorkMate.ViewModels;

namespace WorkMate.Controllers
{
    [Authorize]
    public class TimeTrackingController : Controller
    {
        private string UserId => User.Identity.GetUserId();
        private string UserName => User.Identity.GetUserName();
        // GET: TimeTracking
        public ActionResult Index()
        {
            try
            {
                int currentUserId = Convert.ToInt32(UserId);

                // Get tasks created by current user (previous condition was redundant)
                var allTasks = getAllTask();

                // Get time tracking records for current user
                var allTimeTracks = getAllTimeTracks();

                var model = new TimeTrackingListViewModel(allTasks, allTimeTracks, UserName);

                return View(model);
            }
            catch (Exception ex)
            {
                // Log exception if logger available (e.g., Serilog)
                SetErrorToast($"An error occurred while loading the page: {ex.Message}", "Exception Error");

                // Return empty model to avoid breaking the view
                var emptyModel = new TimeTrackingListViewModel(new List<TaskModel>(), new List<TimeTrackModel>(), UserName);
                return View(emptyModel);
            }
        }


        private List<TaskModel> getAllTask()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    return db.Tasks
                        .Where(t => !t.IsDeleted)
                        .OrderByDescending(t => t.CreatedDate)
                        .ToList();
                }
            }
            catch
            {
                throw;
            }
        }


        private List<TimeTrackModel> getAllTimeTracks()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    return db.TimeTracks
                        .Where(t => !t.IsDeleted)
                        .Include(t => t.Task)      // Eager load Task
                        .Include(t => t.AppUsers)  // Eager load User
                        .OrderByDescending(t => t.CreatedDate)
                        .ToList();
                }
            }
            catch
            {
                
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TimeTrackingViewModel newRecord)
        {
            if (!ModelState.IsValid || newRecord.TaskId <= 0)
            {
                SetErrorToast("Please correct the errors in the form.", "Validation Error");
                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    newRecord.UserId = Convert.ToInt32(UserId);
                    newRecord.UserName = UserName;
                    TimeTrackModel newTimeTrack = new TimeTrackModel(newRecord, newRecord.UserId, newRecord.UserName);

                    //Save the record
                    using(var db = new AppDbContext())
                    {
                        db.TimeTracks.Add(newTimeTrack);
                        await db.SaveChangesAsync();
                    }
                    SetSuccessToast("Time tracking record created successfully.", "Success");
                }
                catch (Exception ex)
                {
                    SetErrorToast("Cannot proceed, server error occured." + ex.Message, "Exception Error");
                }

            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateTimeTrack(int Id, string action, int seconds)
        {
            using (var db = new AppDbContext())
            {
                try
                {
                    var entry = db.TimeTracks.FirstOrDefault(t => t.Id == Id);
                    if (entry == null)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "No time track found."
                        });
                    }

                    DateTime now = DateTime.Now;

                    switch (action)
                    {
                        case "Start":
                            entry.StartDate = now;
                            entry.Status = WorkMate.Models.TaskStatus.InProgress;
                            break;

                        case "Stop":
                            entry.EndDate = now;
                            entry.Status = WorkMate.Models.TaskStatus.InProgress;
                            break;

                        case "Complete":
                            entry.EndDate = now;
                            entry.Status = WorkMate.Models.TaskStatus.Completed;
                            break;

                        case "Delete":
                            db.TimeTracks.Remove(entry);
                            db.SaveChanges();
                            return Json(new
                            {
                                success = true,
                                deleted = true,
                                message = "Time track deleted successfully."
                            });
                    }

                    db.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        status = entry.Status.ToString(),
                        message = $"Time track {action.ToLower()}ed successfully."
                    });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Server error: " + ex.Message
                    });
                }
            }
        }

        #region Toaster Methods

        private void SetSuccessToast(string message, string title)
        {
            TempData["ToastMessage"] = message;
            TempData["ToastTitle"] = title;
            TempData["ToastType"] = "success";
        }

        private void SetErrorToast(string message, string title)
        {
            TempData["ToastMessage"] = message;
            TempData["ToastTitle"] = title;
            TempData["ToastType"] = "error";
        }

        private void SetWarningToast(string message, string title)
        {
            TempData["ToastMessage"] = message;
            TempData["ToastTitle"] = title;
            TempData["ToastType"] = "warning";
        }

        private void SetInfoToast(string message, string title)
        {
            TempData["ToastMessage"] = message;
            TempData["ToastTitle"] = title;
            TempData["ToastType"] = "info";
        }

        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DownloadReport(DateTime StartDate, DateTime EndDate)
        {
            try
            {
                EndDate = EndDate.AddDays(1);
                using (var db = new AppDbContext())
                {
                    var timeTracks = db.TimeTracks
                        .Where(t =>
                            !t.IsDeleted &&
                            t.StartDate >= StartDate &&
                            t.EndDate < EndDate
                        )
                        .Include(t => t.Task)
                        .Include(t => t.AppUsers)
                        .OrderByDescending(t => t.CreatedDate)
                        .ToList();


                    var reportHelper = new Helpers.TimeTrackReportHelper();
                    var excelBytes = reportHelper.GenerateExcelReport(timeTracks);

                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"TimeTrackReport_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }
            catch (Exception ex)
            {
                SetErrorToast($"An error occurred while generating the report: {ex.Message}", "Report Error");
                return RedirectToAction("Index");
            }
        }
    }
}
