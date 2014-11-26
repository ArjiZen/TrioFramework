using System;
using Bingosoft.Data.Attributes;

namespace Bingosoft.TrioFramework.Workflow.Core.Models {
    /// <summary>
    /// 流程转派记录
    /// </summary>
    [Table("WF_AssignHistory")]
    public class WorkflowAssign {
        /// <summary>
        /// 流程实例编号
        /// </summary>
        public string InstanceNo { get; set; }
        /// <summary>
        /// 任务Id
        /// </summary>
        public int TaskId { get; set; }
        /// <summary>
        /// 源用户Id
        /// </summary>
        public string FromUserId { get; set; }
        /// <summary>
        /// 源用户名称
        /// </summary>
        public string FromUserName { get; set; }
        /// <summary>
        /// 转派用户Id
        /// </summary>
        public string ToUserId { get; set; }
        /// <summary>
        /// 转派用户名称
        /// </summary>
        public string ToUserName { get; set; }
        /// <summary>
        /// 转派时间
        /// </summary>
        public DateTime AssignTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
