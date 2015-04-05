using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
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
                    c.SetCellValue(WorkCellValuer.GetValue(head));
                    headRow.Cells.Add(c);
                    cellnum++;
                }
                rownum++;
                // 数据行
                foreach (var row in sheet.Data)
                {
                    var r = s.CreateRow(rownum);
                    cellnum = 0;
                    foreach (WorkCell cell in row.Cells)
                    {
                        var c = r.CreateCell(cellnum);
                        if (cell is WorkDateCell)
                        {
                            c.SetCellValue(WorkCellValuer.GetValue((WorkDateCell)cell));
                        }
                        else if (cell is WorkNumCell)
                        {
                            c.SetCellValue(WorkCellValuer.GetValue((WorkNumCell)cell));
                            c.SetCellType(CellType.Numeric);
                        }
                        else if (cell is WorkMoneyCell)
                        {
                            c.SetCellValue(WorkCellValuer.GetValue((WorkMoneyCell)cell));
                            c.SetCellType(CellType.Numeric);
                        }
                        else if (cell is WorkBoolCell)
                        {
                            c.SetCellValue(WorkCellValuer.GetValue((WorkBoolCell)cell));
                            c.SetCellType(CellType.Boolean);
                        }
                        else
                        {
                            c.SetCellValue(cell.Content);
                            c.SetCellType(CellType.String);
                        }
                        r.Cells.Add(c);
                        cellnum++;
                    }
                    rownum++;
                }
            }

            var ms = new MemoryStream();
            wb.Write(ms);
            return ms;
        }
    }
}
