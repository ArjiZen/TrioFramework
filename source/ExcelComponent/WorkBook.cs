using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Bingosoft.TrioFramework.Component.Excel
{
    /// <summary>
    /// Excel工作簿
    /// </summary>
    public abstract class WorkBook
    {
        #region ctor

        protected WorkBook()
        {
            this.Sheets = new WorkSheetCollection();
            this.Format = ExcelFormat.Xls;
        }

        #endregion

        /// <summary>
        /// Excel文件格式
        /// </summary>
        public enum ExcelFormat
        {
            /// <summary>
            /// Office2003及之前的文件版本
            /// </summary>
            Xls,
            /// <summary>
            /// Office2007及以后的版本
            /// </summary>
            Xlsx
        }

        #region Properties

        /// <summary>
        /// 工作表
        /// </summary>
        /// <value>The sheets.</value>
        public WorkSheetCollection Sheets { get; set; }

        /// <summary>
        /// Excel文件格式
        /// </summary>
        public ExcelFormat Format { get; set; }

        #endregion

        /// <summary>
        /// 创建一个工作簿
        /// </summary>
        /// <returns></returns>
        public static WorkBook Create()
        {
            return WorkFactory.CreateWorkbook();
        }

        /// <summary>
        /// 创建一个工作簿
        /// </summary>
        /// <param name="format">工作簿文件类型</param>
        /// <returns></returns>
        public static WorkBook Create(ExcelFormat format)
        {
            var wb = WorkBook.Create();
            if (wb != null)
            {
                wb.Format = format;
            }
            return wb;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public abstract MemoryStream Save();

        /// <summary>
        /// 创建工作表
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public WorkSheet CreateSheet(string sheetName)
        {
            var s = WorkFactory.CreateWorkSheet();
            if (s != null)
            {
                s.Name = sheetName;
            }
            return s;
        }
    }
}

