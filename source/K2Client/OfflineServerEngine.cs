using System.Collections.Generic;
using Bingosoft.Security.Principal;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace Bingosoft.TrioFramework.Workflow.K2Client {
    /// <summary>
    /// 离线K2服务器引擎
    /// </summary>
    public class OfflineServerEngine : IK2Engine{
        /// <summary>
        /// 保存流程
        /// </summary>
        /// <param name="instance">流程实例</param>
        /// <returns></returns>
		public override bool SaveWorkflow(WorkflowInstance instance) {
            return true;
        }

        /// <summary>
        /// 运行流程
        /// </summary>
        /// <param name="instance">流程实例</param>
        /// <param name="result">处理结果</param>
        /// <param name="listNextUsers">下一环节处理人</param>
		/// <param name="tobeReadUsers">待阅人员</param>
        /// <returns></returns>
		public override bool RunWorkflow(WorkflowInstance instance, ApproveResult result, IList<IUser> listNextUsers, IList<IUser> tobeReadUsers) {
            return true;
        }
    }
}
