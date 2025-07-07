using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkMate.Models
{
    public class TaskModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }
        [Required, MaxLength(200)]
        public string TaskName { get; set; }

        public string TaskDescription { get; set; }
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
        /// Entity framework depends on Paramerterless constructor 
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