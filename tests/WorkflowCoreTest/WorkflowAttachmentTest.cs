using System;
using Bingosoft.TrioFramework.Workflow.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WorkflowCoreTest
{
    [TestClass]
    public class WorkflowAttachmentTest
    {
        [TestMethod]
        public void AddNewTest()
        {
            var attachment = new WorkflowAttachment() {
                FileId = Guid.NewGuid().ToString(),
                InstanceNo = "2015052400001",
                TaskId = 1,
                ActivityName = "提出需求",
                FileTypeId = 0,
                FileName = "测试附件1.txt",
                FilePath = string.Format("{0}/{1}/{2}/{3}_{4}.txt", "2015", "5", "24", "测试附件1", "00001"),
                FileSize = 10240,
                Creator = "泽然",
                CreatorId = "wangzr",
                CreateTime = DateTime.Now
            };
            attachment.AddNew();
            var existsAttachments = WorkflowAttachment.FindAll("2015052400001");
            Assert.IsNotNull(existsAttachments);
            Assert.AreEqual(1, existsAttachments.Count);
            Assert.AreEqual("测试附件1.txt", existsAttachments[0].FileName);
            Assert.IsNotNull(existsAttachments[0].FileType);
            Assert.AreEqual("普通附件", existsAttachments[0].FileType.Name);
        }
    }
}
