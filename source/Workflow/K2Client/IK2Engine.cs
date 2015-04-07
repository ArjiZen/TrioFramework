using System.Collections.Generic;
using Bingosoft.Security.Principal;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace Bingosoft.TrioFramework.Workflow.K2Client {
	/// <summary>
	/// K2流程引擎接口
	/// </summary>
	public abstract class IK2Engine {
		/// <summary>
		/// 当前用户
		/// </summary>
		/// <value>The current user.</value>
		public IUser CurrentUser { get; set; }

		/// <summary>
		/// 启动流程实例
		/// </summary>
		/// <param name="instance">流程实例对象</param>
		/// <remarks>同时新增流程办理历史</remarks>
		/// <returns></returns>
		public abstract bool SaveWorkflow(WorkflowInstance instance);

		/// <summary>
		/// 办理流程
		/// </summary>
		/// <param name="instance">流程实例</param>
		/// <param name="result">处理结果</param>
		/// <param name="listNextUsers">分配办理人员列表</param>
		/// <param name="tobeReadUsers">待阅人员</param>
		/// <remarks></remarks>
		/// <returns></returns>
		public abstract bool RunWorkflow(WorkflowInstance instance, ApproveResult result, IList<IUser> listNextUsers, IList<IUser> tobeReadUsers);

		/// <summary>
		/// 传阅流程
		/// </summary>
		/// <returns><c>true</c>, if around was passed, <c>false</c> otherwise.</returns>
		/// <param name="instance">流程实例.</param>
		/// <param name="toUsers">传阅用户.</param>
		public abstract bool PassAround(WorkflowInstance instance, IList<IUser> toUsers);
	}
}
