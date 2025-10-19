using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkMate.Models;

namespace WorkMate.ViewModels
{
    public class TaskViewModel : IValidatableObject
    {
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Task Name is required.")]
        [MaxLength(25, ErrorMessage = "Task Name cannot exceed 25 characters.")]
        public string TaskName { get; set; }

        //[Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Task Description")]
        public string TaskDescription { get; set; }

        public ICollection<TestCaseModel> TestCases { get; set; } = new List<TestCaseModel>();

        // Assignment fields for Select2 dropdown population and selected values
        public int? AssignedToUserId { get; set; }
        public string AssignedToUserName { get; set; }
        public IEnumerable<SelectListItem> AssignedToList { get; set; } = new List<SelectListItem>();

        public int? AssignedByUserId { get; set; }
        public string AssignedByUserName { get; set; }
        public IEnumerable<SelectListItem> AssignedByList { get; set; } = new List<SelectListItem>();

        // Date fields - will be bound to datepicker in the view
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Deadline/Due-date")]
        public DateTime? EndDate { get; set; }
        
        // Optional: keep existing AssignedDate/DueDate semantics if needed
        public DateTime? AssignedDate { get; set; }
        public DateTime? DueDate { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

        public TaskViewModel() { }

        // Construct from domain model and optional user list for dropdowns
        public TaskViewModel(TaskModel model, IEnumerable<SelectListItem> users = null)
        {
            AssignedToList = users ?? new List<SelectListItem>();
            AssignedByList = users ?? new List<SelectListItem>();

            if (model == null) return;

            TaskId = model.TaskId;
            TaskName = model.TaskName;
            TaskDescription = model.TaskDescription;
            AssignedToUserId = model.AssignedToUserId == 0 ? (int?)null : model.AssignedToUserId;
            AssignedToUserName = GetUserNameById(model.AssignedToUserId);
            AssignedByUserId = model.AssignedByUserId == 0 ? (int?)null : model.AssignedByUserId;
            AssignedByUserName = GetUserNameById(model.AssignedByUserId);
            AssignedDate = model.AssignedDate;
            StartDate = model.StartDate;
            EndDate = model.EndDate;
            Priority = model.Priority;
            // Map status if TaskModel has Status - fall back to NotStarted if missing
            try
            {
                Status = model.GetType().GetProperty("Status") != null
                    ? (TaskStatus)Enum.Parse(typeof(TaskStatus), model.GetType().GetProperty("Status").GetValue(model)?.ToString() ?? "NotStarted")
                    : TaskStatus.NotStarted;
            }
            catch
            {
                Status = TaskStatus.NotStarted;
            }
            TestCases = model.TestCases ?? new List<TestCaseModel>();
        }

        // Server-side validation: ensure StartDate <= EndDate when both present
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate.HasValue && EndDate.HasValue && EndDate.Value.Date < StartDate.Value.Date)
            {
                yield return new ValidationResult("End Date must be the same as or after Start Date.", new[] { nameof(EndDate), nameof(StartDate) });
            }
        }

        private string GetUserNameById(int? userId)
        {
            var user = AssignedToList.FirstOrDefault(u => u.Value == userId.ToString());
            return user != null ? user.Text : string.Empty;
        }
    }

    public class TasksCombinedViewModel 
    {
        // Keep list of domain tasks for listing (used in the table)
        public ICollection<TaskViewModel> AllTaskList { get; set; } = new List<TaskViewModel>();

        // SelectedTask and NewTask are viewmodels (contain select lists etc.)
        public TaskViewModel SelectedTask { get; set; } = new TaskViewModel();
        public TaskViewModel NewTask { get; set; } = new TaskViewModel();//For create 

        public TasksCombinedViewModel() { }

        // Constructor that accepts domain data and a user list; the constructor
        // will create TaskViewModel instances and populate select lists (server-side)
        public TasksCombinedViewModel(List<TaskModel> allTasks, TaskModel selectedTaskModel, IEnumerable<SelectListItem> users)
        {
            foreach (var task in allTasks ?? new List<TaskModel>())
            {
                AllTaskList.Add(new TaskViewModel(task, users));
            }
            SelectedTask = selectedTaskModel != null ? new TaskViewModel(selectedTaskModel, users) : new TaskViewModel(null, users);
            NewTask = new TaskViewModel(null, users);
        }
    }
}