using System;
using System.Globalization;
using System.IO;
using Bingosoft.TrioFramework.Component.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExcelComponentTest
{
    [TestClass]
    public class NPOITest
    {
        [TestMethod]
        public void WorkbookTest()
        {
            var wb = WorkBook.Create(WorkBook.ExcelFormat.Xlsx);

            var sheet = wb.CreateSheet("Sheet1");
            sheet.Head.AddRange("字符串类型", "数值类型", "金额类型", "日期类型", "布尔类型");

            for (int i = 0; i < 100; i++)
            {
                var r = sheet.CreateRow();
                r.AddRange(new[]
                {
                    "Str" + i, 
                    (3 * i).ToString(), 
                    (1.23f * i).ToString(CultureInfo.CurrentCulture), 
                    DateTime.Today.AddDays(i).ToString(CultureInfo.CurrentCulture), 
                    (i % 2 == 0).ToString()
                }, true);
                sheet.Data.Add(r);
            }

            wb.Sheets.Add(sheet);
            var ms = wb.Save();

            string filePath = string.Format("D:\\ExcelFile.xlsx");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var fs = new FileStream(filePath, FileMode.CreateNew))
            {
                var buffer = ms.ToArray();
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
                fs.Dispose();
            }

            Assert.IsTrue(File.Exists(filePath));

        }
    }
}
