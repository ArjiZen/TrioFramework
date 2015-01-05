using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Bingosoft.Security;
using Bingosoft.Security.Exceptions;
using Bingosoft.Security.Principal;
using Bingosoft.TrioFramework.Workflow.Core;
using Bingosoft.TrioFramework.Workflow.Core.Exceptions;
using Bingosoft.TrioFramework.Workflow.Core.Models;
using Bingosoft.TrioFramework.Workflow.K2Client.Exceptions;
using Bingosoft.TrioFramework.Workflow.K2Client.Models;

namespace Bingosoft.TrioFramework.Workflow.K2Client {
	/// <summary>
	/// K2工作流引擎
	/// </summary>
	public class K2WorkflowEngine : WorkflowEngine {

		/// <summary>
		/// 本地数据库访问
		/// </summary>
		protected IK2Engine m_DbEngine = new DbEngine();
		/// <summary>
		/// K2服务器访问
		/// </summary>
		protected IK2Engine m_ServerEngine = new ServerEngine();

		/// <summary>
		/// 实例化K2工作流引擎
		/// </summary>
		public K2WorkflowEngine() {
			// 用于测试的离线K2模式
			if (!SettingProvider.Workflow.IsConnectK2) {
				this.m_ServerEngine = new OfflineServerEngine();
			}
		}

		/// <summary>
		/// 数据库接口
		/// </summary>
		protected IK2Engine DbEngine {
			get {
				if (m_DbEngine != null) {
					m_DbEngine.CurrentUser = this.CurrentUser;
				}
				return m_DbEngine;
			}
		}

		/// <summary>
		/// K2服务器接口
		/// </summary>
		protected IK2Engine ServerEngine {
			get { 
				if (m_ServerEngine != null) {
					m_ServerEngine.CurrentUser = this.CurrentUser;
				}
				return m_ServerEngine;
			}
		}

		/// <summary>
		/// 初始化流程定义，并缓存
		/// </summary>
		/// <returns></returns>
		public override WorkflowDefinition[] LoadDefinitions() {
			var definitions = WorkflowDefinitionFactory.GetAll<K2WorkflowDefinition>();
			foreach (var definition in definitions) {
				definition.InitActivities();
			}
			return definitions.ToArray();
		}

		/// <summary>
		/// 创建工作流
		/// </summary>
		/// <param name="appCode">流程编号</param>
		/// <returns></returns>
		public override WorkflowInstance CreateWorkflow(int appCode) {
			if (CurrentUser == null) {
				throw new UserNotFoundException("未找到登录用户，请先登录本系统");
			}

			var definition = (from e in WorkflowEngine.Definitions
			                  where e.AppCode == appCode
                              orderby e.Version descending
			                  select e).FirstOrDefault();
			if (definition == null) {
				throw new WorkflowDefinitionNotExistsException(appCode, 0);
			}

			var instance = WorkflowInstanceFactory.Create<K2WorkflowInstance>();
			instance.AppCode = appCode;
			instance.AppName = definition.AppName;   // 从流程定义缓存中读取
			instance.Description = definition.Description;
			instance.Version = definition.Version;
			instance.StartTime = DateTime.Now;
			instance.Status = InstanceStatus.Draft;
			instance.Title = "";
			instance.Creator = CurrentUser.Name;
			instance.CreatorId = CurrentUser.Id;
			instance.CurrentActivity = instance.GetFirstActi().Name;
			return instance;
		}

		/// <summary>
		/// 是否可以查看流程
		/// </summary>
		/// <param name="instance">流程实例.</param>
		public override bool CanViewWorkflow(WorkflowInstance instance) {

			// 检查历史环节中当前用户是否参与过流程
			var items = instance.GetWorkItems();
			var hasTakePartIn = items.Any(p => p.PartId.Equals(CurrentUser.Id, StringComparison.OrdinalIgnoreCase)
			                    || (!string.IsNullOrEmpty(p.MandataryId) && p.MandataryId.Equals(CurrentUser.Id, StringComparison.OrdinalIgnoreCase)));

			if (hasTakePartIn) {
				return true;
			}

			// 检查当前环节
			var hasDelegate = DelegateWork.IsDelegate(instance.AppCode, instance.CurrentWorkItem.PartId, CurrentUser.Id);
			if (hasDelegate) {
				return true;
			}

			return false;
		}

