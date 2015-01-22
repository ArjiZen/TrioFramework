using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bingosoft.Data;
using Bingosoft.Data.Attributes;
using Bingosoft.TrioFramework.Workflow.Core.Exceptions;

namespace Bingosoft.TrioFramework.Workflow.Core.Models {

	/// <summary>
	/// 流程实例状态
	/// </summary>
	public enum InstanceStatus {
		/// <summary>
		/// 草稿
		/// </summary>
		Draft = 0,
		/// <summary>
		/// 运行中
		/// </summary>
		Running = 1,
		/// <summary>
		/// 完成
		/// </summary>
		Finished = 2,
		/// <summary>
		/// 作废（用户删除）
		/// </summary>
		Cancel = 3,
		/// <summary>
		/// 删除
		/// </summary>
		Deleted = 4
	}

	/// <summary>
	/// 流程实例
	/// </summary>
	[Table("WF_WorkflowInstance")]
	public abstract class WorkflowInstance {

		#region Properties

		/// <summary>
		/// 流程编号
		/// </summary>
		public int AppCode { get; set; }

		/// <summary>
		/// 流程名称
		/// </summary>
		public string AppName { get; set; }

		/// <summary>
		/// 流程说明
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 流程版本号
		/// </summary>
		public int Version { get; set; }

		/// <summary>
		/// 流程实例编号
		/// </summary>
		[PrimaryKey]
		public string InstanceNo { get; set; }

		/// <summary>
		/// 流程标题
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// 开始时间
		/// </summary>
		public DateTime StartTime { get; set; }

		/// <summary>
		/// 创建人Id
		/// </summary>
		public string CreatorId { get; set; }

		/// <summary>
		/// 创建人
		/// </summary>
		public string Creator { get; set; }

		/// <summary>
		/// 创建人所在部门Id
		/// </summary>
		public string CreatorDeptId { get; set; }

		/// <summary>
		/// 创建人所在部门名称
		/// </summary>
		public string CreatorDeptName { get; set; }

		/// <summary>
		/// 结束时间
		/// </summary>
		public DateTime? EndTime { get; set; }

		/// <summary>
		/// 流程状态
		/// </summary>
		public InstanceStatus Status { get; set; }

		/// <summary>
		/// 当前环节名称
		/// </summary>
		public string CurrentActivity { get; set; }

		/// <summary>
		/// K2流程编号
		/// </summary>
		public string DataLocator { get; set; }

		/// <summary>
		/// 当前环节数据
		/// </summary>
		public WorkflowItem CurrentWorkItem { get; set; }

		#endregion

		private static readonly Dao _dao = Dao.Get();

		/// <summary>
		/// 获取流程第一个环节
		/// </summary>
		/// <returns></returns>
		public WorkflowActivity GetFirstActi() {
			var definition = WorkflowCache.Definitions.FirstOrDefault(p => p.AppCode == this.AppCode && p.Version == this.Version);
			if (definition == null) {
				throw new WorkflowDefinitionNotExistsException(this.AppCode, this.Version);
			}
			if (definition.Activities.Count == 0) {
				throw new WorkflowActivityNotExistsException(this.AppCode, this.Version, this.CurrentActivity);
			}
			return definition.Activities[0];
		}

		/// <summary>
		/// 获取当前流程环节
		/// </summary>
		/// <returns></returns>
		public WorkflowActivity GetCurrentActi() {
			var definition = WorkflowCache.Definitions.FirstOrDefault(p => p.AppCode == this.AppCode && p.Version == this.Version);
			if (definition == null) {
				throw new WorkflowDefinitionNotExistsException(this.AppCode, this.Version);
			}
			var activity = definition.Activities.FirstOrDefault(p => p.Name == this.CurrentActivity);
			if (activity == null) {
				throw new WorkflowActivityNotExistsException(this.AppCode, this.Version, this.CurrentActivity);
			}
			return activity;
		}

		/// <summary>
		/// 获取流程环节
		/// </summary>
		/// <param name="activityName">环节名</param>
		/// <returns></returns>
		public WorkflowActivity GetActi(string activityName) {
			var definition = WorkflowCache.Definitions.FirstOrDefault(p => p.AppCode == this.AppCode && p.Version == this.Version);
			if (definition == null) {
				throw new WorkflowDefinitionNotExistsException(this.AppCode, this.Version);
			}
			var activity = definition.Activities.FirstOrDefault(p => p.Name == activityName);
			if (activity == null) {
				throw new WorkflowActivityNotExistsException(this.AppCode, this.Version, this.CurrentActivity);
			}
			return activity;
		}

		/// <summary>
		/// 获取历史审批记录
		/// </summary>
		/// <returns></returns>
		public abstract IEnumerable<WorkflowItem> GetWorkItems();

		private readonly static object lockObj = new object();

		/// <summary>
		/// 获取新的流程实例编码
		/// </summary>
		/// <returns></returns>
		public static string GetNewInstanceNo() {
			lock (lockObj) {
				var today = DateTime.Today.ToString("yyyyMMdd");
				var instanceNo = _dao.QueryScalar<string>("trio.workflow.core.instance.getmaxno", new {Today = today});
				if (string.IsNullOrEmpty(instanceNo)) {
					instanceNo = today + "00001";
				} else {
					var day = instanceNo.Substring(0, 8);
					var no = instanceNo.Substring(8);
					if (!today.Equals(day, StringComparison.OrdinalIgnoreCase)) {
						day = today;
						no = "00001";
					} else {
						var iNo = Int32.Parse(no);
						no = (iNo + 1).ToString(CultureInfo.InvariantCulture).PadLeft(5, '0');
					}
					instanceNo = day + no;
				}
				return instanceNo;
			}
		}
	}

	/// <summary>
	/// 流程实例工厂类
	/// </summary>
	public static class WorkflowInstanceFactory {

		private readonly static Dao _dao = Dao.Get();

		/// <summary>
		/// 获取流程实例
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instanceNo">流程实例编号</param>
		/// <returns></returns>
		public static T Get<T>(string instanceNo) where T : WorkflowInstance, new() {
			return _dao.Select<T>(instanceNo);
		}

		/// <summary>
		/// 创建新的流程实例
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T Create<T>() where T : WorkflowInstance {
			return Activator.CreateInstance<T>();
		}

		/// <summary>
		/// 检查流程实例是否已存在
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instanceNo">流程实例编号</param>
		/// <returns></returns>
		public static bool IsExists<T>(string instanceNo) where T : WorkflowInstance, new() {
			return _dao.Exists<T>(instanceNo);
		}
	}
}
