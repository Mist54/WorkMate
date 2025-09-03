using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

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
        public virtual AppUsers User { get; set; }


    }
}