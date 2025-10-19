using System;
using System.Collections.Generic;
using System.Linq;
using WorkMate.Models;

namespace WorkMate.ViewModels
{
    public class DashboardViewModel
    {
        // Summary cards
        public int TotalTasks { get; set; }
        public int OpenTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int QATasks { get; set; }
        public int CompletedTasks { get; set; }
        public int BlockedTasks { get; set; }

        // Collections
        public List<TaskCard> RecentTasks { get; set; } = new List<TaskCard>();
        public Dictionary<string, int> TasksPerUser { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> TasksByStatus { get; set; } = new Dictionary<string, int>();

        // Chart colors
        public List<string> StatusColors { get; set; } = new List<string>
        {
            "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF", "#FF9F40"
        };

        public class TaskCard
        {
            public int TaskId { get; set; }
            public string TaskName { get; set; }
            public string AssignedTo { get; set; }
            public string Priority { get; set; }
            public string Status { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        public DashboardViewModel() { }

        public DashboardViewModel(List<TaskModel> tasksList, List<AppUsers> lstUsers)
        {
            if (tasksList == null || tasksList.Count == 0)
            {
                InitializeEmpty();
                return;
            }

            try
            {
                // Summary counts
                TotalTasks = tasksList.Count;
                OpenTasks = tasksList.Count(t => t.Status == TaskStatus.Other || t.Status == TaskStatus.NotStarted);
                InProgressTasks = tasksList.Count(t => t.Status == TaskStatus.InProgress);
                QATasks = tasksList.Count(t => t.Status == TaskStatus.QA);
                CompletedTasks = tasksList.Count(t => t.Status == TaskStatus.Completed);
                BlockedTasks = tasksList.Count(t => t.Status == TaskStatus.OnHold || t.Status == TaskStatus.Cancelled);

                // Recent tasks - last 10
                RecentTasks = tasksList
                    .OrderByDescending(t => t.CreatedDate)
                    .Take(10)
                    .Select(t =>
                    {
                        var user = lstUsers.FirstOrDefault(u => u.Id == t.AssignedToUserId);
                        return new TaskCard
                        {
                            TaskId = t.TaskId,
                            TaskName = string.IsNullOrEmpty(t.TaskName) ? "Untitled Task" : t.TaskName,
                            AssignedTo = user?.UserName ?? "Unassigned",
                            Priority = t.Priority.ToString(),
                            Status = t.Status.ToString(),
                            CreatedDate = t.CreatedDate
                        };
                    })
                    .ToList();

                // Tasks per user - top 8 users
                TasksPerUser = tasksList
                    .Where(t => t.AssignedToUserId != 0)
                    .GroupBy(t => lstUsers.FirstOrDefault(u => u.Id == t.AssignedToUserId)?.UserName ?? "Unassigned")
                    .OrderByDescending(g => g.Count())
                    .Take(8)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Tasks by status
                TasksByStatus = new Dictionary<string, int>
                {
                    { "Open", OpenTasks },
                    { "In Progress", InProgressTasks },
                    { "QA", QATasks },
                    { "Completed", CompletedTasks },
                    { "Blocked", BlockedTasks }
                };
            }
            catch
            {
                InitializeEmpty();
            }
        }


        private void InitializeEmpty()
        {
            TotalTasks = OpenTasks = InProgressTasks = QATasks = CompletedTasks = BlockedTasks = 0;
            RecentTasks = new List<TaskCard>();
            TasksPerUser = new Dictionary<string, int>();
            TasksByStatus = new Dictionary<string, int>
            {
                { "Open", 0 },
                { "In Progress", 0 },
                { "QA", 0 },
                { "Completed", 0 },
                { "Blocked", 0 }
            };
        }
    }
}