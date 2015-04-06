using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

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
        /// 从MemoryStream中加载Excel文档数据
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static WorkBook LoadFrom(MemoryStream ms)
        {
            var wb = WorkBook.Create();
            if (wb != null)
            {
                wb.Load(ms);
            }
            return wb;
        }

        /// <summary>
        /// 从文件读取Excel工作簿
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static WorkBook LoadFrom(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }
            var ext = Path.GetExtension(filePath);
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                if (!fs.CanRead)
                {
                    throw new InvalidOperationException("无法读取文件" + filePath);
                }
                var buffer = fs.ReadBytes(fs.Length);
                var ms = new MemoryStream(buffer);
                return WorkBook.LoadFrom(ms);
            }
        }

        #region Members

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public abstract MemoryStream Save();

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isCover">是否覆盖目标文件</param>
        /// <returns></returns>
        public void Save(string filePath, bool isCover = true)
        {
            if (isCover && File.Exists(filePath))
            {
                var retryCount = 0;
                var isDeleted = false;
                do
                {
                    try
                    {
                        File.Delete(filePath);
                        isDeleted = true;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        break;
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(1000);
                        retryCount++;
                    }
                } while (retryCount <= 2 && !isDeleted);
                File.Delete(filePath);
            }

            if (!isCover && File.Exists(filePath))
            {
                throw new IOException("目标文件已存在");
            }

            var ext = Path.GetExtension(filePath);
            if (ext == null || !ext.Equals("." + this.Format, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException("目标文件格式与当前工作簿文件格式不符，目标文件应该为." + this.Format + "格式");
            }

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                var ms = this.Save();
                var buffer = ms.ToArray();
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
                fs.Dispose();
            }
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="headRows">标题行数</param>
        public abstract void Load(MemoryStream ms, int headRows = 1);

        /// <summary>
        /// 创建工作表并添加到当前工作簿
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
            this.Sheets.Add(s);
            return s;
        }

        #endregion


    }
}