		/// <summary>
		/// 加载流程
		/// </summary>
		/// <param name="appCode">流程编号</param>
		/// <param name="instanceNo">流程实例编号</param>
		/// <param name="taskId">任务id</param>
		/// <returns></returns>
		public override WorkflowInstance LoadWorkflow(int appCode, string instanceNo, int taskId) {
			if (CurrentUser == null) {
				throw new UserNotFoundException("未找到登录用户，请先登录本系统");
			}
			var instance = WorkflowInstanceFactory.Get<K2WorkflowInstance>(instanceNo);
			// 从流程定义缓存中读取流程名称
			var definition = (from e in WorkflowEngine.Definitions
			                  where e.AppCode == appCode
                              orderby e.Version descending
			                  select e).FirstOrDefault();
			if (definition == null) {
				throw new WorkflowDefinitionNotExistsException(appCode, 0);
			}
			instance.AppName = definition.AppName;
			instance.Description = definition.Description;
			instance.CurrentWorkItem = WorkflowItemFactory.Get<K2WorkflowItem>(instance.InstanceNo, taskId);

			if (instance.CurrentWorkItem.ReadTime == null && instance.CurrentWorkItem.PartId == CurrentUser.Id) {
				instance.CurrentWorkItem.ReadTime = DateTime.Now;
				instance.CurrentWorkItem.Update();
			}
			return instance;
		}

		/// <summary>
		/// 加载流程
		/// </summary>
		/// <param name="instanceNo">流程实例编号</param>
		/// <param name="taskId">任务id</param>
		/// <returns></returns>
		public override WorkflowInstance LoadWorkflow(string instanceNo, int taskId) {
			if (CurrentUser == null) {
				throw new UserNotFoundException("未找到登录用户，请先登录本系统");
			}
			var instance = WorkflowInstanceFactory.Get<K2WorkflowInstance>(instanceNo);
			// 从流程定义缓存中读取流程名称
			var definition = (from e in WorkflowEngine.Definitions
			                  where e.AppCode == instance.AppCode
                              orderby e.Version descending
			                  select e).FirstOrDefault();
			if (definition == null) {
				throw new WorkflowDefinitionNotExistsException(instance.AppCode, 0);
			}
			instance.AppName = definition.AppName;
			instance.Description = definition.Description;
			instance.CurrentWorkItem = WorkflowItemFactory.Get<K2WorkflowItem>(instance.InstanceNo, taskId);

			if (instance.CurrentWorkItem.ReadTime == null && instance.CurrentWorkItem.PartId == CurrentUser.Id) {
				instance.CurrentWorkItem.ReadTime = DateTime.Now;
				instance.CurrentWorkItem.Update();
			}
			return instance;
		}

		/// <summary>
		/// 保存流程实例
		/// </summary>
		/// <param name="instance">流程实例</param>
		/// <remarks>
		/// 判断对应的 K2流程是否已启动，否则先启动 K2实例，然后保存
		/// </remarks>
		/// <returns></returns>
		public override bool SaveWorkflow(WorkflowInstance instance) {
			if (!string.IsNullOrEmpty(instance.InstanceNo)) {
				var isInstExists = WorkflowInstanceFactory.IsExists<K2WorkflowInstance>(instance.InstanceNo);
				if (isInstExists) {
					// 如果流程已存在，则只更新相关字段
					bool isUpdateInstance = DbEngine.SaveWorkflow(instance);
					return isUpdateInstance;
				}
			}

			if (string.IsNullOrEmpty(instance.InstanceNo)) {
				instance.InstanceNo = WorkflowInstance.GetNewInstanceNo();
			}
			instance.CreatorId = CurrentUser.Id;
			instance.Creator = CurrentUser.Name;
			instance.CreatorDeptId = CurrentUser.DeptId;
			var dept = SecurityContext.Provider.GetOrganization(CurrentUser.DeptId);
			instance.CreatorDeptName = (dept == null ? "" : dept.FullName);
			instance.StartTime = DateTime.Now;
			instance.Status = InstanceStatus.Running;
			if (string.IsNullOrEmpty(instance.CurrentActivity)) {
				var currentActivity = instance.GetFirstActi();
				if (currentActivity != null) {
					instance.CurrentActivity = currentActivity.Name;
				}
			}

			//启动 k2流程办理
			bool isServerStart = ServerEngine.SaveWorkflow(instance);
			if (!isServerStart)
				return false;

			#region 新增初始办理流程信息
			var workItem = WorkflowItemFactory.Create<K2WorkflowItem>();
			workItem.TaskId = 1;
			workItem.InstanceNo = instance.InstanceNo;
			workItem.PartId = instance.CreatorId;
			workItem.PartName = instance.Creator;
			workItem.PartDeptId = instance.CreatorDeptId;
			workItem.PartDeptName = instance.CreatorDeptName;
			workItem.ReceTime = DateTime.Now;
			workItem.TaskStatus = TaskStatus.Waiting;
			workItem.CurrentActi = instance.CurrentActivity;
			instance.CurrentWorkItem = workItem;
			#endregion

			//启动 本地流程实例的保存
			bool isDbStart = DbEngine.SaveWorkflow(instance);
			return isDbStart;
		}

