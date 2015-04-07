using Bingosoft.Data;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace Bingosoft.TrioFramework.Workflow.K2Client.Models {
    /// <summary>
    /// K2流程项
    /// </summary>
    public class K2WorkflowItem : WorkflowItem {

        private static readonly Dao _dao = Dao.Get();

        /// <summary>
        /// 更新K2流程项
        /// </summary>
        public override void Update() {
            _dao.UpdateFields<K2WorkflowItem>(this, "ReadTime", "IsSign", "SignTime", "FinishTime", "AutoFinished", "TaskStatus", "Comment", "Choice");
        }
    }
}
