using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        [Display(Name = "Duration (hh:mm)")]
        public string Duration
        {
            get
            {
                return EndDate.HasValue
                    ? (EndDate.Value - StartDate).ToString(@"hh\:mm")
                    : "In Progress";
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

        public TimeTrackingListViewModel(List<TaskModel> tasks,List<TimeTrackModel> timeTracks)
        {
            FillAllTasks(tasks);

        }

        private void FillAllTasks(List<TaskModel> lstTasks)
        {
            if (lstTasks == null)
            {
                this.DdlTasks = new List<SelectListItem>();
            }

            this.DdlTasks = lstTasks.Select(t => new SelectListItem
            {
                Value = t.TaskId.ToString(),
                Text = t.TaskName.Trim()
            }).ToList();
        }

        private void FillAllTimeTracks(List<TimeTrackModel> timeTracks)
        {
            foreach(var tracks in timeTracks)
            {
                TimeTrackingViewModel timeTrackingViewModel = new TimeTrackingViewModel
                {
                    Id = tracks.Id,
                    TaskId = tracks.TaskId,
                    TaskName = tracks.Task.TaskName,
                    UserId = tracks.UserId,

                };

                
            }
        }

        /// <summary>
        /// Its important to have a default constructor 
        /// </summary>
        public TimeTrackingListViewModel()
        {

        }
    }


}
