using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Bingosoft.TrioFramework.DB;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace WorkflowCoreTest.Models
{
    [Table("WF_WorkflowInstance")]
    public class UTWorkflowInstance : WorkflowInstance
    {
        public override void Save()
        {
            using (var db = DBFactory.Get<WorkflowInstanceContext<UTWorkflowInstance>>())
            {
                if (string.IsNullOrWhiteSpace(this.InstanceNo))
                {
                    this.InstanceNo = UTWorkflowInstance.GetNewInstanceNo<UTWorkflowInstance>();
                }
                db.Instances.Add(this);
                db.SaveChanges();
            }
        }

        public override void Update()
        {
            using (var db = DBFactory.Get<WorkflowInstanceContext<UTWorkflowInstance>>())
            {
                var entry = db.Entry(this);
                entry.State = EntityState.Modified;
                db.SaveChanges();
            }
        }

    }
}
