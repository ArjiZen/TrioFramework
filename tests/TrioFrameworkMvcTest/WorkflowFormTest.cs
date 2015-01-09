using System;
using NUnit.Framework;
using Bingosoft.TrioFramework.Mvc.Workflow;
using Bingosoft.TrioFramework.Workflow.Core;

namespace Bingosoft.TrioFramework.Mvc.Test {

	[TestFixture()]
	public class WorkflowFormTest {

		[Test()]
		public void TobeRead(){
			var engine = WorkflowEngine.Create();
			engine.SetCurrentUser("songshuang");
			var instance = engine.CreateWorkflow(1);
		}
	}
}

