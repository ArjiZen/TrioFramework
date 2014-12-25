using System;
using NUnit.Framework;
using Bingosoft.TrioFramework.Workflow.Core;
using Bingosoft.Security;
using Bingosoft.TrioFramework.Workflow.Core.Models;
using System.Collections.Generic;
using Bingosoft.Data;
using System.Reflection;
using Bingosoft.Security.Principal;

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

			WorkflowEngine.Instance.CurrentUser = loginUser;
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
			instance.Title = "（单元测试）" + DateTime.Now.ToString("yyyyMMddHHmm");

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

		[Test()]
		public void SignWithExistsTest(){
			var assertSignTime = DateTime.Parse("2014-12-24 11:53:16.510");

			var instance2 = WorkflowEngine.Instance.LoadWorkflow("2014122400001", 2);
			var workitem2 = instance2.CurrentWorkItem;
			Assert.IsNotNull(instance2);
			Assert.IsNotNull(workitem2);
			Assert.IsTrue(workitem2.SignTime.HasValue);
			Assert.IsTrue(workitem2.SignTime.Value >= assertSignTime.AddSeconds(-5) && workitem2.SignTime.Value <= assertSignTime.AddSeconds(5));
		}

	}
}

