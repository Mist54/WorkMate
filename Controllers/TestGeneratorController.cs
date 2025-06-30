using ClosedXML.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkMate.Filters;
using WorkMate.ViewModels;
using WorkMate.Models;
using System.Text;

namespace WorkMate.Controllers
{
    public class TestGeneratorController : Controller
    {
        #region Manual EXCEL Test file generator 
        // GET: TestGenerator
        public ActionResult TestExporter()
        {
            return View();
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public ActionResult SubmitFile(ViewModels.TestCaseSubmission data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Invalid data" });
                }
                else
                {
                    if (data != null && data.TestCases != null && data.TestCases.Count > 0)
                    {
                        // Save the test case data in session for later download
                        Session["LastSubmittedTestCases"] = data.TestCases;
                    }
                    else
                    {
                        return Json(new { success = false, message = "Invalid data" });
                    }

                    return Json(new { success = true, message = "Uploaded successfully. Starting download...", downloadUrl = Url.Action("DownloadExcel") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Invalid data" + ex });
            }




        }

        private FileResult ExportDataToExelFile(ViewModels.TestCaseSubmission testCaseSubmissionData)
        {
            List<ViewModels.TestCaseViewModel> testCases = testCaseSubmissionData.TestCases;

            if (testCases == null || !testCases.Any())
                throw new ArgumentException("No test cases provided");

            string taskName = testCases[0].TaskName?.Replace(" ", "_") ?? "Task";
            int taskId = testCases[0].TaskId;
            string fileName = $"{taskName}_{taskId}.xlsx";

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("TestCases");

                // Header
                ws.Cell(1, 1).Value = "Test Case";
                ws.Cell(1, 2).Value = "Preconditions";
                ws.Cell(1, 3).Value = "Steps";
                ws.Cell(1, 4).Value = "Expected Result";
                ws.Cell(1, 5).Value = "Actual Result";
                ws.Cell(1, 6).Value = "Notes";
                ws.Cell(1, 7).Value = "Type";
                ws.Cell(1, 8).Value = "Priority";
                ws.Cell(1, 9).Value = "Status";
                ws.Cell(1, 10).Value = "Created By";
                ws.Cell(1, 11).Value = "Created Date";

                for (int i = 0; i < testCases.Count; i++)
                {
                    var t = testCases[i];
                    int row = i + 2;

                    ws.Cell(row, 1).Value = t.TestCaseName;
                    ws.Cell(row, 2).Value = t.Preconditions;
                    ws.Cell(row, 3).Value = t.Steps;
                    ws.Cell(row, 4).Value = t.ExpectedResult;
                    ws.Cell(row, 5).Value = t.ActualResult;
                    ws.Cell(row, 6).Value = t.Notes;
                    ws.Cell(row, 7).Value = t.Type.ToString();
                    ws.Cell(row, 8).Value = t.Priority.ToString();
                    ws.Cell(row, 9).Value = t.TestCaseStatus.ToString();
                    ws.Cell(row, 10).Value = string.IsNullOrEmpty(t.CreatedBy) ? "" : t.CreatedBy;
                    ws.Cell(row, 11).Value = DateTime.Now.ToString("dd/MM/yyyy : HH:mm");
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [HttpGet]
        public ActionResult DownloadExcel()
        {
            List<ViewModels.TestCaseViewModel> data = Session["LastSubmittedTestCases"] as List<ViewModels.TestCaseViewModel>;
            if (data == null || !data.Any())
            {
                return Content("No test case data available to download.");
            }

            var submission = new ViewModels.TestCaseSubmission { TestCases = data };
            return ExportDataToExelFile(submission);
        }

        #endregion Manual EXCEL Test file generator 


       
    }

}