		/// <summary>
		/// 提交流程实例
		/// </summary>
		/// <param name="instance">流程实例</param>
		/// <param name="result">办理操作</param>
		/// <param name="tobeReadUsersId">待阅人员用户Id</param>
		/// <remarks>
		/// 验证当办理人，K2办理成功后维护办理记录，流程实例，新增办理记录
		/// requireFields:
		/// instance.CurrentWorkItem
		/// </remarks>
		/// <returns></returns>
		public override bool RunWorkflow(WorkflowInstance instance, ApproveResult result, string[] tobeReadUsersId = null) {
			if (instance.CurrentWorkItem == null) {
				throw new ActivityNotFoundException(instance.InstanceNo, instance.CurrentActivity);
			}
				
			// 验证当前办理人员
			// 非WorkItem默认用户，非被委托人，未设置委托信息
			if (!instance.CurrentWorkItem.PartId.Equals(CurrentUser.Id, StringComparison.OrdinalIgnoreCase)
			    && (!string.IsNullOrEmpty(instance.CurrentWorkItem.MandataryId)
			    && !instance.CurrentWorkItem.MandataryId.Equals(CurrentUser.Id, StringComparison.OrdinalIgnoreCase))
			    && !DelegateWork.IsDelegate(instance.AppCode, instance.CurrentWorkItem.PartId, CurrentUser.Id)) {

				throw new UserNotFoundException("当前用户不是该环节的处理人");
			}

			// 计算下一环节处理人
			var currentActi = instance.GetCurrentActi();
			var nextActi = currentActi.Transitions[result.Choice];
			if (nextActi == null) {
				throw new ChoiceNotFoundException(instance.InstanceNo, currentActi.Name, result.Choice);
			}

			// 下一步骤人员
			var nextStepUsers = new List<IUser>();
			if (!nextActi.Name.Equals("结束", StringComparison.OrdinalIgnoreCase)) {
				if (result.NextUsers == null) {
					result.NextUsers = nextActi.Actor.Resolve(instance).Select(p => p.Id).ToList();
				}
				if (result.NextUsers == null || result.NextUsers.Count == 0) {
					throw new UserNotFoundException(string.Format("未找到下一步骤{0}的处理人", nextActi.Name));
				}
				nextStepUsers.AddRange(result.NextUsers.Select(userid => SecurityContext.Provider.Get(userid)));
			}

			// 下一步骤待阅人员
			var tobeReadUsers = new List<IUser>();
			if (tobeReadUsersId != null && tobeReadUsersId.Length > 0) {
				tobeReadUsers.AddRange(tobeReadUsersId.Select(userid => SecurityContext.Provider.Get(userid)));
			}

			// 提交K2服务器
			bool isK2Finished = ServerEngine.RunWorkflow(instance, result, nextStepUsers, null);
			if (!isK2Finished) {
				return false;
			}

			// 提交数据库更新
			bool isDbFinished = DbEngine.RunWorkflow(instance, result, nextStepUsers, tobeReadUsers);
			if (!isDbFinished) {
				return false;
			}

			return true;
		}

		/// <summary>
		/// 逻辑删除流程实例
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public override bool DeleteWorkflow(WorkflowInstance instance) {
			instance.Status = InstanceStatus.Cancel;
			instance.EndTime = DateTime.Now;
			return DbEngine.SaveWorkflow(instance);
		}

		/// <summary>
		/// 流程签收
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="instanceNo">流程实例编号</param>
		/// <param name="taskId">当前任务id</param>
		public override void SignWorkflow(string instanceNo, int taskId) {
			var instance = this.LoadWorkflow(instanceNo, taskId);
			if (instance == null) {
				throw new WorkflowException("未找到指定的流程实例，流程编号：" + instanceNo + "，任务编号：" + taskId, null);
			}
			var workitem = instance.CurrentWorkItem;
			workitem.SignTime = DateTime.Now;
			workitem.Update();
		}

	}
}
