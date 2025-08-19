using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkMate.Interface
{
    public interface IExcelExportService
    {
        byte[] ExportToExcel<T>(IEnumerable<T> data, Dictionary<string, Func<T, object>> columnMappings, string sheetName = "Sheet1");
    }
}