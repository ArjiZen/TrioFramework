using System;
using NUnit.Framework;
using Bingosoft.TrioFramework.Workflow.Core;
using Bingosoft.Security;
using Bingosoft.TrioFramework.Workflow.Core.Models;
using System.Collections.Generic;
using Bingosoft.Data;
using System.Reflection;
using Bingosoft.Security.Principal;
using Bingosoft.TrioFramework.Workflow.K2Client.Models;

namespace Bingosoft.TrioFramework.Workflow.K2Client.Test {
	[TestFixture()]
	public class WorkflowEngineTest {

		private IUser loginUser = null;
		private string instanceNo = null;

		[TestFixtureSetUp()]
		public void Setup() {
			Assembly.Load("Bingosoft.TrioFramework");

			loginUser = SecurityContext.Provider.GetUser("songshuang");
			Assert.IsNotNull(loginUser);
			Assert.AreEqual("宋爽", loginUser.Name);
			Assert.IsNotEmpty(loginUser.DeptId);

			WorkflowEngine.Instance.SetCurrentUser("songshuang");
		}

		[TestFixtureTearDown()]
		public void TearDown() {
			var instance = WorkflowEngine.Instance.LoadWorkflow(instanceNo, 2);
			var deleteResult = WorkflowEngine.Instance.DeleteWorkflow(instance);
			Assert.IsTrue(deleteResult);
		}

		/// <summary>
		/// 流程签收测试
		/// </summary>
		[Test()]
		public void SignTest() {
			var department = SecurityContext.Provider.GetOrganization(loginUser.DeptId);
			Assert.IsNotNull(department);

			var instance = WorkflowEngine.Instance.CreateWorkflow(1);
			Assert.IsNotNull(instance);

			instance.Creator = loginUser.Name;
			instance.CreatorId = loginUser.Id;
			instance.CreatorDeptId = department.Id;
			instance.CreatorDeptName = department.FullName;
			instance.Title = "（签收单元测试）" + DateTime.Now.ToString("yyyyMMddHHmm");

			var saveResult = WorkflowEngine.Instance.SaveWorkflow(instance);
			Assert.IsTrue(saveResult);

			var approveResult = new ApproveResult();
			approveResult.Choice = "营销接口审核";
			approveResult.Comment = "通过";
			approveResult.NextUsers = new List<string>(){ loginUser.Id };

			var runResult = WorkflowEngine.Instance.RunWorkflow(instance, approveResult);
			Assert.IsTrue(runResult);

			instanceNo = instance.InstanceNo;

			WorkflowEngine.Instance.SignWorkflow(instance.InstanceNo, 2);
			var assertSignTime = DateTime.Now;

			var instance2 = WorkflowEngine.Instance.LoadWorkflow(instance.InstanceNo, 2);
			var workitem2 = instance2.CurrentWorkItem;
			Assert.IsNotNull(instance2);
			Assert.IsNotNull(workitem2);
			Assert.AreEqual(instance.Title, instance2.Title);
			Assert.IsTrue(workitem2.SignTime.HasValue);
			Assert.IsTrue(workitem2.SignTime.Value >= assertSignTime.AddSeconds(-5) && workitem2.SignTime.Value <= assertSignTime.AddSeconds(5));

		}

		/// <summary>
		/// 已存在流程签收测试
		/// </summary>
		[Test()]
		public void SignWithExistsTest() {
			var assertSignTime = DateTime.Parse("2014-12-24 11:53:16.510");

			var instance2 = WorkflowEngine.Instance.LoadWorkflow("2014122400001", 2);
			var workitem2 = instance2.CurrentWorkItem;
			Assert.IsNotNull(instance2);
			Assert.IsNotNull(workitem2);
			Assert.IsTrue(workitem2.SignTime.HasValue);
			Assert.IsTrue(workitem2.SignTime.Value >= assertSignTime.AddSeconds(-5) && workitem2.SignTime.Value <= assertSignTime.AddSeconds(5));
		}

		/// <summary>
		/// 提交流程时推送待办测试
		/// </summary>
		[Test()]
		public void PendingJobTodoTest() {
			var department = SecurityContext.Provider.GetOrganization(loginUser.DeptId);
			Assert.IsNotNull(department);

			var instance = WorkflowEngine.Instance.CreateWorkflow(1);
			Assert.IsNotNull(instance);

			instance.Creator = loginUser.Name;
			instance.CreatorId = loginUser.Id;
			instance.CreatorDeptId = department.Id;
			instance.CreatorDeptName = department.FullName;
			instance.Title = "（待办单元测试）" + DateTime.Now.ToString("yyyyMMddHHmm");

			var saveResult = WorkflowEngine.Instance.SaveWorkflow(instance);
			Assert.IsTrue(saveResult);

			var approveResult = new ApproveResult();
			approveResult.Choice = "营销接口审核";
			approveResult.Comment = "通过";
			approveResult.NextUsers = new List<string>(){ loginUser.Id };

			var runResult = WorkflowEngine.Instance.RunWorkflow(instance, approveResult);
			Assert.IsTrue(runResult);

			var job = PendingJob.Get(instance.InstanceNo, 2);
			Assert.IsTrue(job.DoPush);
			Assert.IsNull(job.PushedTime);
		}

