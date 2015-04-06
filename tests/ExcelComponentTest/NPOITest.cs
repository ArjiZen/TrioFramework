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
        public void CreateWorkbookTest()
        {
            var wb = WorkBook.Create(WorkBook.ExcelFormat.Xlsx);

            var sheet = wb.CreateSheet("Sheet1");
            sheet.Head.AddRange("字符串类型", "数值类型", "金额类型", "日期类型", "布尔类型");

            for (int i = 0; i < 1000; i++)
            {
                var r = sheet.CreateRow();
                r.AddRange(new[]
                {
                    "Str" + i, 
                    (3 * i).ToString(), 
                    (1.23f * i).ToString(CultureInfo.CurrentCulture), 
                    DateTime.Today.AddDays(i).ToString(CultureInfo.CurrentCulture), 
                    (i % 2 == 0).ToString()
                });
                r[2].DataFormat = "0%";
                r[3].DataFormat = "yyyy-MM-dd HH:mm:ss";
            }

            string filePath = string.Format("D:\\ExcelFile.xlsx");
            wb.Save(filePath);

            Assert.IsTrue(File.Exists(filePath));
        }


        [TestMethod]
        public void LoadWorkbookTest()
        {
            var wb = WorkBook.LoadFrom("D:\\ExcelFile.xlsx");
            Assert.IsNotNull(wb);
            Assert.AreEqual(1, wb.Sheets.Count);
            Assert.AreEqual(5, wb.Sheets[0].Head.Count);

            wb.Save("D:\\ExcelFile1.xlsx");
        }
    }
}
