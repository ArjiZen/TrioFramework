using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Bingosoft.TrioFramework.DB;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace WorkflowCoreTest.Models
{
    [Table("WF_WorkItems")]
    public class UTWorkflowItem : WorkflowItem
    {
        public override void AddNew()
        {
            using (var db = DBFactory.Get<WorkflowItemContext<UTWorkflowItem>>())
            {
                db.WorkItems.Add(this);
                db.SaveChanges();
            }
        }

        public override void Update()
        {
            using (var db = DBFactory.Get<WorkflowItemContext<UTWorkflowItem>>())
            {
                var entry = db.Entry(this);
                entry.State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
