using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkMate.Models
{
    public enum TaskPriority {Low, Medium, High, Critical,Other } 
    public enum TaskStatus { NotStarted, InProgress, Completed, OnHold, Cancelled, QA, Other } 

    public class TaskModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }
        [Required, MaxLength(200)]
        public string TaskName { get; set; }

        //[Required]
        public int AssignedToUserId { get; set; }
        

        //[Required]
        public int AssignedByUserId { get; set; }
        

        /// <summary>
        /// Original assigned date (kept for backward compatibility)
        /// </summary>
        [Required]
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// New semantic start date for the task (optional)
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// New semantic end date for the task (optional)
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public string TaskDescription { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

        public ICollection<TestCaseModel> TestCases { get; set; } = new List<TestCaseModel>();

        //[Required]
        [MaxLength(256)]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        //[Required, MaxLength(256)]
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Entity framework depends on Parameter less constructor 
        /// </summary>
        public TaskModel()
        {
            //Define anything if required else no need to define 
        }

        /// <summary>
        /// parameterized constructor 
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="taskDescription"></param>
        /// <param name="isDeleted"></param>
        public TaskModel(string taskName, string taskDescription, bool isDeleted = false)
        {
            TaskName = taskName;
            TaskDescription = taskDescription;
            CreatedDate = DateTime.Now;
            IsDeleted = isDeleted;
        }
    }
}