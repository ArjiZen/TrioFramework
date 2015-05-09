using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bingosoft.TrioFramework.Workflow.Core;
using Bingosoft.TrioFramework.Workflow.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bingosoft.TrioFramework.Workflow.K2Client.Test
{
    [TestClass]
    class PaiallelTest
    {
        [TestInitialize]
        public void WorkflowInit()
        {
        }

        private void CreateAndRunWorkflow(object obj)
        {
            var engine = WorkflowEngine.Create();
            engine.SetCurrentUser("songshuang");
            var instance = engine.CreateWorkflow(1);
            instance.Title = Guid.NewGuid().ToString();
            engine.SaveWorkflow(instance);
            engine.RunWorkflow(instance, new ApproveResult() {
                Choice = "营销接口人审核",
                Comment = new Random().Next(0, 100).ToString(),
                NextTobeReadUsers = null,
                NextUsers = new List<string>(1) { "0300054661" }
            });

            var instance2 = engine.LoadWorkflow(1, instance.InstanceNo, 2);
            engine.RunWorkflow(instance2, new ApproveResult()
            {
                Choice = "通过",
                Comment = new Random().Next(0, 100).ToString(),
                NextTobeReadUsers = null,
                NextUsers = new List<string>(1) {"0300054661"}
            });
        }

        [TestMethod]
        public void RunWorkflowTest()
        {
            for (int i = 0; i < 10; i++)
            {
                ThreadPool.QueueUserWorkItem(CreateAndRunWorkflow);
                Thread.Sleep(100);
            }
            while (true)
            {
                int workThead, b;
                ThreadPool.GetAvailableThreads(out workThead, out b);
                if (workThead == 0)
                {
                    break;
                }
                Thread.Sleep(500);
            }
        }

    }
}
