using Bingosoft.TrioFramework.Workflow.K2Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K2OnlineTest {
    [TestClass]
    public class WorkflowDefinitionTest {
        [TestMethod]
        public void LoadDefinition() {
            var engine = new ServerEngine();
            var definition = engine.GetDefinitionXml(@"ITSPMAS\营销支撑流程");
            Assert.AreNotEqual("", definition);
        }

        [TestMethod]
        public void SyncDefinition() {
            var helper = new K2WorkflowHelper();
            var success = helper.SyncWorkflowDefinition("营销支撑流程", true);
            Assert.AreEqual<bool>(true, success);
        }
    }
}
