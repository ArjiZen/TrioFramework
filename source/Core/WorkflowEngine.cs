using System;
using System.Configuration;
using System.Linq;
using Bingosoft.Security;
using Bingosoft.Security.Principal;
using Bingosoft.TrioFramework.Workflow.Core.Exceptions;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace Bingosoft.TrioFramework.Workflow.Core {
	/// <summary>
	/// 工作流引擎
	/// </summary>
	public abstract class WorkflowEngine {
		public WorkflowEngine() {
			this.m_CurrentUser = null;
		}

		#region 实例化流程引擎

		/// <summary>
		/// 获取流程引擎实例
		/// </summary>
		[Obsolete("Instance属性不再返回单一实例，改用Create()创建流程引擎实例")]
		public static WorkflowEngine Instance {
			get {
				var configurator = SettingProvider.Workflow.Provider;
				var t = Type.GetType(configurator);
				if (t == null) {
					throw new WorkflowEngineNotFoundException(configurator);
				}
				var engine = (WorkflowEngine)Activator.CreateInstance(t);
				return engine;
			}
		}

		/// <summary>
		/// 创建流程引擎实例
		/// </summary>
		public static WorkflowEngine Create(){
			var configurator = SettingProvider.Workflow.Provider;
			var t = Type.GetType(configurator);
			if (t == null) {
				throw new WorkflowEngineNotFoundException(configurator);
			}
			var engine = (WorkflowEngine)Activator.CreateInstance(t);
			return engine;
		}

		#endregion

		private IUser m_CurrentUser = null;

		/// <summary>
		/// 当前用户
		/// </summary>
		public IUser CurrentUser {
			get {
				if (m_CurrentUser == null) {
					m_CurrentUser = SecurityContext.User;
				}
				if (m_CurrentUser == null) {
					throw new NullReferenceException("未设置流程当前处理用户");
				}
				return m_CurrentUser;
			}
		}

		/// <summary>
		/// 设置当前用户
		/// </summary>
		/// <param name="loginId">用户登录id</param>
		public void SetCurrentUser(string loginId) {
			this.m_CurrentUser = SecurityContext.Provider.GetUser(loginId);
		}

		/// <summary>
		/// 获取流程定义缓存
		/// </summary>
		public static WorkflowDefinition[] Definitions {
			get { return WorkflowCache.Definitions.OrderBy(p => p.AppCode).ToArray(); }
		}

		/// <summary>
		/// 初始化流程定义
		/// </summary>
		/// <returns></returns>
		public abstract WorkflowDefinition[] LoadDefinitions();

		/// <summary>
		/// 清空流程定义缓存
		/// </summary>
		/// <returns></returns>
		public void ClearDefinitionCache() {
			WorkflowCache.Clear();
		}

		/// <summary>
		/// 创建流程
		/// </summary>
		/// <param name="appCode">流程编码</param>
		/// <returns></returns>
		public abstract WorkflowInstance CreateWorkflow(int appCode);

		/// <summary>
		/// 加载流程
		/// </summary>
		/// <param name="appCode">流程编码</param>
		/// <param name="instanceNo">流程编号</param>
		/// <param name="taskId">当前任务序号</param>
		/// <returns></returns>
		public abstract WorkflowInstance LoadWorkflow(int appCode, string instanceNo, int taskId);

		/// <summary>
		/// 加载流程
		/// </summary>
		/// <param name="instanceNo">流程编号</param>
		/// <param name="taskId">当前任务序号</param>
		/// <returns></returns>
		public abstract WorkflowInstance LoadWorkflow(string instanceNo, int taskId);

		/// <summary>
		/// 是否可以查看流程
		/// </summary>
		public abstract bool CanViewWorkflow(WorkflowInstance instance);

		/// <summary>
		/// 保存流程
		/// </summary>
		/// <param name="instance">流程实例</param>
		/// <returns></returns>
		/// <remarks>
		/// 持久化流程实例及流程流转过程数据
		/// 1、根据InstanceNo和taskId获取流程信息
		/// 2、如果流程不存在，则创建新的流程数据（WorkflowInstance和WorkflowItem)
		/// 3、如果流程已存在，则更新相关流程数据（WorkflowInstance和WorkflowItem)
		/// ===============================================================
		/// 调用K2接口
		/// </remarks>
		public abstract bool SaveWorkflow(WorkflowInstance instance);

		/// <summary>
		/// 运行流程
		/// </summary>
		/// <param name="instance">流程编号</param>
		/// <param name="result">审批结果</param>
		/// <returns></returns>
		/// <remarks>
		/// 根据用户选择的下一步骤，计算出下一环节的参与者并持久化到数据库
		/// 1、检查流程数据的合法性（及权限）
		/// 2、根据Choice获取下一环节的定义（名称）
		/// 3、结束当前WorkflowItem
		/// 4、添加下一环节处理人的WorkItem（s）数据
		/// =========================================================
		/// 调用K2接口
		/// </remarks>
		public abstract bool RunWorkflow(WorkflowInstance instance, ApproveResult result);

		/// <summary>
		/// 删除流程
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public abstract bool DeleteWorkflow(WorkflowInstance instance);

		/// <summary>
		/// 流程签收
		/// </summary>
		/// <returns><c>true</c>, 签收成功, <c>false</c> 签收失败.</returns>
		/// <param name="instanceNo">流程实例编号</param>
		/// <param name="taskId">当前任务id</param>
		public abstract void SignWorkflow(string instanceNo, int taskId);

	}
}
