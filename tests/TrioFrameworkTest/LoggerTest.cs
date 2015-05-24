using System;
using Bingosoft.TrioFramework;
using Bingosoft.TrioFramework.Log;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TrioFrameworkTest
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public void LogErrorTest()
        {
            var listBefore = ErrorLog.FindAll(1, 999);
            Logger.LogError("TrioFrameworkTest.LoggerTest", new Exception("异常测试信息"));
            var listAfter = ErrorLog.FindAll(1, 999);
            Assert.AreEqual(1, listAfter.Count - listBefore.Count);
        }

        [TestMethod]
        public void LogOperationTest()
        {
            var listBefore = OperationLog.FindAll(1, 10);
            Logger.LogOperation("TrioFrameworkTest.LoggerTest", "Test", "LogOperationTest");
            var listAfter = OperationLog.FindAll(1, 10);
            Assert.AreEqual(1, listAfter.Count - listBefore.Count);
        }

        [TestMethod]
        public void LogLoginTest()
        {
            var listBefore = LoginLog.FindAll(1, 10);
            Logger.Login("wangzr");
            var listAfter = LoginLog.FindAll(1, 10);
            Assert.AreEqual(1, listAfter.Count - listBefore.Count);
        }

        [TestMethod]
        public void LogServiceCallTest()
        {
            var token = DateTime.Now.ToString("yyyyMMddHHmmss");
            var listBefore = ServiceCallLog.FindAll(1, 100);
            var logid = Logger.LogServiceRequest("TrioFrameworkTest.LoggerTest", "ServiceTest", "Request Test " + token);
            Logger.LogServiceResponse(logid, "Response Test");
            var listAfter = ServiceCallLog.FindAll(1, 100);
            Assert.AreEqual(1, listAfter.Count - listBefore.Count);
            var entity = listAfter[0];
            Assert.AreEqual("Request Test " + token, entity.RequestContent);
        }

        [TestMethod]
        public void LogServiceCallExceptionTest()
        {
            var token = DateTime.Now.ToString("yyyyMMddHHmmss");
            var listBefore = ServiceCallLog.FindAll(1, 100);
            var logid = Logger.LogServiceRequest("TrioFrameworkTest.LoggerTest", "ServiceTest", "Request Test " + token);
            Logger.LogServiceResponse(logid, new Exception("异常测试"));
            var listAfter = ServiceCallLog.FindAll(1, 100);
            Assert.AreEqual(1, listAfter.Count - listBefore.Count);
            var entity = listAfter[0];
            Assert.AreEqual("Request Test " + token, entity.RequestContent);
        }
    }
}
