using System;
using System.Collections.Generic;
using System.Transactions;
using Bingosoft.Data;
using Bingosoft.Security;
using Bingosoft.Security.Principal;
using Bingosoft.TrioFramework.Workflow.Core.Models;
using Bingosoft.TrioFramework.Workflow.K2Client.Models;

namespace Bingosoft.TrioFramework.Workflow.K2Client {
	/// <summary>
	/// 本地服务器调用
	/// </summary>
	public class DbEngine : IK2Engine {

		private readonly static Dao _dao = Dao.Get();

		/// <summary>
		/// 启动流程实例
		/// </summary>
		/// <param name="instance">流程实例对象</param>
		/// <remarks>同时新增流程办理历史</remarks>
		/// <returns></returns>
		public bool SaveWorkflow(WorkflowInstance instance) {
			var ret = 0;
			var isInstExists = WorkflowInstanceFactory.IsExists<K2WorkflowInstance>(instance.InstanceNo);
			if (isInstExists) {
				ret = _dao.UpdateFields<K2WorkflowInstance>(instance, new string[] { "Title", "Status", "EndTime" });
			} else {
				using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
					ret = _dao.Insert<WorkflowInstance>(instance);
					_dao.Insert<K2WorkflowItem>(instance.CurrentWorkItem);   //增加一个新的流程办理信息
					transactionScope.Complete();
				}
			}
			// 流程取消或删除时，删除对应的待办记录
			if (instance.Status == InstanceStatus.Cancel || instance.Status == InstanceStatus.Deleted) {
				var workitems = instance.GetWorkItems();
				var effectRows = 0;
				using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
					foreach (var workitem in workitems) {
						if (!workitem.FinishTime.HasValue) {
							workitem.FinishTime = DateTime.Now;
							workitem.TaskStatus = TaskStatus.Finished;
							effectRows = _dao.UpdateFields<K2WorkflowItem>(workitem, new string[]{ "FinishTime", "TaskStatus" });
							if (effectRows > 0) {
								PendingJob.Delete(workitem.InstanceNo, workitem.TaskId);
							}
						}
					}
					transactionScope.Complete();
				}
			}
			return (ret > 0);
		}

		/// <summary>
		/// 获取最新的流程实例办理编号
		/// </summary>
		/// <param name="instanceNo">流程实例ID</param>
		/// <returns></returns>
		private int GetWorkItemLastTaskId(string instanceNo) {
			return _dao.QueryScalar<int>("K2Client.Instance.GetWorkItemLastTaskId", new { InstanceNo = instanceNo });
		}

		/// <summary>
		/// 运行流程
		/// </summary>
		/// <param name="instance">流程实例</param>
		/// <param name="result">处理结果</param>
		/// <param name="nextStepUsers">下一环节处理人</param>
		/// <returns></returns>
		public bool RunWorkflow(WorkflowInstance instance, ApproveResult result, IList<IUser> nextStepUsers) {
			var nextWorkItems = new List<K2WorkflowItem>();
			IEnumerable<K2WorkflowItem> curWorkItems = new K2WorkflowItem[0];

			var currentActi = instance.GetCurrentActi();
			var nextActi = currentActi.Transitions[result.Choice];

			#region 更新当前环节处理状态

			curWorkItems = WorkflowItemFactory.GetAll<K2WorkflowItem>(instance.InstanceNo, instance.CurrentActivity);
			foreach (var workItem in curWorkItems) {
				if ((workItem.TaskId == instance.CurrentWorkItem.TaskId)) {
					workItem.Comment = result.Comment;
					workItem.AutoFinished = false;
					workItem.Choice = result.Choice;
					// 默认处理结果中有以下关键字的为退回，其他都为通过
					var rejectWords = new[] { "不同意", "退回", "拒绝" };
					var isReject = false;
					foreach (var word in rejectWords) {
						isReject = result.Choice.Contains(word);
						if (isReject) {
							break;
						}
					}
					workItem.TaskStatus = isReject ? TaskStatus.Reject : TaskStatus.Accept;
				} else {
					workItem.AutoFinished = true; //（默认当前办理方式是多选一）
					workItem.TaskStatus = TaskStatus.Finished;
				}
				workItem.FinishTime = DateTime.Now;
			}

			#endregion

			#region 添加下一环节流程项

			var lastTaskId = this.GetWorkItemLastTaskId(instance.InstanceNo);
			foreach (var user in nextStepUsers) {
				var workItem = WorkflowItemFactory.Create<K2WorkflowItem>();
				lastTaskId += 1;
				workItem.TaskId = lastTaskId;
				workItem.InstanceNo = instance.InstanceNo;
				workItem.PartId = user.Id;
				workItem.PartName = user.Name;
				workItem.PartDeptId = user.DeptId;
				var dept = SecurityContext.Provider.GetOrganization(user.DeptId);
				workItem.PartDeptName = (dept == null ? "" : dept.FullName);
				workItem.ReceTime = DateTime.Now;
				workItem.TaskStatus = TaskStatus.Waiting;
				workItem.CurrentActi = nextActi.Name;
				nextWorkItems.Add(workItem);
			}

			#endregion

			instance.CurrentActivity = nextActi.Name;
			if (nextActi.Name.Equals("结束", StringComparison.OrdinalIgnoreCase)) {
				instance.EndTime = DateTime.Now;
				instance.Status = InstanceStatus.Finished;
			}

			using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
				//1.更新当前办理环节及其相关环节
				foreach (var workItem in curWorkItems) {
					_dao.UpdateFields<K2WorkflowItem>(workItem, "FinishTime", "AutoFinished", "Choice", "Comment", "TaskStatus");
				}
				//2.新增下一批办理环节
				foreach (var workItem in nextWorkItems) {
					_dao.Insert<K2WorkflowItem>(workItem);
				}
				//3.更新流程实例
				_dao.UpdateFields<K2WorkflowInstance>(instance, "EndTime", "Status", "CurrentActivity");
				transactionScope.Complete();
			}

			// 更新待办
			using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
				foreach (var workItem in curWorkItems) {
					// 结束待办
					PendingJob.Finish(workItem.InstanceNo, workItem.TaskId);
				}
				//2.新增下一批办理环节
				foreach (var workItem in nextWorkItems) {
					// 增加待办
					PendingJob.Todo(workItem.InstanceNo, workItem.TaskId);
				}
				transactionScope.Complete();
			}

			return true;
		}
	}
}
