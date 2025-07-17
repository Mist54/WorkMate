using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WorkMate.Models;

namespace WorkMate.ViewModels
{
    public class TaskViewModel
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
    }

    public class TasksCombinedViewModel 
    {
        public ICollection<TaskModel> AllTaskList { get; set; } = new List<TaskModel>();

        public TaskModel SelectedTask { get; set; }
        public TaskModel NewTask { get; set; }//For create 
    }
}