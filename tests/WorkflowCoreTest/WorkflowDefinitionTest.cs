using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowCoreTest.Models;

namespace WorkflowCoreTest
{
    [TestClass]
    public class WorkflowDefinitionTest
    {
        [TestMethod]
        public void SaveTest()
        {
            var definition = new UTWorkflowDefinition() {
                AppCode = 1,
                Version = 1,
                AppName = "测试流程",
                InternalName = "",
                DefinitionXml = "",
                Description = "Workflow Definition For UnitTest"
            };
            definition.Save();
            var existsDefinition = UTWorkflowDefinition.Find<UTWorkflowDefinition>(1, 1);
            Assert.IsNotNull(existsDefinition);
            Assert.AreEqual("测试流程", existsDefinition.AppName);
        }

        [TestMethod]
        public void UpdateTest()
        {
            var definition = UTWorkflowDefinition.Find<UTWorkflowDefinition>(1, 1);
            definition.InternalName = "Electric/Workflow";
            definition.Update();
            var existsDefinition = UTWorkflowDefinition.Find<UTWorkflowDefinition>(1, 1);
            Assert.IsNotNull(existsDefinition);
            Assert.AreEqual("Electric/Workflow", existsDefinition.InternalName);
        }
    }
}
