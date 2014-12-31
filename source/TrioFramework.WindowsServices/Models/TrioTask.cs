using System;
using System.Data.Linq;
using Bingosoft.Data;
using Bingosoft.Data.Attributes;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bingosoft.TrioFramework.WindowsServices {

	/// <summary>
	/// 任务实体
	/// </summary>
	public abstract class TrioTask<T> {
	
		[Table("SYS_TaskLog")]
		internal class TaskLog {

			private readonly static Dao _dao = Dao.Get();

			#region Properties

			/// <summary>
			/// 任务名
			/// </summary>
			public string TaskName { get; set; }

			/// <summary>
			/// 业务主键编号
			/// </summary>
			public string BusinessNo { get; set; }

			/// <summary>
			/// 任务内容
			/// </summary>
			public string Content { get; set; }

			/// <summary>
			/// 执行时间
			/// </summary>
			public DateTime ExecuteTime { get; set; }

			/// <summary>
			/// 是否执行成功
			/// </summary>
			public bool IsSuccess { get; set; }

			/// <summary>
			/// 错误消息
			/// </summary>
			public string ErrorMessage { get; set; }

			#endregion

			internal bool Log() {
				var effectRow = _dao.Insert(this);
				return effectRow > 0;
			}

		}

		/// <summary>
		/// 获取任务名称
		/// </summary>
		/// <value>The name of the task.</value>
		protected abstract string TaskName { get; }

		/// <summary>
		/// 执行任务（对外公开方法）
		/// </summary>
		public void Execute() {
			Log("", true, content: "开始执行");
			var tasks = this.LoadTasks();
			if (tasks == null) {
				Log("", true, content: "获取需要执行的任务数为0");
			} else {
				Log("", true, content: "获取需要执行的任务数为" + tasks.Count);
			}
			if (tasks != null) {
				foreach (var task in tasks) {
					try {
						this.InternalExecute(task);
						Thread.Sleep(1000);
					} catch (Exception ex) {
						var businessId = JsonConvert.SerializeObject(task);
						Log(businessId, false, content: "执行失败", errorMessage: ex.GetAllMessage());
					}
				}
			}
			Log("", true, content: "执行结束");
		}

		/// <summary>
		/// 内部重载获取任务方法
		/// </summary>
		/// <returns>The tasks.</returns>
		internal abstract IList<T> LoadTasks();

		/// <summary>
		/// 内部重载任务执行方法
		/// </summary>
		/// <param name="task">任务项</param>
		internal abstract bool InternalExecute(T task);

		/// <summary>
		/// 记录日志
		/// </summary>
		/// <param name="businessNo">业务数据编号.</param>
		/// <param name="success">是否执行成功.</param>
		/// <param name="content">日志内容.</param>
		/// <param name="errorMessage">错误信息.</param>
		protected void Log(string businessNo, bool success, string content = "", string errorMessage = "") {
			new TaskLog() {
				TaskName = this.TaskName,
				BusinessNo = businessNo,
				Content = content,
				ExecuteTime = DateTime.Now,
				IsSuccess = success,
				ErrorMessage = errorMessage
			}.Log();
		}
	}
}

