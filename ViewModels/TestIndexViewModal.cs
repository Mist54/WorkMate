using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Web.Mvc; 
using WorkMate.Models;

namespace WorkMate.ViewModels
{
    public class TestIndexViewModal
    {
        // Public properties for the main lists and counts
        public List<TaskModel> Tasks { get; private set; }
        public List<TestCaseModel> TestCases { get; private set; }
        public int TaskCount { get; private set; }
        public int TestCaseCount { get; private set; }

        // New properties for dropdowns and selected items
        public IEnumerable<SelectListItem> ddlTaskList { get; private set; }
        public IEnumerable<SelectListItem> ddlTestCaseList { get; private set; }
        public int SelectedTaskId { get; set; }
        public int SelectedTestCaseId { get; set; }

        public int TotalFailedTestcaseCount { get; set; } = 0;
        public int TotalPassedTestcaseCount { get; set; } = 0;
        public int TotalOtherTestcaseCount { get; set; } = 0;


        public int FailedTestcaseCount { get; set; } = 0;
        public int PassedTestcaseCount { get; set; } = 0;
        public int OtherTestcaseCount { get; set; } = 0;

        public TestIndexViewModal(List<TaskModel> tasks, List<TestCaseModel> testCases)
        {
            Tasks = tasks ?? new List<TaskModel>();
            TestCases = testCases ?? new List<TestCaseModel>();

            TaskCount = Tasks.Count;
            TestCaseCount = TestCases.Count;

            // Automatically populate the dropdown lists
            PopulateDropdowns();
            setTestcaseStatusCount(testCases);


        }

        /// <summary>
        /// Populates the dropdown lists from the main Task and TestCase lists.
        /// </summary>
        public void PopulateDropdowns()
        {
            ddlTaskList = Tasks.Select(t => new SelectListItem
            {
                Value = t.TaskId.ToString(),
                Text = t.TaskName
            }).ToList();

            ddlTestCaseList = TestCases.Select(tc => new SelectListItem
            {
                Value = tc.TestCaseId.ToString(),
                Text = tc.TestCaseName
            }).ToList(); 
        }

        public void setTestcaseStatusCount(List<TestCaseModel> testCases)
        {
            if (testCases == null || !testCases.Any())
                return;

            TotalPassedTestcaseCount = testCases.Count(x => x.TestCaseStatus == TestCaseStatus.Passed);
            TotalFailedTestcaseCount = testCases.Count(x => x.TestCaseStatus == TestCaseStatus.Failed);
            TotalOtherTestcaseCount = testCases.Count(x => x.TestCaseStatus == TestCaseStatus.Other);
        }

    }
}