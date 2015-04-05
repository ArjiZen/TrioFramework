using System;

namespace Bingosoft.TrioFramework.Component.Excel
{

    public static class WorkCellValuer
    {
        public static string GetValue(WorkStrCell cell)
        {
            return cell.Content;
        }

        public static Int32 GetValue(WorkNumCell cell)
        {
            Int32 val;
            if (!Int32.TryParse(cell.Content, out val))
            {
                throw new InvalidCastException("");
            }
            return val;
        }

        public static DateTime GetValue(WorkDateCell cell)
        {
            DateTime val;
            if (!DateTime.TryParse(cell.Content, out val))
            {
                throw new InvalidCastException("");
            }
            return val;
        }

        public static double GetValue(WorkMoneyCell cell)
        {
            Double val;
            if (!Double.TryParse(cell.Content, out val))
            {
                throw new InvalidCastException("");
            }
            return val;
        }

        public static bool GetValue(WorkBoolCell cell)
        {
            bool val;
            if (!bool.TryParse(cell.Content, out val))
            {
                throw new InvalidCastException("");
            }
            return val;
        }
    }

    /// <summary>
    /// Excel单元格
    /// </summary>
    public abstract class WorkCell
    {
        #region ctor

        protected WorkCell()
        {
        }

        protected WorkCell(string content)
        {
            this.Content = content;
        }

        #endregion

        /// <summary>
        /// 单元格内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 创建单元格
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static WorkCell Create(string content)
        {
            return WorkFactory.CreateWorkCell(content);
        }
    }

    /// <summary>
    /// 字符串类型的单元格
    /// </summary>
    public class WorkStrCell : WorkCell
    {
        public WorkStrCell()
        {

        }

        public WorkStrCell(string content)
            : base(content)
        {
        }
    }

    /// <summary>
    /// 数字类型的单元格
    /// </summary>
    public class WorkNumCell : WorkCell
    {
        public WorkNumCell()
        {

        }

        public WorkNumCell(string content)
            : base(content)
        {
        }
    }

    /// <summary>
    /// 日期类型的单元格
    /// </summary>
    public class WorkDateCell : WorkCell
    {
        public WorkDateCell()
        {

        }

        public WorkDateCell(string content)
            : base(content)
        {
        }
    }

    /// <summary>
    /// 金额类型的单元格
    /// </summary>
    public class WorkMoneyCell : WorkCell
    {
        public WorkMoneyCell()
        {

        }

        public WorkMoneyCell(string content)
            : base(content)
        {
        }
    }

    /// <summary>
    /// 布尔类型的单元格
    /// </summary>
    public class WorkBoolCell : WorkCell
    {
        public WorkBoolCell()
        {

        }

        public WorkBoolCell(string content)
            : base(content)
        {

        }
    }
}

