using System;
using Bingosoft.TrioFramework.Workflow.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WorkflowCoreTest
{
    [TestClass]
    public class WorkflowAttachTypeTest
    {
        [TestMethod]
        public void AddNewTest()
        {
            var attachType = new WorkflowAttachType() {
                Id = 1,
                Name = "普通附件"
            };
            attachType.AddNew();
            var existsType = WorkflowAttachType.Find(1);
            Assert.IsNotNull(existsType);
            Assert.AreEqual("普通附件", existsType.Name);
        }
    }
}
