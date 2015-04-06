using System.Collections.ObjectModel;

namespace Bingosoft.TrioFramework.Component.Excel
{

    /// <summary>
    /// Excel表头集合
    /// </summary>
    public class WorkHeadCollection : Collection<WorkHead>
    {
        public void Add(string content)
        {
            var c = WorkFactory.CreateWorkHead();
            if (c != null)
            {
                c.Content = content;
                this.Add(c);
            }
        }

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
        public WorkHead()
        {

        }

        public WorkHead(string content)
            : base(content)
        {

        }

        /// <summary>
        /// 子表头集合
        /// </summary>
        public WorkHeadCollection Children { get; set; }
    }
}

