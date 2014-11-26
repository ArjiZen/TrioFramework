using System.Collections.Generic;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace Bingosoft.TrioFramework.Workflow.Core {
    internal class WorkflowCache {
        static WorkflowCache() {
            WorkflowCache.Definitions = new HashSet<WorkflowDefinition>();
        }
        /// <summary>
        /// 流程定义缓存
        /// </summary>
        public static HashSet<WorkflowDefinition> Definitions { get; set; }
    }
}
