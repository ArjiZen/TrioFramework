using System;
using System.Linq;
using System.Collections.Generic;
using Bingosoft.Data;
using Bingosoft.Data.Attributes;

namespace Bingosoft.TrioFramework.Workflow.Core.Models {

	/// <summary>
	/// 任务状态
	/// </summary>
	public enum TaskStatus {
		/// <summary>
		/// 待处理
		/// </summary>
		Waiting = 0,
		/// <summary>
		/// 待阅
		/// </summary>
		ToRead = 4,
		/// <summary>
		/// 同意
		/// </summary>
		Accept = 1,
		/// <summary>
		/// 退回
		/// </summary>
		Reject = 2,
		/// <summary>
		/// 系统自动处理
		/// </summary>
		Finished = 3
	}

	/// <summary>
	/// 流程流转过程
	/// </summary>
	[Table("WF_WorkflowItem")]
	public abstract class WorkflowItem {

		#region Properties

		/// <summary>
		/// 任务编号
		/// </summary>
	    [PrimaryKey]
		public int TaskId { get; set; }

		/// <summary>
		/// 实例编号
		/// </summary>
		[PrimaryKey]
		public string InstanceNo { get; set; }

		/// <summary>
		/// 审批人Id
		/// </summary>
		public string PartId { get; set; }

		/// <summary>
		/// 审批人名称
		/// </summary>
		public string PartName { get; set; }

		/// <summary>
		/// 审批人所在部门Id
		/// </summary>
		public string PartDeptId { get; set; }

		/// <summary>
		/// 审批人所在部门名称
		/// </summary>
		public string PartDeptName { get; set; }

		/// <summary>
		/// 接收时间
		/// </summary>
		public DateTime ReceTime { get; set; }

		/// <summary>
		/// 已读时间
		/// </summary>
		public DateTime? ReadTime { get; set; }

		/// <summary>
		/// 签收时间
		/// </summary>
		public DateTime? SignTime { get; set; }

		/// <summary>
		/// 是否已签收
		/// </summary>
		public bool IsSign { get; set; }

		/// <summary>
		/// 审批时间
		/// </summary>
		public DateTime? FinishTime { get; set; }

		/// <summary>
		/// 是否为自动完成（一般用于多人审批）
		/// </summary>
		public bool AutoFinished { get; set; }

		/// <summary>
		/// 环节状态
		/// </summary>
		public TaskStatus TaskStatus { get; set; }

		/// <summary>
		/// 审批意见
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// 当前环节名称
		/// </summary>
		public string CurrentActi { get; set; }

		/// <summary>
		/// 用户选择处理结果
		/// </summary>
		/// <remarks>
		/// 同意/通过 => 1
		/// 不同意/退回/拒绝 => 2
		/// (AutoFinished == true) => 3
		/// 传阅 => 4
		/// </remarks>
		public string Choice { get; set; }

		/// <summary>
		/// 被委托人Id
		/// </summary>
		public string MandataryId { get; set; }

		/// <summary>
		/// 被委托人
		/// </summary>
		public string Mandatary { get; set; }

		/// <summary>
		/// 中途意见
		/// </summary>
		public MidwayOpinion[] MidwayOpinions { get; set; }

		#endregion

		/// <summary>
		/// 更新
		/// </summary>
		public abstract void Update();
	}

	/// <summary>
	/// 流程项工厂类
	/// </summary>
	public static class WorkflowItemFactory {

		/// <summary>
		/// 创建新的流程项
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T Create<T>() where T : WorkflowItem {
			return Activator.CreateInstance<T>();
		}

		/// <summary>
		/// 获取该流程的所有流程项
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instanceNo">流程实例编号</param>
		/// <returns></returns>
		public static IList<T> GetAll<T>(string instanceNo) where T : WorkflowItem, new() {
			var type = typeof(T);
			var attr = type.GetFirstAttr<TableAttribute>();
			var sql = string.Format("SELECT * FROM {0} WHERE InstanceNo = @InstanceNo", attr.Name);
			var workitems = DBFactory.WorkflowDB.QueryEntities<T>(sql, new { InstanceNo = instanceNo });
			LoadMidwayOpinions(instanceNo, workitems);
			return workitems;
		}

		/// <summary>
		/// 获取流程项
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instanceNo">流程实例编号</param>
		/// <param name="taskId">任务编号</param>
		/// <returns></returns>
		public static T Get<T>(string instanceNo, int taskId) where T : WorkflowItem, new() {
			var type = typeof(T);
			var attr = type.GetFirstAttr<TableAttribute>();
			var sql = string.Format("SELECT * FROM {0} WHERE InstanceNo = @InstanceNo AND TaskId = @TaskId", attr.Name);
			return DBFactory.WorkflowDB.QueryEntity<T>(sql, new { InstanceNo = instanceNo, TaskId = taskId });
		}

		/// <summary>
		/// 获取该流程中该环节的流程项
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instanceNo">流程实例编号</param>
		/// <param name="activityName">环节名称</param>
		/// <returns></returns>
		public static IEnumerable<T> GetAll<T>(string instanceNo, string activityName) where T : WorkflowItem, new() {
			var type = typeof(T);
			var attr = type.GetFirstAttr<TableAttribute>();
			var sql = string.Format("SELECT * FROM {0} WHERE InstanceNo = @InstanceNo AND CurrentActi = @CurrentActi AND FinishTime IS NULL", attr.Name);
			return DBFactory.WorkflowDB.QueryEntities<T>(sql, new { InstanceNo = instanceNo, CurrentActi = activityName });
		}

		/// <summary>
		/// 加载中途意见
		/// </summary>
		/// <param name="instanceNo">流程实例编号.</param>
		/// <param name="workitems">流程工作项.</param>
		/// <typeparam name="T"></typeparam>
		private static void LoadMidwayOpinions<T>(string instanceNo, IEnumerable<T> workitems) where T : WorkflowItem, new() {
			var midwayOpinions = MidwayOpinion.GetAll(instanceNo);
			foreach (var item in workitems) {
				item.MidwayOpinions = midwayOpinions.Where(p => p.TaskId == item.TaskId && p.InstanceNo.Equals(item.InstanceNo, StringComparison.OrdinalIgnoreCase)).OrderBy(p => p.TaskId).ToArray();
			}
		}

	}

	/// <summary>
	/// WorkItem比较器
	/// </summary>
	public class WorkflowItemEqualityComparer : IEqualityComparer<WorkflowItem> {
		/// <summary>
		/// 比较WorkItem
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(WorkflowItem x, WorkflowItem y) {
			if (x.CurrentActi == y.CurrentActi) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int GetHashCode(WorkflowItem obj) {
			return obj.GetHashCode();
		}
	}
}