		/// <summary>
		/// 删除流程时删除待办测试
		/// </summary>
		[Test()]
		public void PendingJobDeleteTest() {
			var department = SecurityContext.Provider.GetOrganization(loginUser.DeptId);
			Assert.IsNotNull(department);

			var instance = WorkflowEngine.Instance.CreateWorkflow(1);
			Assert.IsNotNull(instance);

			instance.Creator = loginUser.Name;
			instance.CreatorId = loginUser.Id;
			instance.CreatorDeptId = department.Id;
			instance.CreatorDeptName = department.FullName;
			instance.Title = "（待办单元测试）" + DateTime.Now.ToString("yyyyMMddHHmm");

			var saveResult = WorkflowEngine.Instance.SaveWorkflow(instance);
			Assert.IsTrue(saveResult);

			var approveResult = new ApproveResult();
			approveResult.Choice = "营销接口审核";
			approveResult.Comment = "通过";
			approveResult.NextUsers = new List<string>(){ loginUser.Id };

			var runResult = WorkflowEngine.Instance.RunWorkflow(instance, approveResult);
			Assert.IsTrue(runResult);

			var job = PendingJob.Get(instance.InstanceNo, 2);
			Assert.IsTrue(job.DoPush);
			Assert.IsNull(job.PushedTime);

			var instance2 = WorkflowEngine.Instance.LoadWorkflow(instance.InstanceNo, 2);
			Assert.IsNotNull(instance2);

			var delResult = WorkflowEngine.Instance.DeleteWorkflow(instance2);
			Assert.IsTrue(delResult);

			var job2 = PendingJob.Get(instance.InstanceNo, 2);
			Assert.IsTrue(job2.DoDelete);
			Assert.IsNull(job2.DeletedTime);
		}
			
		/// <summary>
		/// 提交流程时完成待办测试
		/// </summary>
		[Test()]
		public void PendingJobFinishTest() {
			var department = SecurityContext.Provider.GetOrganization(loginUser.DeptId);
			Assert.IsNotNull(department);

			var instance = WorkflowEngine.Instance.CreateWorkflow(1);
			Assert.IsNotNull(instance);

			instance.Creator = loginUser.Name;
			instance.CreatorId = loginUser.Id;
			instance.CreatorDeptId = department.Id;
			instance.CreatorDeptName = department.FullName;
			instance.Title = "（待办单元测试）" + DateTime.Now.ToString("yyyyMMddHHmm");

			var saveResult = WorkflowEngine.Instance.SaveWorkflow(instance);
			Assert.IsTrue(saveResult);

			// to 营销接口审核、评估
			var approveResult = new ApproveResult();
			approveResult.Choice = "营销接口审核";
			approveResult.Comment = "通过";
			approveResult.NextUsers = new List<string>(){ loginUser.Id };

			var runResult = WorkflowEngine.Instance.RunWorkflow(instance, approveResult);
			Assert.IsTrue(runResult);

			var job = PendingJob.Get(instance.InstanceNo, 2);
			Assert.IsTrue(job.DoPush);
			Assert.IsNull(job.PushedTime);

			var instance2 = WorkflowEngine.Instance.LoadWorkflow(instance.InstanceNo, 2);
			Assert.IsNotNull(instance2);

			// to 需求组长审核、评估
			var approveResult2 = new ApproveResult();
			approveResult2.Choice = "通过";
			approveResult2.Comment = "通过";
			approveResult2.NextUsers = new List<string>(){ loginUser.Id };

			var runResult2 = WorkflowEngine.Instance.RunWorkflow(instance2, approveResult2);
			Assert.IsTrue(runResult2);

			var job2 = PendingJob.Get(instance.InstanceNo, 2);
			Assert.IsTrue(job2.DoFinish);
			Assert.IsNull(job2.FinishedTime);

			var job3 = PendingJob.Get(instance.InstanceNo, 3);
			Assert.IsTrue(job3.DoPush);
			Assert.IsNull(job3.PushedTime);

			var instance3 = WorkflowEngine.Instance.LoadWorkflow(instance.InstanceNo, 3);
			Assert.IsNotNull(instance3);

			var delResult = WorkflowEngine.Instance.DeleteWorkflow(instance3);
			Assert.IsTrue(delResult);
		}
	}
}

