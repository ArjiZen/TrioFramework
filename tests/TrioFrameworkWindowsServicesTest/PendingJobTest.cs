using NUnit.Framework;
using System;

namespace Bingosoft.TrioFramework.WindowsServices.Test {

	/// <summary>
	/// 推送待办测试
	/// </summary>
	[TestFixture()]
	public class PendingJobTest {
	
		[Test()]
		public void PendingTodoTest() {
			var task = new PushJobTask();
			var tasks = task.LoadTasks();
			Assert.AreNotEqual(0, tasks.Count);

			var pushResult = task.InternalExecute(tasks[0]);
			Assert.IsTrue(pushResult);
		}


		[Test()]
		public void FinishTodoTest() {
			var task = new FinishJobTask();
			var tasks = task.LoadTasks();
			Assert.AreNotEqual(0, tasks.Count);

			var pushResult = task.InternalExecute(tasks[0]);
			Assert.IsTrue(pushResult);
		}

		[Test()]
		public void DeleteTodoTest() {
			var task = new DeleteJobTask();
			var tasks = task.LoadTasks();
			Assert.AreNotEqual(0, tasks.Count);

			var pushResult = task.InternalExecute(tasks[0]);
			Assert.IsTrue(pushResult);
		}
	}

}

