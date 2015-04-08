using System.Collections.ObjectModel;
using System.Linq;

namespace Bingosoft.TrioFramework.Component.Excel
{

    /// <summary>
    /// Excel表头集合
    /// </summary>
    public class WorkHeadCollection : Collection<WorkHead>
    {
        /// <summary>
        /// 初始化表头单元格
        /// </summary>
        /// <param name="content"></param>
        public void Add(string content)
        {
            var c = WorkFactory.CreateWorkHead();
            if (c != null)
            {
                c.Content = content;
                this.Add(c);
            }
        }

        /// <summary>
        /// 初始化表头单元格集合
        /// </summary>
        /// <param name="heads"></param>
        public void AddRange(params string[] heads)
        {
            if (heads == null) return;
            foreach (var head in heads)
            {
                var c = WorkFactory.CreateWorkHead();
                if (c != null)
                {
                    c.Content = head;
                    this.Add(c);
                }
            }
        }
    }

    /// <summary>
    /// Excel表头
    /// </summary>
    /// <remarks>
    /// 树结构
    /// </remarks>
    public class WorkHead : WorkStrCell
    {
        /// <summary>
        /// 初始化表头单元格
        /// </summary>
        public WorkHead()
        {
            this.Children = new WorkHeadCollection();
        }

        /// <summary>
        /// 初始化表头单元格
        /// </summary>
        /// <param name="content"></param>
        public WorkHead(string content)
            : base(content)
        {
            this.Children = new WorkHeadCollection();
        }

        /// <summary>
        /// 子表头集合
        /// </summary>
        public WorkHeadCollection Children { get; set; }

        /// <summary>
        /// 新建表头单元格
        /// </summary>
        /// <returns></returns>
        public static WorkHead Create()
        {
            return WorkFactory.CreateWorkHead();
        }

        /// <summary>
        /// 新建表头单元格
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public new static WorkHead Create(string content)
        {
            var h = WorkFactory.CreateWorkHead();
            h.Content = content;
            return h;
        }
    }
}

