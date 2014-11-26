using System.Collections.Generic;

namespace Bingosoft.TrioFramework.Workflow.Core.Models {
    /// <summary>
    /// 流程环节
    /// </summary>
    public abstract class WorkflowActivity {
        /// <summary>
        /// 环节名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 当前环节审批人所属角色（用于人员计算）
        /// </summary>
        public Actor Actor { get; set; }
        /// <summary>
        /// 当前环节迁移
        /// </summary>
        public IDictionary<string, WorkflowActivity> Transitions { get; set; }
    }
}
