using System;
using NUnit.Framework;
using Bingosoft.Security;
using Bingosoft.TrioFramework.Workflow.Core;

namespace Bingosoft.TrioFramework.Workflow.K2Client.Test {

	[TestFixture()]
	public class DelegateTest {

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

	}
}

