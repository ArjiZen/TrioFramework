using System.Collections.Generic;
using Bingosoft.Security.Principal;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace Bingosoft.TrioFramework.Workflow.K2Client {
    /// <summary>
    /// K2流程引擎接口
    /// </summary>
    public interface IK2Engine {

        /// <summary>
        /// 启动流程实例
        /// </summary>
        /// <param name="instance">流程实例对象</param>
        /// <remarks>同时新增流程办理历史</remarks>
        /// <returns></returns>
        bool SaveWorkflow(WorkflowInstance instance);

        /// <summary>
        /// 办理流程
        /// </summary>
        /// <param name="instance">流程实例</param>
        /// <param name="result">处理结果</param>
        /// <param name="listNextUsers">分配办理人员列表</param>
        /// <remarks></remarks>
        /// <returns></returns>
        bool RunWorkflow(WorkflowInstance instance, ApproveResult result, IList<IUser> listNextUsers);
    }
}
