using System;
using NUnit.Framework;
using Bingosoft.Security;
using Bingosoft.TrioFramework.Workflow.Core;
using System.Reflection;

namespace Bingosoft.TrioFramework.Workflow.K2Client.Test {

	[TestFixture()]
	public class DelegateTest {

		[TestFixtureSetUp()]
		public void Setup(){
			Assembly.Load("Bingosoft.TrioFramework");
		}

		/// <summary>
		/// 添加新的委托关系
		/// </summary>
		[Test()]
		public void AddNewTest(){
			var delegator = SecurityContext.Provider.GetUser("songshuang");
			var mandatary = SecurityContext.Provider.GetUser("liangyanshan");

			Assert.IsNotNull(delegator);
			Assert.IsNotNull(mandatary);

			var delegateWork = new DelegateWork();
			delegateWork.AppCode = 0;
			delegateWork.Delegator = delegator.Name;
			delegateWork.DelegatorId = delegator.Id;
			delegateWork.Mandatary = mandatary.Name;
			delegateWork.MandataryId = mandatary.Id;
			delegateWork.StartTime = DateTime.Now.AddMinutes(-5);
			delegateWork.EndTime = DateTime.Now.AddMinutes(5);
			delegateWork.IsDeleted = false;

			var addSuccess = delegateWork.AddNew();
			Assert.IsTrue(addSuccess);
		}

		/// <summary>
		/// 根据委托人获取委托信息
		/// </summary>
		[Test()]
		public void GetByDelegatorTest(){
			var delegator = SecurityContext.Provider.GetUser("songshuang");
			var mandatary = SecurityContext.Provider.GetUser("liangyanshan");

			Assert.IsNotNull(delegator);
			Assert.IsNotNull(mandatary);

			var delegateWork = new DelegateWork();
			delegateWork.AppCode = 0;
			delegateWork.Delegator = delegator.Name;
			delegateWork.DelegatorId = delegator.Id;
			delegateWork.Mandatary = mandatary.Name;
			delegateWork.MandataryId = mandatary.Id;
			delegateWork.StartTime = DateTime.Now.AddSeconds(-5);
			delegateWork.EndTime = DateTime.Now.AddSeconds(30);
			delegateWork.IsDeleted = false;

			var addSuccess = delegateWork.AddNew();
			Assert.IsTrue(addSuccess);

			var works = DelegateWork.GetByDeletagor(0, delegator.Id);
			Assert.IsNotNull(works);
			Assert.AreEqual(1, works.Count);
			Assert.AreEqual(mandatary.Id, works[0].MandataryId);
		}

		/// <summary>
		/// 根据被委托人获取委托信息
		/// </summary>
		[Test()]
		public void GetByMandataryTest(){
			var delegator = SecurityContext.Provider.GetUser("songshuang");
			var mandatary = SecurityContext.Provider.GetUser("liangyanshan");

			Assert.IsNotNull(delegator);
			Assert.IsNotNull(mandatary);

			var delegateWork = new DelegateWork();
			delegateWork.AppCode = 0;
			delegateWork.Delegator = delegator.Name;
			delegateWork.DelegatorId = delegator.Id;
			delegateWork.Mandatary = mandatary.Name;
			delegateWork.MandataryId = mandatary.Id;
			delegateWork.StartTime = DateTime.Now.AddSeconds(-5);
			delegateWork.EndTime = DateTime.Now.AddSeconds(30);
			delegateWork.IsDeleted = false;

			var addSuccess = delegateWork.AddNew();
			Assert.IsTrue(addSuccess);

			var works = DelegateWork.GetByMandatary(0, mandatary.Id);
			Assert.IsNotNull(works);
			Assert.AreEqual(1, works.Count);
			Assert.AreEqual(delegator.Id, works[0].DelegatorId);
		}
	}
}

