using System;
using System.Collections.Generic;
using System.Threading;
using Bingosoft.TrioFramework.Workflow.Core;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace WorkflowPerformanceConsoles
{
    class ParallelController
    {
        private void CreateAndRunWorkflow(object obj)
        {
            try
            {
                Console.WriteLine("Thread {0} : 开始", Thread.CurrentThread.ManagedThreadId);
                Console.WriteLine("Thread {0} : 开始创建流程", Thread.CurrentThread.ManagedThreadId);
                var engine = WorkflowEngine.Create();
                engine.SetCurrentUser("songshuang");
                var instance = engine.CreateWorkflow(1);
                instance.Title = Guid.NewGuid().ToString();
                engine.SaveWorkflow(instance);
                engine.RunWorkflow(instance, new ApproveResult() {
                    Choice = "营销接口人审核",
                    Comment = new Random().Next(0, 100).ToString(),
                    NextTobeReadUsers = null,
                    NextUsers = new List<string>(1) { "0300054661", "0300050508", "0300000398" }
                });
                Console.WriteLine("Thread {0} : 流程 {1} 已提交 => 营销接口人审核 环节", Thread.CurrentThread.ManagedThreadId, instance.InstanceNo);

                var instance2 = engine.LoadWorkflow(1, instance.InstanceNo, 2);
                engine.RunWorkflow(instance2, new ApproveResult() {
                    Choice = "通过",
                    Comment = new Random().Next(0, 100).ToString(),
                    NextTobeReadUsers = null,
                    NextUsers = new List<string>(1) { "0300054661" }
                });
                Console.WriteLine("Thread {0} : 流程 {1} 已提交 => 需求组长审核 环节", Thread.CurrentThread.ManagedThreadId, instance.InstanceNo);
                Console.WriteLine("Thread {0} : 结束", Thread.CurrentThread.ManagedThreadId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Thread {0} : 出现异常 => {1}", Thread.CurrentThread.ManagedThreadId, ex.GetMessages());
            }
        }

        public void RunWorkflow()
        {
            for (int i = 0; i < 2; i++)
            {
                ThreadPool.QueueUserWorkItem(CreateAndRunWorkflow);
                Thread.Sleep(new Random().Next(500, 800));
            }
        }

    }
}
