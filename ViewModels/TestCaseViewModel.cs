using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WorkMate.Models;

namespace WorkMate.ViewModels
{
    public class TestCaseViewModel
    {
        [Required]
        [Display(Name = "Task Slno")]
        public int TaskId { get; set; }

        [Required, MaxLength(200)]
        [Display(Name = "Task name")]
        public string TaskName { get; set; }


        public string TaskDescription { get; set; }

        public int TestCaseId { get; set; }

        [Required, MaxLength(200)]
        [Display(Name = "Test case")]
        public string TestCaseName { get; set; }

        [Display(Name = "Pre condition")]
        [DataType(DataType.MultilineText)]
        public string Preconditions { get; set; }

        [Required]
        [Display(Name = "Testcase approch steps")]
        [DataType(DataType.MultilineText)]
        public string Steps { get; set; }

        [Required]
        [Display(Name = "Expected result")]
        [DataType(DataType.MultilineText)]
        public string ExpectedResult { get; set; }

        //[Required]
        [Display(Name = "Actual result")]
        [DataType(DataType.MultilineText)]
        public string ActualResult { get; set; }

        [Display(Name = "Dev notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Required]
        [Display(Name = "Testcase type")]
        public TestCaseType Type { get; set; }

        [Required]
        [Display(Name = "Testcase priority")]
        public TestCasePriority Priority { get; set; }

        [Required]
        [Display(Name = "Testcase satus")]
        public TestCaseStatus TestCaseStatus { get; set; }

        //[Required, MaxLength(100)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }


        public string ModifiedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        public IEnumerable<TestCaseType> TypeOptions { get; set; }
        public IEnumerable<TestCasePriority> PriorityOptions { get; set; }
        public IEnumerable<TestCaseStatus> StatusOptions { get; set; }

        public List<TestCaseViewModel> ExistingTestCases { get; set; } = new List<TestCaseViewModel>();

        public TestCaseViewModel()
        {
            TaskDescription = string.Empty;
            Preconditions = string.Empty;
            Notes = string.Empty;
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;

        }

        public TestCaseViewModel(int taskId, string taskName, string taskDescription, int testCaseId, string testCaseName, string preconditions, string steps, string expectedResult, string actualResult,
                                    string notes, TestCaseType type, TestCasePriority priority, TestCaseStatus testCaseStatus, string createdBy, DateTime createdDate, DateTime? modifiedDate, string modifiedBy)
        {
            TaskId = taskId;
            TaskName = taskName;
            TaskDescription = taskDescription ?? string.Empty;
            TestCaseId = testCaseId;
            TestCaseName = testCaseName;
            Preconditions = preconditions ?? string.Empty;
            Steps = steps;
            ExpectedResult = expectedResult;
            ActualResult = actualResult;
            Notes = notes ?? string.Empty;
            Type = type;
            Priority = priority;
            TestCaseStatus = testCaseStatus;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
            ModifiedDate = modifiedDate;
            ModifiedBy = modifiedBy;
        }


        /// <summary>
        /// used for checking object value 
        /// hashset not able to filter if pointers are differnt 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj == null || GetType() != obj.GetType())
            {
                return result;
            }
            else
            {
                var other = (TestCaseViewModel)obj;
                if (string.Equals(ExpectedResult, other.ExpectedResult, StringComparison.OrdinalIgnoreCase)
                     && string.Equals(ActualResult, other.ActualResult, StringComparison.OrdinalIgnoreCase))
                {
                    result = true;

                }
                return result;
            }
        }


        /// <summary>
        /// returning hashcode 
        /// here hashSet able to return hashsets with corrected hashcode after equal.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(
                ExpectedResult?.Trim().ToLowerInvariant() ?? "",
                ActualResult?.Trim().ToLowerInvariant() ?? ""
            );
        }

    }
    public class TestCaseSubmission
    {
        public List<TestCaseViewModel> TestCases { get; set; }
    }

}


