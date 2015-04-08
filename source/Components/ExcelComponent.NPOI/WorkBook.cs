using System;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
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
            if (this.Format == ExcelFormat.xls)
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
                // 多行表头
                var headArr = sheet.Head.Get2DArray();
                for (int headRowIndex = 0; headRowIndex < headArr.Length; headRowIndex++)
                {
                    cellnum = 0;
                    var headRow = headArr[headRowIndex];
                    var row = s.CreateRow(rownum);
                    for (int headCellIndex = 0; headCellIndex < headRow.Length; headCellIndex++)
                    {
                        var cell = headRow[headCellIndex];
                        if (cell != null)
                        {
                            var c = row.CreateCell(cellnum);
                            c.SetCellValue(WorkCellUtil.GetValue(cell));
                            c.SetCellType(CellType.String);
                            var ncell = cell;
                            if (ncell.ColSpan > 1)
                            {
                                // 列合并
                                s.AddMergedRegion(new CellRangeAddress(rownum, rownum, cellnum, cellnum + ncell.ColSpan - 1));
                            }
                            if (headRowIndex + 1 < headArr.Length && headArr[headRowIndex + 1][headCellIndex] == null)
                            {
                                // 如果当前单元格的下面一个单元格为NULL，则默认为行合并
                                // 行合并
                                s.AddMergedRegion(new CellRangeAddress(rownum, headArr.Length - 1, cellnum, cellnum));
                            }
                        }
                        cellnum++;
                    }
                    rownum++;
                }
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
                            var val = WorkCellUtil.GetValue(wc);
                            if (this.Format == ExcelFormat.xls)
                            {
                                c.SetCellValue(val.ToString("yyyy-MM-dd HH:mm:ss"));
                                c.SetCellType(CellType.String);
                            }
                            else if (this.Format == ExcelFormat.xlsx)
                            {
                                c.SetCellValue(val);
                            }
                            // 设置数据格式
                            if (!string.IsNullOrEmpty(wc.DataFormat) && this.Format == ExcelFormat.xlsx)
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
                            if (!string.IsNullOrEmpty(wc.DataFormat) && this.Format == ExcelFormat.xlsx)
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
                            if (!string.IsNullOrEmpty(wc.DataFormat) && this.Format == ExcelFormat.xlsx)
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
                            if (!string.IsNullOrEmpty(cell.DataFormat) && this.Format == ExcelFormat.xlsx)
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
            IWorkbook wb = WorkbookFactory.Create(ms);
            this.Format = (wb is XSSFWorkbook) ? ExcelFormat.xlsx : ExcelFormat.xls;
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
