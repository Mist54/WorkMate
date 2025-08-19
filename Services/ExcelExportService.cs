using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WorkMate.Interface;

namespace WorkMate.Services
{
    public class ExcelExportService : IExcelExportService
    {
        public byte[] ExportToExcel<T>(IEnumerable<T> data,Dictionary<string, Func<T, object>> columnMappings,string sheetName = "Sheet1")
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(sheetName);

                    // --- Headers ---
                    int col = 1;
                    foreach (var header in columnMappings.Keys)
                    {
                        worksheet.Cell(1, col).Value = header;
                        worksheet.Cell(1, col).Style.Font.Bold = true;
                        col++;
                    }

                    // --- Rows ---
                    int row = 2;
                    foreach (var item in data)
                    {
                        col = 1;
                        foreach (var map in columnMappings.Values)
                        {
                            var val = map(item);

                            if (val == null)
                            {
                                worksheet.Cell(row, col).Value = string.Empty;
                            }
                            else if (val is DateTime dt)
                            {
                                worksheet.Cell(row, col).Value = dt;
                                worksheet.Cell(row, col).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
                            }
                            else if (val.GetType().IsEnum)
                            {
                                worksheet.Cell(row, col).Value = val.ToString();
                            }
                            else if (val is bool b)
                            {
                                worksheet.Cell(row, col).Value = b;
                            }
                            else if (val is int || val is long || val is double || val is decimal || val is float)
                            {
                                worksheet.Cell(row, col).Value = Convert.ToDouble(val);
                            }
                            else
                            {
                                worksheet.Cell(row, col).Value = val.ToString();
                            }

                            col++;
                        }
                        row++;
                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return stream.ToArray();
                    }
                }
            }
            catch
            {
                throw; // preserve stack trace
            }
        }

    }
}