using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Bingosoft.TrioFramework.Component.Excel
{
    /// <summary>
    /// Excel数据表
    /// </summary>
    public class WorkDataTable : Collection<WorkDataRow>
    {
    }

    /// <summary>
    /// Excel数据行
    /// </summary>
    public abstract class WorkDataRow
    {
        protected WorkDataRow()
        {
            this.Cells = new List<WorkCell>();
        }

        /// <summary>
        /// 单元格集合
        /// </summary>
        /// <value>The cells.</value>
        public ICollection<WorkCell> Cells { get; set; }

        /// <summary>
        /// 添加数据单元格（自动识别数据类型）
        /// </summary>
        /// <param name="value">数据值</param>
        /// <param name="autoRecognize">自动识别数据类型</param>
        public void Add(string value, bool autoRecognize = true)
        {
            this.Cells.Add(autoRecognize ? WorkCell.Create(value) : new WorkStrCell(value));
        }

        /// <summary>
        /// 添加数据单元格
        /// </summary>
        /// <param name="cell"></param>
        public void Add(WorkCell cell)
        {
            this.Cells.Add(cell);
        }

        /// <summary>
        /// 批量添加数据单元格（自动识别数据类型）
        /// </summary>
        /// <param name="values"></param>
        /// /// <param name="autoRecognize">自动识别数据类型</param>
        public void AddRange(string[] values, bool autoRecognize = true)
        {
            if (values == null)
                return;

            foreach (var value in values)
            {
                this.Add(value, autoRecognize);
            }
        }
    }
}

