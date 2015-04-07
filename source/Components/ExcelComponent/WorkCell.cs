using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Bingosoft.TrioFramework.Component.Excel
{
    /// <summary>
    /// Excel单元格取值、赋值辅助类
    /// </summary>
    public static class WorkCellUtil
    {
        public static string GetValue(WorkStrCell cell)
        {
            return cell.Content;
        }

        public static Double GetValue(WorkNumCell cell)
        {
            Double val;
            if (!Double.TryParse(cell.Content, out val))
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

        public static WorkBoolCell SetValue(bool value)
        {
            return new WorkBoolCell(value.ToString());
        }

        public static WorkDateCell SetValue(DateTime value)
        {
            return new WorkDateCell(value.ToString(CultureInfo.CurrentCulture));
        }

        public static WorkNumCell SetValue(Double value)
        {
            return new WorkNumCell(value.ToString());
        }

        public static WorkStrCell SetValue(string str)
        {
            return new WorkStrCell(str);
        }
    }

    /// <summary>
    /// 单元格集合
    /// </summary>
    public class WorkCellCollection : Collection<WorkCell>
    {
        
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

        #region Properties

        /// <summary>
        /// 单元格内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 数据格式
        /// </summary>
        public string DataFormat { get; set; }

        #endregion

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
            this.DataFormat = "yyyy-MM-dd";
        }

        public WorkDateCell(string content)
            : base(content)
        {
            this.DataFormat = "yyyy-MM-dd";
        }
    }

    /// <summary>
    /// 金额类型的单元格
    /// </summary>
    public class WorkMoneyCell : WorkCell
    {
        public WorkMoneyCell()
        {
            this.DataFormat = "￥#.##0";
        }

        public WorkMoneyCell(string content)
            : base(content)
        {
            this.DataFormat = "￥#.##0";
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

