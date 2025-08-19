using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using WorkMate.Models;

namespace WorkMate.ViewModels
{
    /// <summary>
    /// ViewModel for representing Test Case details in the UI layer.
    /// </summary>
    public class TestCaseViewModel
    {
        [Required]
        [DisplayName("Task")]
        public int TaskId { get; set; }

        
        [DisplayName("Task name")]
        public string TaskName { get; set; }

        [DisplayName("Task descriptions")]
        public string TaskDescription { get; set; }

       
        public int TestCaseId { get; set; }


        [DisplayName("Testcase name")]
        [Required, StringLength(200)]
        public string TestCaseName { get; set; }

        [Display(Name = "Test Case Timestamp")]
        [DataType(DataType.DateTime)]
        public DateTime TestCaseTimeStamp { get; set; } = DateTime.Now;


        [DisplayName("Testcase Pre-conditions")]
        public string Preconditions { get; set; }

        [Required]
        [DisplayName("Testcase steps")]
        public string Steps { get; set; }

        [Required]
        [DisplayName("Expected result")]
        public string ExpectedResult { get; set; }

        
        [DisplayName("Actual result")]
        public string ActualResult { get; set; }

        [Required]
        [DisplayName("Type")]
        public TestCaseType Type { get; set; }

        [Required]
        [DisplayName("Priority")]
        public TestCasePriority Priority { get; set; }

        [Required]
        [DisplayName("Status")]
        public TestCaseStatus TestCaseStatus { get; set; }


        [DisplayName("Dev/tester notes")]
        public string Notes { get; set; }

        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime? ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        public IEnumerable<SelectListItem> ddlTasks { get; set; }

        public List<TestCaseModel> AllTestCases { get; set; }

        public TestCaseViewModel()
        {
            // Keep parameterless constructor for model binding.
        }

        public TestCaseViewModel(List<TaskModel> lstTasks, List<TestCaseModel> lstTestcases)
        {
            //Fill the tasks
            FillAllTasks(lstTasks);
            AllTestCases = lstTestcases;
        }
        public TestCaseViewModel(List<TaskModel> lstTasks, List<TestCaseModel> lstTestcases,int taskId)
        {
            FillAllTasks(lstTasks,taskId);
            AllTestCases = lstTestcases;
        }


        /// <summary>
        /// Creates a ViewModel from an entity model.
        /// </summary>
        public TestCaseViewModel(TestCaseModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            TaskId = model.TaskId;
            TaskName = model.Task?.TaskName?.Trim();
            TaskDescription = model.Task?.TaskDescription?.Trim();
            TestCaseId = model.TestCaseId;
            TestCaseName = model.TestCaseName?.Trim();
            TestCaseTimeStamp = model.TestCaseTimeStamp;
            Preconditions = model.Preconditions?.Trim();
            Steps = model.Steps?.Trim();
            ExpectedResult = model.ExpectedResult?.Trim();
            ActualResult = model.ActualResult?.Trim();
            Type = model.Type;
            Priority = model.Priority;
            TestCaseStatus = model.TestCaseStatus;
            Notes = model.Notes?.Trim();
            CreatedBy = model.CreatedBy?.Trim();
            CreatedDate = model.CreatedDate;
            ModifiedDate = model.ModifiedDate;
            ModifiedBy = model.ModifiedBy?.Trim();
            IsDeleted = model.IsDeleted;

        }

        /// <summary>
        /// Converts this ViewModel back into a TestCaseModel for saving to the database.
        /// </summary>
        public TestCaseModel ToModel()
        {
            return new TestCaseModel
            {
                TaskId = this.TaskId,
                TestCaseId = this.TestCaseId,
                TestCaseName = this.TestCaseName?.Trim(),
                TestCaseTimeStamp = this.TestCaseTimeStamp,
                Preconditions = this.Preconditions?.Trim(),
                Steps = this.Steps?.Trim(),
                ExpectedResult = this.ExpectedResult?.Trim(),
                ActualResult = this.ActualResult?.Trim(),
                Type = this.Type,
                Priority = this.Priority,
                TestCaseStatus = this.TestCaseStatus,
                Notes = this.Notes?.Trim(),
                CreatedBy = this.CreatedBy?.Trim(),
                CreatedDate = this.CreatedDate,
                ModifiedDate = this.ModifiedDate,
                ModifiedBy = this.ModifiedBy?.Trim(),
                IsDeleted = this.IsDeleted

            };

            
        }

        private void FillAllTasks(List<TaskModel> lstTasks)
        {
            if (lstTasks == null)
            {
                this.ddlTasks = new List<SelectListItem>();
            }

            this.ddlTasks = lstTasks.Select(t => new SelectListItem
            {
                Value = t.TaskId.ToString(),
                Text = t.TaskName.Trim()
            }).ToList();
        }

        private void FillAllTasks(List<TaskModel> lstTasks, int selectedTaskId)
        {
            if (lstTasks == null)
            {
                this.ddlTasks = new List<SelectListItem>();
                return;
            }

            this.ddlTasks = lstTasks.Select(t => new SelectListItem
            {
                Value = t.TaskId.ToString(),
                Text = t.TaskName.Trim(),
                Selected = t.TaskId == selectedTaskId
            }).ToList();
        }
    }
}
