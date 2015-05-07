using System;

// ReSharper disable once CheckNamespace
namespace Bingosoft.TrioFramework.Workflow.Core {
    /// <summary>
    /// 用于标示当前配置所属流程
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class WorkflowAttribute : Attribute {

        /// <summary>
        /// 标示当前配置所属流程
        /// </summary>
        /// <param name="appCode">流程编号</param>
        public WorkflowAttribute(int appCode) {
            this.AppCode = appCode;
        }

        #region Properties

        /// <summary>
        /// 流程编号
        /// </summary>
        public int AppCode { get; set; }

        #endregion
    }
}