using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingosoft.TrioFramework.Component.Excel.NPOI
{
    /// <summary>
    /// Excel工作表
    /// </summary>
    public class WorkSheet : Excel.WorkSheet
    {
        #region ctor

        public WorkSheet()
            : base()
        {
        }

        public WorkSheet(string name)
            : base(name)
        {
        }

        #endregion
    }
}
