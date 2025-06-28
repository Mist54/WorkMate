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

    }
}