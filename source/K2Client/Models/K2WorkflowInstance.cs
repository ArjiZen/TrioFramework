using System.Collections.Generic;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace Bingosoft.TrioFramework.Workflow.K2Client.Models {
    /// <summary>
    /// K2流程实例
    /// </summary>
    public class K2WorkflowInstance : WorkflowInstance {

        /// <summary>
        /// 获取流程实例办理记录
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<WorkflowItem> GetWorkItems() {
            return WorkflowItemFactory.GetAll<K2WorkflowItem>(this.InstanceNo);
        }
    }
}
