using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Bingosoft.TrioFramework.Component.Excel.NPOI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkBook = Bingosoft.TrioFramework.Component.Excel.WorkBook;
using WorkHead = Bingosoft.TrioFramework.Component.Excel.WorkHead;

namespace ExcelComponentTest
{
    [TestClass]
    public class NPOITest
    {
        [TestMethod]
        public void CreateWorkbookTest()
        {
            var wb = WorkBook.Create();

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

            var xlsxPath = string.Format("D:\\SingleHeadExcel.xlsx");
            wb.Format= WorkBook.ExcelFormat.xlsx;
            wb.Save(xlsxPath);
            Assert.IsTrue(File.Exists(xlsxPath));

            var xlsPath = string.Format("D:\\SingleHeadExcel.xls");
            wb.Format = WorkBook.ExcelFormat.xls;
            wb.Save(xlsPath);
            Assert.IsTrue(File.Exists(xlsPath));
        }

        [TestMethod]
        public void CreateMultiHeadWorkbookTest()
        {
            var wb = WorkBook.Create();

            var sheet = wb.CreateSheet("Sheet1");
            var h1 = sheet.CreateHead();
            h1.Content = "h1";
            var h11 = WorkHead.Create("h11");
            var h111 = WorkHead.Create("h111");
            h11.Children.Add(h111);
            var h12 = WorkHead.Create("h12");
            var h121 = WorkHead.Create("h121");
            var h122 = WorkHead.Create("h122");
            h12.Children.Add(h121);
            h12.Children.Add(h122);
            h1.Children.Add(h11);
            h1.Children.Add(h12);

            var h2 = sheet.CreateHead();
            h2.Content = "h2";
            var h21 = WorkHead.Create("h21");
            var h211 = WorkHead.Create("h211");
            h21.Children.Add(h211);
            h2.Children.Add(h21);

            var h3 = sheet.CreateHead();
            h3.Content = "h3";

            Assert.IsNotNull(sheet.Head[0] as Bingosoft.TrioFramework.Component.Excel.NPOI.WorkHead);
            Assert.AreEqual(3, (sheet.Head[0] as Bingosoft.TrioFramework.Component.Excel.NPOI.WorkHead).ColSpan);

            var array = sheet.Head.Get2DArray();

            Assert.AreEqual("h122", array[2][2].Content);
            Assert.IsNull(array[1][2]);

            for (int i = 0; i < 1000; i++)
            {
                var r = sheet.CreateRow();
                r.AddRange(new[]
                {
                    "Str" + i, 
                    (3 * i).ToString(), 
                    (1.23f * i).ToString(CultureInfo.CurrentCulture), 
                    DateTime.Today.AddDays(i).ToString(CultureInfo.CurrentCulture), 
                });
                r[2].DataFormat = "0%";
                r[3].DataFormat = "yyyy-MM-dd HH:mm:ss";
            }

            string xlsxPath = string.Format("D:\\MultiHeadExcel.xlsx");
            wb.Format = WorkBook.ExcelFormat.xlsx;
            wb.Save(xlsxPath);
            Assert.IsTrue(File.Exists(xlsxPath));

            string xlsPath = string.Format("D:\\MultiHeadExcel.xls");
            wb.Format = WorkBook.ExcelFormat.xls;
            wb.Save(xlsPath);
            Assert.IsTrue(File.Exists(xlsPath));

        }


        [TestMethod]
        public void CreateBigWorkbookTest()
        {
            var wb = WorkBook.Create();

            var sheet = wb.CreateSheet("Sheet1");
            sheet.Head.AddRange("字符串类型", "数值类型", "金额类型", "日期类型", "布尔类型");

            for (int i = 0; i < 100000; i++)
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

            var formatter = new BinaryFormatter();
            var ms = new MemoryStream();
            formatter.Serialize(ms, wb);
            ms.Seek(0, SeekOrigin.Begin);
            using (var fs = new FileStream("D:\\wb.size", FileMode.CreateNew))
            {
                var buffer = ms.ToArray();
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
                fs.Close();
            }

            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
            //var xlsxPath = string.Format("D:\\SingleHeadExcel.xlsx");
            //wb.Format = WorkBook.ExcelFormat.xlsx;
            //wb.Save(xlsxPath);
            //stopwatch.Stop();
            //Console.WriteLine("XLSX:" + stopwatch.ElapsedMilliseconds + "ms");
            //Assert.IsTrue(File.Exists(xlsxPath));

            //stopwatch.Reset();
            //stopwatch.Start();
            //var xlsPath = string.Format("D:\\SingleHeadExcel.xls");
            //wb.Format = WorkBook.ExcelFormat.xls;
            //wb.Save(xlsPath);
            //stopwatch.Stop();
            //Console.WriteLine("XLS:" + stopwatch.ElapsedMilliseconds + "ms");
            //Assert.IsTrue(File.Exists(xlsPath));
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
