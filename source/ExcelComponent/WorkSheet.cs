using System.Collections.ObjectModel;

namespace Bingosoft.TrioFramework.Component.Excel
{
    /// <summary>
    /// Excel工作表集合
    /// </summary>
    public class WorkSheetCollection : Collection<WorkSheet>
    {
    }

    /// <summary>
    /// Excel工作表
    /// </summary>
    public abstract class WorkSheet
    {
        #region ctor

        protected WorkSheet()
        {
            this.Head = new WorkHeadCollection();
            this.Data = new WorkDataTable();
        }

        protected WorkSheet(string name)
            : this()
        {
            this.Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 工作表名称
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// 表头集合
        /// </summary>
        /// <value>The headers.</value>
        public WorkHeadCollection Head { get; set; }

        /// <summary>
        /// 数据集合
        /// </summary>
        /// <value>The table.</value>
        public WorkDataTable Data { get; set; }

        #endregion

        /// <summary>
        /// 创建表头
        /// </summary>
        /// <returns></returns>
        public WorkHead CreateHead()
        {
            return WorkFactory.CreateWorkHead();
        }

        /// <summary>
        /// 创建数据行
        /// </summary>
        /// <returns></returns>
        public WorkDataRow CreateRow()
        {
            return WorkFactory.CreateDataRow();
        }
    }
}

