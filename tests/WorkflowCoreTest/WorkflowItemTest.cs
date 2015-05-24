using System;
using Bingosoft.TrioFramework.Workflow.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowCoreTest.Models;

namespace WorkflowCoreTest
{
    [TestClass]
    public class WorkflowItemTest
    {
        [TestMethod]
        public void SaveTest()
        {
            var item = new UTWorkflowItem() {
                InstanceNo = "2015052400001",
                TaskId = 1,
                PartId = "wangzr",
                PartName = "泽然",
                PartDeptId = "",
                PartDeptName = "",
                ReceTime = DateTime.Now,
                TaskStatus = TaskStatus.Waiting,
                CurrentActi = "提出需求"
            };
            item.AddNew();
            var existsItem = UTWorkflowItem.Find<UTWorkflowItem>("2015052400001", 1);
            Assert.IsNotNull(existsItem);
        }

        [TestMethod]
        public void UpdateTest()
        {
            var item = UTWorkflowItem.Find<UTWorkflowItem>("2015052400001", 1);
            item.ReadTime = DateTime.Now;
            item.FinishTime = DateTime.Now;
            item.AutoFinished = false;
            item.TaskStatus = TaskStatus.Accept;
            item.Choice = "通过";
            item.Comment = "OK";
            item.Update();
            var existsItem = UTWorkflowItem.Find<UTWorkflowItem>("2015052400001", 1);
            Assert.IsNotNull(existsItem);
            Assert.IsNotNull(item.FinishTime);
        }

        [TestMethod]
        public void WorkflowInstanceHasItems()
        {
            var instance = UTWorkflowInstance.Find<UTWorkflowInstance>("2015052400001");
            Assert.IsNotNull(instance.WorkItems);
            Assert.AreEqual(1, instance.WorkItems.Count);
        }
    }
}
