using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WorkMate.ViewModels;

namespace WorkMate.Models
{
    public enum TaskStatus { Open, InProgress, Completed }

    public class TimeTrackModel: AuditableEntityModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int TaskId { get; set; }
        [Required]
        public int UserId {  get; set; }    
        [Required,MaxLength(500)]
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        [NotMapped]
        public TimeSpan Duration
        {
            get
            {
                return EndDate.HasValue
                    ? EndDate.Value - StartDate
                    : TimeSpan.Zero;
            }
        }

        [Required]
        public TaskStatus Status { get; set; }

        [Required]
        public bool IsManual { get; set; } = false;

        [ForeignKey("TaskId")]
        public virtual TaskModel Task { get; set; }
        [ForeignKey("UserId")]
        public virtual AppUsers AppUsers { get; set; }

       
        public TimeTrackModel()
        {

        }

        public TimeTrackModel(TimeTrackingViewModel model, int loggedInUserId, string loggedInUserName)
        {
            this.TaskId = model.TaskId;
            this.UserId = loggedInUserId;
            this.Description = model.Description;
            this.StartDate = model.StartDate;
            this.EndDate = model.EndDate;
            this.Status = model.Status;
            this.IsManual = model.IsManual;
            this.CreatedBy = loggedInUserName;
            this.UpdatedBy = loggedInUserName;

        }


    }
}