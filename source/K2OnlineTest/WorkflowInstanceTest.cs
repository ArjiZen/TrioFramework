using Bingosoft.TrioFramework.Workflow.Core.Models;
using Bingosoft.TrioFramework.Workflow.K2Client.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K2OnlineTest {
    [TestClass]
    public class WorkflowInstanceTest {

        [TestMethod]
        public void GetTest() {
            var instance = WorkflowInstanceFactory.Get<K2WorkflowInstance>("2014110600008");
            Assert.AreEqual<string>("K2流程测试110608", instance.Title);
        }
    }
}
