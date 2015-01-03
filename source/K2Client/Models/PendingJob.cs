using System;
using Bingosoft.Data;
using Bingosoft.Data.Attributes;
using System.Configuration;
using Bingosoft.TrioFramework.Workflow.Core.Models;
using Bingosoft.TrioFramework.Workflow.K2Client.Models;
using System.Text.RegularExpressions;

namespace Bingosoft.TrioFramework.Workflow.K2Client.Models {
	/// <summary>
	/// 任务推送
	/// </summary>
	[Table("WF_PendingJob")]
	public class PendingJob {

		private static object lockObj = new object();
		private static string pendingJobUrl = "";

		/// <summary>
		/// 获取推送待办的地址
		/// </summary>
		/// <remarks>
		/// 读取配置文件appSettings/PendingJobUrl的值
		/// </remarks>
		private static string PendingJobUrl {
			get {
				if (string.IsNullOrEmpty(pendingJobUrl)) {
					lock (lockObj) {
						if (string.IsNullOrEmpty(pendingJobUrl)) {
							pendingJobUrl = ConfigurationManager.AppSettings["PendingJobUrl"];
						}
					}
				}
				return pendingJobUrl;
			}
		}

		#region Properties

		/// <summary>
		/// 流程实例编号
		/// </summary>
		[PrimaryKey]
		public string InstanceNo { get; set; }

		/// <summary>
		/// 任务编号
		/// </summary>
		[PrimaryKey]
		public int TaskId { get; set; }

		/// <summary>
		/// iOffice任务标示
		/// </summary>
		public string JobId { get; set; }

		/// <summary>
		/// 任务标题
		/// </summary>
		public string JobTitle { get; set; }

		/// <summary>
		/// 任务URL地址
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// 用户Id
		/// </summary>
		public string UserId { get; set; }

		/// <summary>
		/// 用户名
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// 执行结果
		/// </summary>
		public string Result { get; set; }

		/// <summary>
		/// 是否推送
		/// </summary>
		public bool DoPush { get; set; }

		/// <summary>
		/// 是否删除
		/// </summary>
		public bool DoDelete { get; set; }

		/// <summary>
		/// 是否结束待办
		/// </summary>
		public bool DoFinish { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateTime { get; set; }

		/// <summary>
		/// 推送时间
		/// </summary>
		public DateTime? PushedTime { get; set; }

		/// <summary>
		/// 待办删除时间
		/// </summary>
		public DateTime? DeletedTime { get; set; }

		/// <summary>
		/// 待办完成时间
		/// </summary>
		public DateTime? FinishedTime { get; set; }

		#endregion

		private readonly static Dao _dao = Dao.Get();

		/// <summary>
		/// 获取待办任务
		/// </summary>
		/// <param name="instanceNo">流程实例编号.</param>
		/// <param name="taskId">任务编号.</param>
		internal static PendingJob Get(string instanceNo, int taskId) {
			return _dao.QueryEntity<PendingJob>("trio.k2client.pendingjob.get", new {InstanceNo = instanceNo, TaskId = taskId});
		}

		/// <summary>
		/// 添加新待办记录
		/// </summary>
		private bool AddNew() {
			var instance = WorkflowInstanceFactory.Get<K2WorkflowInstance>(this.InstanceNo);
			var url = PendingJobUrl;
			url = Regex.Replace(url, @"\{appCode\}", instance.AppCode.ToString(), RegexOptions.IgnoreCase);
			url = Regex.Replace(url, @"\{instanceNo\}", this.InstanceNo, RegexOptions.IgnoreCase);
			url = Regex.Replace(url, @"\{taskId\}", this.TaskId.ToString(), RegexOptions.IgnoreCase);
			this.Url = url;
			var effectRows = _dao.ExecuteNonQuery("trio.k2client.pendingjob.addtodo", this);
			return effectRows > 0;
		}

		/// <summary>
		/// 完成待办记录
		/// </summary>
		private bool UpdateFinished() {
			var effectRows = _dao.ExecuteNonQuery("trio.k2client.pendingjob.update.finished", this);
			return effectRows > 0;
		}

		/// <summary>
		/// 删除待办记录
		/// </summary>
		private bool UpdateDeleted() {
			var effectRows = _dao.ExecuteNonQuery("trio.k2client.pendingjob.update.delete", this);
			return effectRows > 0;
		}

		/// <summary>
		/// 推送任务成功
		/// </summary>
		internal bool PushSuccess() {
			if (string.IsNullOrEmpty(this.JobId)) {
				throw new ArgumentNullException("JobId不能为空");
			}
			var effectRows = _dao.ExecuteNonQuery("trio.k2client.pendingjob.push.success", this);
			return effectRows > 0;
		}

		/// <summary>
		/// 结束任务成功
		/// </summary>
		internal bool FinishSuccess() {
			var effectRows = _dao.ExecuteNonQuery("trio.k2client.pendingjob.finish.success", this);
			return effectRows > 0;
		}

		/// <summary>
		/// 删除任务成功
		/// </summary>
		internal bool DeleteSuccess() {
			var effectRows = _dao.ExecuteNonQuery("trio.k2client.pendingjob.delete.success", this);
			return effectRows > 0;
		}

		/// <summary>
		/// 推送任务失败
		/// </summary>
		internal bool PendingFailure() {
			if (string.IsNullOrEmpty(this.Result)) {
				throw new ArgumentNullException("Result不能为空");
			}
			var effectRows = _dao.ExecuteNonQuery("trio.k2client.pendingjob.failure", this);
			return effectRows > 0;
		}

		/// <summary>
		/// 推送待办任务
		/// </summary>
		/// <param name="instanceNo">流程编号</param>
		/// <param name="taskid">任务id</param>
		public static bool Todo(string instanceNo, int taskid) {
			var job = new PendingJob() {
				InstanceNo = instanceNo,
				TaskId = taskid
			};
			return job.AddNew();
		}

		/// <summary>
		/// 完成任务
		/// </summary>
		/// <param name="instanceNo">流程编号</param>
		/// <param name="taskId">任务id</param>
		public static bool Finish(string instanceNo, int taskId) {
			var job = PendingJob.Get(instanceNo, taskId);
			if (job != null) {
				//throw new NullReferenceException("未找到待办记录，流程编号：" + instanceNo + "，任务编号：" + taskId);
				return job.UpdateFinished();
			}
			return false;
		}

		/// <summary>
		/// 删除任务记录
		/// </summary>
		/// <param name="instanceNo">流程编号</param>
		/// <param name="taskId">任务id</param>
		public static bool Delete(string instanceNo, int taskId) {
			var job = PendingJob.Get(instanceNo, taskId);
			if (job != null) {
				//throw new NullReferenceException("未找到待办记录，流程编号：" + instanceNo + "，任务编号：" + taskId);
				return job.UpdateDeleted();
			}
			return false;
		}
	
	}
}

