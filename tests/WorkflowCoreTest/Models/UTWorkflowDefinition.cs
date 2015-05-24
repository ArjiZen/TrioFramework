using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using Bingosoft.TrioFramework.DB;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace WorkflowCoreTest.Models
{
    /// <summary>
    /// Workflow Definition For Unit Test
    /// </summary>
    [Table("WF_Definitions")]
    public class UTWorkflowDefinition : WorkflowDefinition
    {
        public override void Save()
        {
            using (var db = DBFactory.Get<WorkflowDefinitionContext<UTWorkflowDefinition>>())
            {
                db.Definitions.Add(this);
                db.SaveChanges();
            }
        }

        public override void Update()
        {
            using (var db = DBFactory.Get<WorkflowDefinitionContext<UTWorkflowDefinition>>())
            {
                var entry = db.Entry(this);
                entry.State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public override void InitActivities()
        {
            throw new NotImplementedException();
        }
    }
}
