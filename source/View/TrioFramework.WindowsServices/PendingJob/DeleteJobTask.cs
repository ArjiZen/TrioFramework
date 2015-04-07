using System;
using System.Collections.Generic;
using Bingosoft.Data;
using Bingosoft.TrioFramework.Workflow.K2Client.Models;

namespace Bingosoft.TrioFramework.WindowsServices {

	/// <summary>
	/// 删除待办任务
	/// </summary>
	public class DeleteJobTask : TrioTask<PendingJob> {

		private readonly static Dao _dao = Dao.Get();

		/// <summary>
		/// 获取任务名称
		/// </summary>
		protected override string TaskName {
			get {
				return "删除待办";
			}
		}

		/// <summary>
		/// 内部重载获取待删除任务方法
		/// </summary>
		/// <returns>The tasks.</returns>
		internal override IList<PendingJob> LoadTasks() {
			var jobs = _dao.QueryEntities<PendingJob>("trio.winservices.pendingjob.get.dodelete");
			return jobs;
		}

		/// <summary>
		/// 删除待办
		/// </summary>
		/// <param name="task">任务项</param>
		internal override bool InternalExecute(PendingJob task) {
			if (task == null) {
				return false;
			}
			var businessNo = task.InstanceNo + "|" + task.TaskId;
			try {
				var client = new PendingJobProvider.PendingJobII();
				var isSuccess = false;
				if (SettingProvider.PendingJob.IsEnabled) {
					isSuccess = client.FinishPendingJobByJobIDs(task.JobId);
				} else {
					isSuccess = true;
				}
				if (isSuccess) {
					task.DeleteSuccess();
					base.Log(businessNo, true, "任务删除成功");
					return true;
				} else {
					throw new InvalidOperationException("接口返回：" + isSuccess + " 未知原因");
				}
			} catch (Exception ex) {
				task.Result = "任务删除失败：" + ex.GetAllMessage();
				task.PendingFailure();
				base.Log(businessNo, false, "任务删除失败", task.Result);
				return false;
			}
		}
	}
}

