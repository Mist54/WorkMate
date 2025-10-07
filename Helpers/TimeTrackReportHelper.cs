using System.Collections.Generic;
using System.Linq;
using WorkMate.Models;
using WorkMate.Services;
using System;

namespace WorkMate.Helpers
{
    public class TimeTrackReportHelper
    {
        private readonly ExcelExportService _excelExportService;

        public TimeTrackReportHelper()
        {
            _excelExportService = new ExcelExportService();
        }

        public byte[] GenerateExcelReport(IEnumerable<TimeTrackModel> timeTracks)
        {
            var columnMappings = new Dictionary<string, Func<TimeTrackModel, object>>
            {
                { "Task Name", t => t.Task?.TaskName },
                { "Description", t => t.Description },
                { "Start Date", t => t.StartDate },
                { "End Date", t => t.EndDate },
                { "Status", t => t.Status.ToString() },
                { "Is Manual", t => t.IsManual ? "Yes" : "No" }
            };

            return _excelExportService.ExportToExcel(timeTracks, columnMappings, "Time Track Report");
        }
    }
}
