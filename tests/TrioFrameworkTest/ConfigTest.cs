using System;
using NUnit.Framework;

namespace Bingosoft.TrioFramework.Test {

	[TestFixture()]
	public class ConfigTest {

		[Test()]
		public void CommonSettingTest() {
			Assert.IsNotNull(SettingProvider.Common);
			Assert.AreEqual("TrioFramework", SettingProvider.Common.SystemId);
			Assert.AreEqual("pass@word1", SettingProvider.Common.SystemPassword);
			Assert.AreEqual("TrioFramework", SettingProvider.Common.SystemAccount);
			Assert.AreEqual("TrioFramework", SettingProvider.Common.SystemName);
		}


		[Test()]
		public void PendingJobTest() {
			Assert.IsNotNull(SettingProvider.PendingJob);
			Assert.IsTrue(SettingProvider.PendingJob.IsEnabled);
			Assert.IsFalse(SettingProvider.PendingJob.IsEnabledSMS);
			Assert.IsNotNullOrEmpty(SettingProvider.PendingJob.ApiUrl);
			Assert.IsNotNullOrEmpty(SettingProvider.PendingJob.JobUrl);
		}

		[Test()]
		public void WorkflowTest() {
			Assert.IsNotNullOrEmpty(SettingProvider.Workflow.Provider);
			Assert.IsFalse(SettingProvider.Workflow.IsConnectK2);
		}

		[Test()]
		public void FileServerTest(){
			Assert.IsNotNullOrEmpty(SettingProvider.FileServer.Server);
			Assert.IsNotNullOrEmpty(SettingProvider.FileServer.UserName);
			Assert.IsNotNullOrEmpty(SettingProvider.FileServer.Password);
		}

        [Test()]
	    public void ExcelComponentTest()
	    {
	        Assert.IsNotNull(SettingProvider.Excel.Assembly);
	    }

	}
}

