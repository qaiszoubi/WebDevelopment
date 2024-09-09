using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace Hotel_Reservation_System
{
    public class ExcelExportService
    {
        public byte[] ExportToExcel<T>(List<T> data, string sheetName) where T : class
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            // Add headers
            var properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = properties[i].Name;
            }

            // Add data
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < properties.Length; j++)
                {
                    worksheet.Cells[i + 2, j + 1].Value = properties[j].GetValue(data[i]);
                }
            }

            return package.GetAsByteArray();
        }
    }
}
