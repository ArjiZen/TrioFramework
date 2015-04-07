using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Bingosoft.Data;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace Bingosoft.TrioFramework.Workflow.K2Client.Models {
    /// <summary>
    /// K2流程定义
    /// </summary>
    public class K2WorkflowDefinition : WorkflowDefinition {

        private readonly static Dao _dao = Dao.Get();
        /// <summary>
        /// 将XML信息转换为对应实体属性
        /// </summary>
        public override void InitActivities() {
            if (string.IsNullOrEmpty(this.DefinitionXml))
                return;

            var actors = K2ActivityConfig.GetAll(this.AppCode, this.Version);
            this.Activities = new List<WorkflowActivity>();

            var rootNode = XElement.Parse(this.DefinitionXml);
            //获取所有为 Activity的节点，包括孙子节点
            var activityNodes = from n in rootNode.Descendants("Activity") select n;
            var lineNodes = from n in rootNode.Descendants("Line") select n;
            foreach (XElement activityElement in activityNodes) {
                var activity = new K2WorkflowActivity();
                activity.Name = activityElement.Attribute("Name").Value;

                activity.Actor = (from e in actors
                                  where e.ActivityName == activity.Name
                                  select new K2Actor { RoleName = e.RoleName, RoleBase = e.RoleBase, DeptId = e.DeptId, RefActivityName = e.RefActivityName })
                                  .FirstOrDefault();

                #region 内部WorkflowActivity 的Transitions初始化
                IDictionary<string, WorkflowActivity> activityTransitions = new Dictionary<string, WorkflowActivity>();

                //获得对应的迁移方向，及其环节属性
                var targets = from l in lineNodes
                              join a in activityNodes on (string)l.Attribute("ToId").Value equals (string)a.Attribute("ID").Value
                              where (string)l.Attribute("FromId").Value == (string)activityElement.Attribute("ID").Value
                              select new {
                                  Label = (string)l.Attribute("Label").Value,
                                  Activity = a
                              };
                //填充
                foreach (var target in targets) {
                    K2WorkflowActivity targetActivity = new K2WorkflowActivity();
                    targetActivity.Name = target.Activity.Attribute("Name").Value;
                    targetActivity.Actor = (from e in actors
                                            where e.ActivityName == targetActivity.Name
                                            select new K2Actor { RoleName = e.RoleName, RoleBase = e.RoleBase, DeptId = e.DeptId, RefActivityName = e.RefActivityName })
                                  .FirstOrDefault();
                    targetActivity.Transitions = new Dictionary<string, WorkflowActivity>();    //边界就到这里

                    activityTransitions.Add(target.Label, targetActivity);
                }
                #endregion
                activity.Transitions = activityTransitions;
                this.Activities.Add(activity);
            }
        }

        /// <summary>
        /// 保存流程定义
        /// </summary>
        /// <returns></returns>
        public override bool Save() {
            var effectRow = _dao.Insert<K2WorkflowDefinition>(this);
            return effectRow > 0;
        }

        /// <summary>
        /// 更新流程定义
        /// </summary>
        /// <returns></returns>
        public override bool Update() {
            var effectRow = _dao.UpdateFields<K2WorkflowDefinition>(this, "DefinitionXml", "Description");
            return effectRow > 0;
        }
    }
}
