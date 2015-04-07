using System;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Bingosoft.TrioFramework.Component.Excel.NPOI
{
    /// <summary>
    /// Excel工作簿
    /// </summary>
    public class WorkBook : Excel.WorkBook
    {
        #region ctor

        public WorkBook()
            : base()
        {
        }

        #endregion

        /// <summary>
        /// 获取工作表
        /// </summary>
        /// <returns></returns>
        private IWorkbook GetWorkbook()
        {
            if (this.Format == ExcelFormat.Xls)
            {
                return new HSSFWorkbook();
            }
            else
            {
                return new XSSFWorkbook();
            }
        }

        /// <summary>
        /// 保存工作表
        /// </summary>
        /// <returns></returns>
        public override MemoryStream Save()
        {
            IWorkbook wb = GetWorkbook();
            foreach (var sheet in this.Sheets)
            {
                var s = wb.CreateSheet(sheet.Name);
                var rownum = 0;
                var cellnum = 0;
                // 表头
                var headRow = s.CreateRow(rownum);
                foreach (var head in sheet.Head)
                {
                    var c = headRow.CreateCell(cellnum);
                    c.SetCellValue(WorkCellUtil.GetValue(head));
                    c.SetCellType(CellType.String);
                    cellnum++;
                }
                rownum++;
                // 数据行
                foreach (var row in sheet.Data)
                {
                    var r = s.CreateRow(rownum);
                    cellnum = 0;
                    foreach (Excel.WorkCell cell in row.Cells)
                    {
                        var c = r.CreateCell(cellnum);
                        if (cell is WorkDateCell)
                        {
                            var wc = cell as WorkDateCell;
                            c.SetCellValue(WorkCellUtil.GetValue(wc));
                            c.SetCellType(CellType.Numeric);
                            // 设置数据格式
                            if (!string.IsNullOrEmpty(wc.DataFormat) && this.Format == ExcelFormat.Xlsx)
                            {
                                // ISSUS: XLS格式文件写入21行后就没有自定义格式
                                var style = wb.CreateCellStyle();
                                style.DataFormat = wb.CreateDataFormat().GetFormat(wc.DataFormat);
                                c.CellStyle = style;
                            }
                        }
                        else if (cell is WorkNumCell)
                        {
                            var wc = cell as WorkNumCell;
                            c.SetCellValue(WorkCellUtil.GetValue(wc));
                            c.SetCellType(CellType.Numeric);
                            if (!string.IsNullOrEmpty(wc.DataFormat) && this.Format == ExcelFormat.Xlsx)
                            {
                                var style = wb.CreateCellStyle();
                                var format = wb.CreateDataFormat();
                                style.DataFormat = format.GetFormat(wc.DataFormat);
                                c.CellStyle = style;
                            }
                        }
                        else if (cell is WorkMoneyCell)
                        {
                            var wc = cell as WorkMoneyCell;
                            c.SetCellValue(WorkCellUtil.GetValue(wc));
                            if (!string.IsNullOrEmpty(wc.DataFormat) && this.Format == ExcelFormat.Xlsx)
                            {
                                var style = wb.CreateCellStyle();
                                var format = wb.CreateDataFormat();
                                style.DataFormat = format.GetFormat(wc.DataFormat);
                                c.CellStyle = style;
                            }
                        }
                        else if (cell is WorkBoolCell)
                        {
                            c.SetCellValue(WorkCellUtil.GetValue((WorkBoolCell)cell));
                            c.SetCellType(CellType.Boolean);
                        }
                        else
                        {
                            c.SetCellValue(cell.Content);
                            c.SetCellType(CellType.String);
                            if (!string.IsNullOrEmpty(cell.DataFormat) && this.Format == ExcelFormat.Xlsx)
                            {
                                var style = wb.CreateCellStyle();
                                var format = wb.CreateDataFormat();
                                style.DataFormat = format.GetFormat(cell.DataFormat);
                                c.CellStyle = style;
                            }
                        }
                        cellnum++;
                    }
                    rownum++;
                }
            }

            var ms = new MemoryStream();
            wb.Write(ms);
            return ms;
        }

        /// <summary>
        /// 从文件读取Excel文件
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="headRows">标题行数</param>
        public override void Load(MemoryStream ms, int headRows = 1)
        {
            if (headRows > 1)
            {
                throw new NotImplementedException("目前只支持单行表头的读取");
            }
            var wb = WorkbookFactory.Create(ms);
            this.Format = (wb is XSSFWorkbook) ? ExcelFormat.Xlsx : ExcelFormat.Xls;
            for (int sheetIndex = 0; sheetIndex < wb.NumberOfSheets; sheetIndex++)
            {
                var s = wb.GetSheetAt(sheetIndex);
                var mSheet = this.CreateSheet(s.SheetName);
                // 标题
                for (int rowIndex = 0; rowIndex < headRows; rowIndex++)
                {
                    var r = s.GetRow(rowIndex);
                    // 标题行数
                    for (int cellIndex = 0; cellIndex < r.PhysicalNumberOfCells; cellIndex++)
                    {
                        var c = r.GetCell(cellIndex);
                        mSheet.Head.Add(c.StringCellValue);
                    }
                }
                // 数据
                for (int rowIndex = headRows; rowIndex < s.PhysicalNumberOfRows; rowIndex++)
                {
                    var r = s.GetRow(rowIndex);
                    var mRow = mSheet.CreateRow();
                    for (int cellIndex = 0; cellIndex < r.PhysicalNumberOfCells; cellIndex++)
                    {
                        var c = r.GetCell(cellIndex);
                        switch (c.CellType)
                        {
                            case CellType.Boolean:
                            {
                                mRow.Add(WorkCellUtil.SetValue(c.BooleanCellValue));
                                break;
                            }
                            case CellType.Numeric:
                            {
                                if (DateUtil.IsCellDateFormatted(c))
                                {
                                    mRow.Add(WorkCellUtil.SetValue(c.DateCellValue));
                                }
                                else
                                {
                                    mRow.Add(WorkCellUtil.SetValue(c.NumericCellValue));
                                }
                                break;
                            }
                            case CellType.String:
                            {
                                try
                                {
                                    mRow.Add(WorkCellUtil.SetValue(c.RichStringCellValue.String));
                                }
                                catch (Exception)
                                {
                                    mRow.Add(WorkCellUtil.SetValue(c.StringCellValue));
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
