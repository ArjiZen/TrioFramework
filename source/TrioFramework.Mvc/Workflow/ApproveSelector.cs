using Bingosoft.Security;
using Bingosoft.Security.Principal;
using Bingosoft.TrioFramework.Models;
using Bingosoft.TrioFramework.Workflow.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bingosoft.TrioFramework.Mvc.Workflow {

    /// <summary>
    /// 审批页面选择对象
    /// </summary>
    public class ApproveSelector {
        /// <summary>
        /// 实例化审批页面选择对象
        /// </summary>
        public ApproveSelector() {
            this.NextActivities = new List<string>();
            this.NeedChoice = true;
        }
        /// <summary>
        /// 实例化审批页面选择对象
        /// </summary>
        /// <param name="instance"></param>
        public ApproveSelector(WorkflowInstance instance)
            : this() {
            var currentActi = instance.GetCurrentActi();
            foreach (var choice in currentActi.Transitions.Keys) {
                var nextActi = currentActi.Transitions[choice];
                var actor = nextActi.Actor;
                this.NextActivities.Add(choice);
                this.ChoiceActivityMapped.Add(choice, nextActi.Name);
                if (actor != null) {
                    IEnumerable<IUser> users;
                    if (choice.Contains("退回")) {
                        actor.RefActivityName = nextActi.Name;
                        users = actor.Resolve(instance);
                    } else {
                        users = actor.Resolve(instance);    
                    }
                    foreach (var user in users) {
                        var organization = SecurityContext.Provider.GetOrganization(user.DeptId);
                        this[choice].Add(new SelectorUser() {
                            id = user.Id,
                            name = user.Name,
                            deptName = organization.FullName
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 已选择用户
        /// </summary>
        [Serializable]
        public class SelectorUser {
            /// <summary>
            /// 用户id
            /// </summary>
            public string id { get; set; }
            /// <summary>
            /// 用户名
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 部门名称
            /// </summary>
            public string deptName { get; set; }
        }

        /// <summary>
        /// 下一环节
        /// </summary>
        public IList<string> NextActivities { get; set; }
        /// <summary>
        /// 审批结果与下一环节名称的映射关系
        /// </summary>
        public IDictionary<string, string> ChoiceActivityMapped = new Dictionary<string, string>();

        /// <summary>
        /// 是否需要用户选择
        /// </summary>
        public bool NeedChoice { get; set; }

        /// <summary>
        /// 已选中用户
        /// </summary>
        private readonly IDictionary<string, IList<SelectorUser>> m_Users = new Dictionary<string, IList<SelectorUser>>();
        /// <summary>
        /// 默认选中用户
        /// </summary>
        private readonly IDictionary<string, IList<string>> m_DefaultUsers = new Dictionary<string, IList<string>>();
        /// <summary>
        /// 获取该环节的参与者
        /// </summary>
        /// <param name="activityName">环节名称</param>
        /// <returns></returns>
        public IList<SelectorUser> this[string activityName] {
            get {
                if (NextActivities.Contains(activityName)) {
                    if (!m_Users.ContainsKey(activityName)) {
                        m_Users.Add(activityName, new List<SelectorUser>());
                    }
                    return m_Users[activityName];
                }
                return new List<SelectorUser>();
            }
        }

        /// <summary>
        /// 移除环节及相关处理人
        /// </summary>
        /// <param name="activityName">环节名称</param>
        public void RemoveActivity(string activityName) {
            if (NextActivities.Contains(activityName)) {
                m_Users.Remove(activityName);
                NextActivities.Remove(activityName);
            }
        }

        /// <summary>
        /// 清空环节处理人（不移除环节）
        /// </summary>
        /// <param name="activityName">环节名称</param>
        public void ClearUsers(string activityName) {
            this[activityName].Clear();
        }

        /// <summary>
        /// 添加环节处理人
        /// </summary>
        /// <param name="activityName">环节名</param>
        /// <param name="userId">用户id</param>
        /// <param name="selected">是否默认选中</param>
        public void AddUser(string activityName, string userId, bool selected = false) {
            if (this[activityName].Any(p => p.id.Equals(userId, StringComparison.OrdinalIgnoreCase))) {
                return;
            }
            var user = SecurityContext.Provider.Get(userId);
            if (user == null) {
                throw new NullReferenceException(string.Format("未找到ID为{0}的用户", userId));
            }
            var dept = Organization.Load(user.DeptId);
            if (dept == null) {
                throw new NullReferenceException(string.Format("未找到用户{0}所属的ID为{1}的部门", user.Name, user.DeptId));
            }
            this[activityName].Add(new SelectorUser() {
                id = user.Id,
                name = user.Name,
                deptName = dept.FullName
            });
            if (selected) {
                if (this.m_DefaultUsers.ContainsKey(activityName)) {
                    this.m_DefaultUsers[activityName].Add(userId);
                } else {
                    this.m_DefaultUsers.Add(activityName, new List<string>() { userId });
                }
            }
        }
        /// <summary>
        /// 设置默认审批用户（界面可重新选择）
        /// </summary>
        /// <param name="activityName"></param>
        /// <param name="userid"></param>
        public void SetDefault(string activityName, string userid) {
            if (m_DefaultUsers.ContainsKey(activityName)) {
                var lst = m_DefaultUsers[activityName];
                if (!lst.Contains(userid)) {
                    lst.Add(userid);
                }
            } else {
                m_DefaultUsers.Add(activityName, new List<string>() { userid });
            }
        }
        /// <summary>
        /// 转为Json字符串，供页面使用
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 返回数据格式：
        /// {
        ///    "needChoice": "True",
        ///    "next": ["营销接口审核"],
        ///    "nextActi": {
        ///        "营销接口审核": "营销接口审核、评估"
        ///    },
        ///    "营销接口审核": [{
        ///        "id": "0321031435",
        ///        "name": "张英霞",
        ///        "deptName": "业务支持中心.规划建设室"
        ///    }],
        ///    "default": {
        ///         "营销接口审核" : ["0321031435"]
        ///     }
        ///}
        /// </remarks>
        public override string ToString() {
            var nextActivities = this.NextActivities.OrderBy(p => p.Contains("退回"));
            var sbString = new StringBuilder("{");
            sbString.Append("\"needChoice\":\"" + this.NeedChoice + "\",");
            sbString.Append("\"next\":[");
            foreach (var activity in nextActivities) {
                sbString.Append("\"" + activity + "\",");
            }
            sbString.Remove(sbString.Length - 1, 1);
            sbString.Append("],");
            sbString.Append("\"nextActi\":{");
            foreach (var activity in NextActivities) {
                sbString.Append("\"" + activity + "\" : \"" + ChoiceActivityMapped[activity] + "\",");
            }
            sbString.Remove(sbString.Length - 1, 1);
            sbString.Append("},");
            foreach (var activity in NextActivities) {
                sbString.Append("\"" + activity + "\"").Append(":");
                var jUsers = JsonConvert.SerializeObject(this[activity]);
                sbString.Append(jUsers);
                sbString.Append(",");
            }
            sbString.Remove(sbString.Length - 1, 1);
            sbString.Append(",");
            sbString.Append("\"default\":{");
            foreach (var activity in NextActivities) {
                sbString.Append("\"" + activity + "\" : [");
                if (this.m_DefaultUsers.ContainsKey(activity)) {
                    foreach (var userid in this.m_DefaultUsers[activity]) {
                        sbString.Append("\"" + userid + "\",");
                    }
                    sbString.Remove(sbString.Length - 1, 1);
                }
                sbString.Append("],");
            }
            sbString.Remove(sbString.Length - 1, 1);
            sbString.Append("}}");
            return sbString.ToString();
        }
    }
}