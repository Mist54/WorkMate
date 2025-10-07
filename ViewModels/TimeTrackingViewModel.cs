using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkMate.Models;

namespace WorkMate.ViewModels
{
   
    public class TimeTrackingViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Task")]
        public int TaskId { get; set; }

        [Display(Name = "Task Name")]
        public string TaskName { get; set; }

        // Auto-filled from logged-in user, not editable in UI
        public int UserId { get; set; }

        [Display(Name = "User")]
        public string UserName { get; set; }

        [Required]
        [MaxLength(500)]
        [Display(Name = "Work Description")]
        public string Description { get; set; }

        // Allow both manual entry and auto (default = DateTime.Now)
        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Display(Name = "End Time")]
        public DateTime? EndDate { get; set; } = DateTime.Now.AddHours(1);

        [Display(Name = "Duration")]
        public DateTime Duration
        {
            get
            {
                if (!EndDate.HasValue)
                    return DateTime.MinValue;

                TimeSpan duration = EndDate.Value - StartDate;
                // Return a DateTime representing just duration (base date + hours/minutes)
                return new DateTime().Add(duration);
            }
        }

        [Required]
        [Display(Name = "Status")]
        public TaskStatus Status { get; set; }

        // Flag to differentiate manual vs automatic entry
        [Display(Name = "Manual tracking?")]
        public bool IsManual { get; set; } = true;

        /// <summary>
        /// Its important to have a default constructor 
        /// </summary>
        public TimeTrackingViewModel()
        {


        }
    }

    public class TimeTrackingListViewModel
    {
        public IEnumerable<TimeTrackingViewModel> TaskTimeEntries { get; set; }
        = new List<TimeTrackingViewModel>();

        // This will be used by the "Create" form
        public TimeTrackingViewModel NewRecord { get; set; }
            = new TimeTrackingViewModel();


        public IEnumerable<SelectListItem> DdlTasks { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }

        public TimeTrackingListViewModel()
        {
            // Parameterless constructor for model binding
        }

        public TimeTrackingListViewModel(List<TaskModel> tasks, List<TimeTrackModel> timeTracks, string loggedInUser)
        {
            tasks = FilterTaskListByUser(tasks, loggedInUser);
            var filteredTimeTracks = FilterTimeTracks(timeTracks, loggedInUser);
            TaskTimeEntries = PopulateTimeTracks(filteredTimeTracks);
            PopulateTaskList(tasks);

        }

        private IEnumerable<TimeTrackingViewModel> PopulateTimeTracks(List<TimeTrackModel> timeTracks)
        {
            return timeTracks.Select(t => new TimeTrackingViewModel
            {
                Id = t.Id,
                TaskId = t.TaskId,
                TaskName = t.Task?.TaskName,
                UserId = t.UserId,
                UserName = t.AppUsers?.UserName,
                Description = t.Description,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Status = t.Status,
                IsManual = t.IsManual
            }).ToList();
        }

        private List<TaskModel> FilterTaskListByUser(List<TaskModel> tasks, string loggedInUser)
        {
            return tasks
                .Where(t => t.CreatedBy.Trim().Equals(loggedInUser.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private List<TimeTrackModel> FilterTimeTracks(List<TimeTrackModel> timeTracks, string loggedInUser)
        {
            return timeTracks
                .Where(t => t.CreatedBy.Trim().Equals(loggedInUser.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private void PopulateTaskList(List<TaskModel> lstTasks)
        {
            DdlTasks = lstTasks?.Select(t => new SelectListItem
            {
                Value = t.TaskId.ToString(),
                Text = t.TaskName.Trim()
            }).ToList() ?? new List<SelectListItem>();
        }
    }
}
