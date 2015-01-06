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
		private WorkflowEngine engine = null;

		[TestFixtureSetUp()]
		public void Setup() {
			Assembly.Load("Bingosoft.TrioFramework");

			loginUser = SecurityContext.Provider.GetUser("songshuang");
			Assert.IsNotNull(loginUser);
			Assert.AreEqual("宋爽", loginUser.Name);
			Assert.IsNotEmpty(loginUser.DeptId);

			engine = WorkflowEngine.Create();
			engine.SetCurrentUser(loginUser.LoginId);
		}

		[TestFixtureTearDown()]
		public void TearDown() {
			if (engine != null) {
				var instance = engine.LoadWorkflow(instanceNo, 2);
				var deleteResult = engine.DeleteWorkflow(instance);
				Assert.IsTrue(deleteResult);
			}
		}

		[Test()]
		public void LoadDefinitionTest(){
			var engine = WorkflowEngine.Create();
			var definitions = engine.LoadDefinitions();
			Assert.AreNotEqual(0, definitions.Length);
		}


		/// <summary>
		/// 流程签收测试
		/// </summary>
		[Test()]
		public void SignTest() {
			Assert.IsNotNull(engine);

			var department = SecurityContext.Provider.GetOrganization(loginUser.DeptId);
			Assert.IsNotNull(department);

			var instance = engine.CreateWorkflow(1);
			Assert.IsNotNull(instance);

			instance.Creator = loginUser.Name;
			instance.CreatorId = loginUser.Id;
			instance.CreatorDeptId = department.Id;
			instance.CreatorDeptName = department.FullName;
			instance.Title = "（签收单元测试）" + DateTime.Now.ToString("yyyyMMddHHmm");

			var saveResult = engine.SaveWorkflow(instance);
			Assert.IsTrue(saveResult);

			var approveResult = new ApproveResult();
			approveResult.Choice = "营销接口审核";
			approveResult.Comment = "通过";
			approveResult.NextUsers = new List<string>(){ loginUser.Id };

			var runResult = engine.RunWorkflow(instance, approveResult);
			Assert.IsTrue(runResult);

			instanceNo = instance.InstanceNo;

			engine.SignWorkflow(instance.InstanceNo, 2);
			var assertSignTime = DateTime.Now;

			var instance2 = engine.LoadWorkflow(instance.InstanceNo, 2);
			var workitem2 = instance2.CurrentWorkItem;
			Assert.IsNotNull(instance2);
			Assert.IsNotNull(workitem2);
			Assert.AreEqual(instance.Title, instance2.Title);
			Assert.IsTrue(workitem2.SignTime.HasValue);
			Assert.IsTrue(workitem2.SignTime.Value >= assertSignTime.AddSeconds(-5) && workitem2.SignTime.Value <= assertSignTime.AddSeconds(5));

		}


		/// <summary>
		/// 多人流程签收测试
		/// </summary>
		[Test()]
		public void MutiSignTest() {
			Assert.IsNotNull(engine);

			var user1 = SecurityContext.Provider.GetUser("liangyanshan");
			var user2 = SecurityContext.Provider.GetUser("zhuyan");

			var department = SecurityContext.Provider.GetOrganization(loginUser.DeptId);
			Assert.IsNotNull(department);

			var instance = engine.CreateWorkflow(1);
			Assert.IsNotNull(instance);

			instance.Creator = loginUser.Name;
			instance.CreatorId = loginUser.Id;
			instance.CreatorDeptId = department.Id;
			instance.CreatorDeptName = department.FullName;
			instance.Title = "（签收单元测试）" + DateTime.Now.ToString("yyyyMMddHHmm");

			var saveResult = engine.SaveWorkflow(instance);
			Assert.IsTrue(saveResult);

			var approveResult = new ApproveResult();
			approveResult.Choice = "营销接口审核";
			approveResult.Comment = "通过";
			approveResult.NextUsers = new List<string>(){ loginUser.Id, user1.Id, user2.Id };

			var runResult = engine.RunWorkflow(instance, approveResult);
			Assert.IsTrue(runResult);

			instanceNo = instance.InstanceNo;

			engine.SignWorkflow(instance.InstanceNo, 2);
			var assertSignTime = DateTime.Now;

			var instance2 = engine.LoadWorkflow(instance.InstanceNo, 2);
			var workitem2 = instance2.CurrentWorkItem;
			Assert.IsNotNull(instance2);
			Assert.IsNotNull(workitem2);
			Assert.AreEqual(instance.Title, instance2.Title);
			Assert.IsTrue(workitem2.IsSign);
			Assert.IsTrue(workitem2.SignTime.HasValue);
			Assert.IsTrue(workitem2.SignTime.Value >= assertSignTime.AddSeconds(-5) && workitem2.SignTime.Value <= assertSignTime.AddSeconds(5));

			engine.SetCurrentUser(user1.LoginId);
			var instance3 = engine.LoadWorkflow(instance.InstanceNo, 3);
			var workitem3 = instance3.CurrentWorkItem;
			Assert.IsNotNull(instance3);
			Assert.IsNotNull(workitem3);
			Assert.IsTrue(workitem3.IsSign);
			Assert.IsFalse(workitem3.SignTime.HasValue);
		}

		/// <summary>
		/// 已存在流程签收测试
		/// </summary>
		[Test()]
		public void SignWithExistsTest() {
			var assertSignTime = DateTime.Parse("2014-12-24 11:53:16.510");

			var instance2 = engine.LoadWorkflow("2014122400001", 2);
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

			var instance = engine.CreateWorkflow(1);
			Assert.IsNotNull(instance);

			instance.Creator = loginUser.Name;
			instance.CreatorId = loginUser.Id;
			instance.CreatorDeptId = department.Id;
			instance.CreatorDeptName = department.FullName;
			instance.Title = "（待办单元测试）" + DateTime.Now.ToString("yyyyMMddHHmm");

			var saveResult = engine.SaveWorkflow(instance);
			Assert.IsTrue(saveResult);

			var approveResult = new ApproveResult();
			approveResult.Choice = "营销接口审核";
			approveResult.Comment = "通过";
			approveResult.NextUsers = new List<string>(){ loginUser.Id };

			var runResult = engine.RunWorkflow(instance, approveResult);
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

			var instance = engine.CreateWorkflow(1);
			Assert.IsNotNull(instance);

			instance.Creator = loginUser.Name;
			instance.CreatorId = loginUser.Id;
			instance.CreatorDeptId = department.Id;
			instance.CreatorDeptName = department.FullName;
			instance.Title = "（待办单元测试）" + DateTime.Now.ToString("yyyyMMddHHmm");

			var saveResult = engine.SaveWorkflow(instance);
			Assert.IsTrue(saveResult);

			var approveResult = new ApproveResult();
			approveResult.Choice = "营销接口审核";
			approveResult.Comment = "通过";
			approveResult.NextUsers = new List<string>(){ loginUser.Id };

			var runResult = engine.RunWorkflow(instance, approveResult);
			Assert.IsTrue(runResult);

			var job = PendingJob.Get(instance.InstanceNo, 2);
			Assert.IsTrue(job.DoPush);
			Assert.IsNull(job.PushedTime);

			var instance2 = engine.LoadWorkflow(instance.InstanceNo, 2);
			Assert.IsNotNull(instance2);

			var delResult = engine.DeleteWorkflow(instance2);
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

			var instance = engine.CreateWorkflow(1);
			Assert.IsNotNull(instance);

			instance.Creator = loginUser.Name;
			instance.CreatorId = loginUser.Id;
			instance.CreatorDeptId = department.Id;
			instance.CreatorDeptName = department.FullName;
			instance.Title = "（待办单元测试）" + DateTime.Now.ToString("yyyyMMddHHmm");

			var saveResult = engine.SaveWorkflow(instance);
			Assert.IsTrue(saveResult);

			// to 营销接口审核、评估
			var approveResult = new ApproveResult();
			approveResult.Choice = "营销接口审核";
			approveResult.Comment = "通过";
			approveResult.NextUsers = new List<string>(){ loginUser.Id };

			var runResult = engine.RunWorkflow(instance, approveResult);
			Assert.IsTrue(runResult);

			var job = PendingJob.Get(instance.InstanceNo, 2);
			Assert.IsTrue(job.DoPush);
			Assert.IsNull(job.PushedTime);

			var instance2 = engine.LoadWorkflow(instance.InstanceNo, 2);
			Assert.IsNotNull(instance2);

			// to 需求组长审核、评估
			var approveResult2 = new ApproveResult();
			approveResult2.Choice = "通过";
			approveResult2.Comment = "通过";
			approveResult2.NextUsers = new List<string>(){ loginUser.Id };

			var runResult2 = engine.RunWorkflow(instance2, approveResult2);
			Assert.IsTrue(runResult2);

			var job2 = PendingJob.Get(instance.InstanceNo, 2);
			Assert.IsTrue(job2.DoFinish);
			Assert.IsNull(job2.FinishedTime);

			var job3 = PendingJob.Get(instance.InstanceNo, 3);
			Assert.IsTrue(job3.DoPush);
			Assert.IsNull(job3.PushedTime);

			var instance3 = engine.LoadWorkflow(instance.InstanceNo, 3);
			Assert.IsNotNull(instance3);

			var delResult = engine.DeleteWorkflow(instance3);
			Assert.IsTrue(delResult);
		}

		/// <summary>
		/// 添加委托关系
		/// </summary>
		/// <param name="delegator">委托人</param>
		/// <param name="mandatary">被委托人</param>
		private void AddDelegateWork(IUser delegator, IUser mandatary){
			Assert.IsNotNull(delegator);
			Assert.IsNotNull(mandatary);

			var delegateWork = new DelegateWork();
			delegateWork.AppCode = 0;
			delegateWork.Delegator = delegator.Name;
			delegateWork.DelegatorId = delegator.Id;
			delegateWork.Mandatary = mandatary.Name;
			delegateWork.MandataryId = mandatary.Id;
			delegateWork.StartTime = DateTime.Now.AddMinutes(-1);
			delegateWork.EndTime = DateTime.Now.AddMinutes(2);
			delegateWork.IsDeleted = false;

			var addSuccess = delegateWork.AddNew();
			Assert.IsTrue(addSuccess);
		}

		/// <summary>
		/// 先设置委托时的流程待办处理，由被委托人处理
		/// </summary>
		[Test()]
		public void Delegate_First_MandataryApproveTest(){
			// 添加委托
			var mandatary = SecurityContext.Provider.GetUser("liangyanshan");
			AddDelegateWork(loginUser, mandatary);

			// 发起流程
			var department = SecurityContext.Provider.GetOrganization(loginUser.DeptId);
			Assert.IsNotNull(department);

			var instance = engine.CreateWorkflow(1);
			Assert.IsNotNull(instance);

			instance.Creator = loginUser.Name;
			instance.CreatorId = loginUser.Id;
			instance.CreatorDeptId = department.Id;
			instance.CreatorDeptName = department.FullName;
			instance.Title = "（委托单元测试）" + DateTime.Now.ToString("yyyyMMddHHmm");

			var saveResult = engine.SaveWorkflow(instance);
			Assert.IsTrue(saveResult);

			var approveResult = new ApproveResult();
			approveResult.Choice = "营销接口审核";
			approveResult.Comment = "通过";
			approveResult.NextUsers = new List<string>(){ loginUser.Id };

			var runResult = engine.RunWorkflow(instance, approveResult);
			Assert.IsTrue(runResult);

			instanceNo = instance.InstanceNo;

			// 第二个环节的WorkItem表已添加被委托人信息
			var instance2 = engine.LoadWorkflow(instance.InstanceNo, 2);
			var workitem2 = instance2.CurrentWorkItem;
			Assert.IsNotNull(instance2);
			Assert.IsNotNull(workitem2);
			Assert.AreEqual(mandatary.Id, workitem2.MandataryId);

			// 设置流程的当前人为被委托人，然后提交流程
			engine.SetCurrentUser(mandatary.LoginId);

			var approveResult2 = new ApproveResult();
			approveResult2.Choice = "通过";
			approveResult2.Comment = "通过";
			approveResult2.NextUsers = new List<string>(){ mandatary.Id };

			var runResult2 = engine.RunWorkflow(instance2, approveResult2);
			Assert.IsTrue(runResult2);

			var instance3 = engine.LoadWorkflow(instance2.InstanceNo, 2);
			var workitem3 = instance3.CurrentWorkItem;
			Assert.IsNotNull(instance3);
			Assert.IsNotNull(workitem3);
			Assert.AreEqual(mandatary.Id, workitem3.MandataryId);

		}
			
		/// <summary>
		/// 先产生待办后设置委托信息，由被委托人处理
		/// </summary>
		[Test()]
		public void Delegate_After_MandataryApproveTest(){
			// 发起流程
			var department = SecurityContext.Provider.GetOrganization(loginUser.DeptId);
			Assert.IsNotNull(department);

			var instance = engine.CreateWorkflow(1);
			Assert.IsNotNull(instance);

			instance.Creator = loginUser.Name;
			instance.CreatorId = loginUser.Id;
			instance.CreatorDeptId = department.Id;
			instance.CreatorDeptName = department.FullName;
			instance.Title = "（委托单元测试）" + DateTime.Now.ToString("yyyyMMddHHmm");

			var saveResult = engine.SaveWorkflow(instance);
			Assert.IsTrue(saveResult);

			var approveResult = new ApproveResult();
			approveResult.Choice = "营销接口审核";
			approveResult.Comment = "通过";
			approveResult.NextUsers = new List<string>(){ loginUser.Id };

			var runResult = engine.RunWorkflow(instance, approveResult);
			Assert.IsTrue(runResult);

			instanceNo = instance.InstanceNo;

			// 第二个环节的WorkItem表已添加被委托人信息
			var instance2 = engine.LoadWorkflow(instance.InstanceNo, 2);
			var workitem2 = instance2.CurrentWorkItem;
			Assert.IsNotNull(instance2);
			Assert.IsNotNull(workitem2);
			Assert.IsNullOrEmpty(workitem2.MandataryId);

			// 添加委托
			var mandatary = SecurityContext.Provider.GetUser("liangyanshan");
			AddDelegateWork(loginUser, mandatary);

			// 设置流程的当前人为被委托人，然后提交流程
			engine.SetCurrentUser(mandatary.LoginId);

			var approveResult2 = new ApproveResult();
			approveResult2.Choice = "通过";
			approveResult2.Comment = "通过";
			approveResult2.NextUsers = new List<string>(){ mandatary.Id };

			var runResult2 = engine.RunWorkflow(instance2, approveResult2);
			Assert.IsTrue(runResult2);

			var instance3 = engine.LoadWorkflow(instance2.InstanceNo, 2);
			var workitem3 = instance3.CurrentWorkItem;
			Assert.IsNotNull(instance3);
			Assert.IsNotNull(workitem3);
			Assert.AreEqual(mandatary.Id, workitem3.MandataryId);

		}

		/// <summary>
		/// 待阅测试
		/// </summary>
		[Test()]
		public void ToBeReadTest(){
			Assert.IsNotNull(engine);

			var department = SecurityContext.Provider.GetOrganization(loginUser.DeptId);
			Assert.IsNotNull(department);

			var instance = engine.CreateWorkflow(1);
			Assert.IsNotNull(instance);

			instance.Creator = loginUser.Name;
			instance.CreatorId = loginUser.Id;
			instance.CreatorDeptId = department.Id;
			instance.CreatorDeptName = department.FullName;
			instance.Title = "（待阅单元测试）" + DateTime.Now.ToString("yyyyMMddHHmm");

			var saveResult = engine.SaveWorkflow(instance);
			Assert.IsTrue(saveResult);

			var approveResult = new ApproveResult();
			approveResult.Choice = "营销接口审核";
			approveResult.Comment = "通过";
			approveResult.NextUsers = new List<string>(){ loginUser.Id };

			var runResult = engine.RunWorkflow(instance, approveResult, new string[]{ loginUser.Id });
			Assert.IsTrue(runResult);

			instanceNo = instance.InstanceNo;

			var instance2 = engine.LoadWorkflow(instance.InstanceNo, 3);
			var workitem2 = instance2.CurrentWorkItem;
			Assert.IsNotNull(instance2);
			Assert.IsNotNull(workitem2);
			Assert.AreEqual(TaskStatus.ToRead, workitem2.TaskStatus);
		}
	}
}

