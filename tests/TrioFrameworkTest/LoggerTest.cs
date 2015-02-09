using System;
using NUnit.Framework;

namespace Bingosoft.TrioFramework.Test {
	[TestFixture()]
	public class LoggerTest {

		[Test()]
		public void LogServiceRequestTest() {
			var logid = Logger.LogServiceRequest("Test", "Test", "Test");
			Assert.AreNotEqual(-1, logid);
		}

		[Test()]
		public void LogServiceResponseTest() {
			var logid = Logger.LogServiceRequest("Test", "Test", "Test");
			Assert.AreNotEqual(-1, logid);
			var isSuccess = Logger.LogServiceResponse(logid, "Test");
			Assert.IsTrue(isSuccess);
		}
	}
}

