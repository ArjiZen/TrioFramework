using System;
using System.Collections.Generic;

namespace Bingosoft.TrioFramework.Workflow.Core.Models {

    /// <summary>
    /// 审批结果
    /// </summary>
    [Serializable]
    public class ApproveResult {
        /// <summary>
        /// 审批结果
        /// </summary>
        public string Choice { get; set; }
        /// <summary>
        /// 下一步人员Id
        /// </summary>
        public IList<string> NextUsers { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>
        public string Comment { get; set; }
    }
}