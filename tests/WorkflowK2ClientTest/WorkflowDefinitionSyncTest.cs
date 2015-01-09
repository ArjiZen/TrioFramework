using System;
using NUnit.Framework;
using Bingosoft.TrioFramework.Workflow.Core;
using System.Linq;

namespace Bingosoft.TrioFramework.Workflow.K2Client.Test {

	[TestFixture()]
	public class WorkflowDefinitionSyncTest {

		[Test()]
		public void SyncWorkflow1Test(){

			K2WorkflowHelper helper = new K2WorkflowHelper();
			var syncResult = helper.SyncWorkflowDefinition("营销支撑流程", true);
			Assert.IsTrue(syncResult);

			var engine = WorkflowEngine.Create();
			var definitions = engine.LoadDefinitions();
			Assert.AreNotEqual(0, definitions.Length);

			var leastDefinition = definitions.Where(p => p.AppCode.Equals(1)).OrderByDescending(p => p.Version).FirstOrDefault();
			Assert.IsNotNull(leastDefinition);

			Assert.AreEqual(16 ,leastDefinition.Activities.Count);
		}
	}
}

