using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowCoreTest.Models;

namespace WorkflowCoreTest
{
    [TestClass]
    public class WorkflowInstanceTest
    {
        [TestMethod]
        public void SaveTest()
        {
            var definition = UTWorkflowDefinition.Find<UTWorkflowDefinition>(1, 1);
            var instance = new UTWorkflowInstance() {
                AppCode = definition.AppCode,
                AppName = definition.AppName,
                Version = definition.Version,
                CreatorId = "wangzr",
                Creator = "泽然",
                CreatorDeptId = "",
                CreatorDeptName = "",
                CurrentActi = "提出需求",
                StartTime = DateTime.Now,
                Title = "测试流程" + DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            instance.Save();
            var existsInstance = UTWorkflowInstance.Find<UTWorkflowInstance>(instance.InstanceNo);
            Assert.IsNotNull(existsInstance);
            Assert.AreEqual(instance.Title, existsInstance.Title);
        }

        [TestMethod]
        public void UpdateTest()
        {
            var instance = UTWorkflowInstance.Find<UTWorkflowInstance>("2015052400001");
            instance.EndTime = DateTime.Now;
            instance.Update();
            var existsInstance = UTWorkflowInstance.Find<UTWorkflowInstance>(instance.InstanceNo);
            Assert.IsNotNull(existsInstance);
            Assert.IsNotNull(existsInstance.EndTime);
        }
    }
}
