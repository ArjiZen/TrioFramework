using System;
using System.Collections.Generic;
using Bingosoft.Data;
using Bingosoft.TrioFramework.Workflow.K2Client.Models;

namespace Bingosoft.TrioFramework.WindowsServices {

	/// <summary>
	/// 推送待办
	/// </summary>
	public class PushJobTask : TrioTask<PendingJob> {

		private readonly static Dao _dao = Dao.Get();

		/// <summary>
		/// 获取任务名称
		/// </summary>
		protected override string TaskName {
			get {
				return "推送待办";
			}
		}

		/// <summary>
		/// 内部重载获取任务方法
		/// </summary>
		/// <returns>The tasks.</returns>
		internal override IList<PendingJob> LoadTasks() {
			var jobs = _dao.QueryEntities<PendingJob>("trio.winservices.pendingjob.get.dopush");
			return jobs;
		}

		/// <summary>
		/// 推送待办
		/// </summary>
		/// <param name="task">任务项</param>
		/// <returns><c>true</c>, if execute was internaled, <c>false</c> otherwise.</returns>
		internal override bool InternalExecute(PendingJob task) {
			if (task == null) {
				return false;
			}
			var businessNo = task.InstanceNo + "|" + task.TaskId;
			try {
				var jobId = "";
				var isSuccess = false;
				if (SettingProvider.PendingJob.IsEnabled) {
					var creatorId = task.CreatorId;
					var creator = task.Creator;
					if (string.IsNullOrEmpty(creatorId) || string.IsNullOrEmpty(creator)) {
						// 兼容旧数据，读取流程建单人
						using (var reader = _dao.QueryReader("trio.winservices.pendingjob.get.creator", new {InstanceNo = task.InstanceNo})) {
							if (reader.Read()) {
								creatorId = reader["CreatorId"].ToString();
								creator = reader["Creator"].ToString();
								reader.Close();
							}
						}
					}

					var client = new PendingJobProvider.PendingJobII();
					isSuccess = client.AddPendingJob(task.UserId, 
						ref jobId, task.JobTitle, task.Url, creatorId, creator, businessNo, 
						SettingProvider.Common.SystemId, SettingProvider.Common.SystemName, 
						"", SettingProvider.PendingJob.IsEnabledSMS ? 1 : 0, "", "", 0);
				} else {
					jobId = "TEST_JOBID_" + DateTime.Now.ToString("yyyyMMddHHmmss");
					isSuccess = true;
				}
				if (isSuccess) {
					task.JobId = jobId;
					task.PushSuccess();
					base.Log(businessNo, true, "待办推送成功");
					return true;
				} else {
					throw new InvalidOperationException("接口返回：" + isSuccess + " 未知原因");
				}
			} catch (Exception ex) {
				task.Result = "待办推送失败：" + ex.GetAll();
				task.PendingFailure();
				base.Log(businessNo, false, "待办推送失败", task.Result);
				return false;
			}
		}
	}
}

