using Bingosoft.Security;
using Bingosoft.Security.Principal;
using Bingosoft.TrioFramework.Workflow.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bingosoft.TrioFramework.Mvc.Workflow
{

    /// <summary>
    /// 审批页面选择对象
    /// </summary>
    public class ApproveSelector : Selector
    {
        /// <summary>
        /// 实例化审批页面选择对象
        /// </summary>
        public ApproveSelector()
        {
            this.NeedChoice = true;
        }

        /// <summary>
        /// 实例化审批页面选择对象
        /// </summary>
        /// <param name="instance"></param>
        public ApproveSelector(WorkflowInstance instance)
            : this()
        {
            var currentActi = instance.GetCurrentActi();
            foreach (var choice in currentActi.Transitions.Keys)
            {
                var nextActi = currentActi.Transitions[choice];
                if (nextActi == null)
                {
                    throw new NextActivityNotFoundException(instance.InstanceNo, instance.CurrentActivity, choice);
                }
                var actor = nextActi.Actor;
                this.m_NextChoice.Add(choice);
                this._activities.Add(choice, nextActi.Name);

                IEnumerable<IUser> users = new List<IUser>();
                if (actor != null)
                {
                    if (choice.Contains("退回"))
                    {
                        actor.RefActivityName = nextActi.Name;
                        users = actor.Resolve(instance);
                    }
                    else
                    {
                        users = actor.Resolve(instance);
                    }
                }

                foreach (var user in users)
                {
                    var organization = SecurityContext.Provider.GetOrganization(user.DeptId);
                    this[choice].Add(new SelectorUser() {
                        Id = user.Id,
                        Name = user.Name,
                        DeptName = organization.FullName
                    });
                }
            }
        }

        /// <summary>
        /// 下一环节
        /// </summary>
        protected IList<string> m_NextChoice = new List<string>();

        /// <summary>
        /// 当前环节的所有审批项
        /// </summary>
        public string[] Choices
        {
            get { return m_NextChoice.ToArray(); }
        }

        /// <summary>
        /// 审批结果与下一环节名称的映射关系
        /// </summary>
        private readonly IDictionary<string, string> _activities = new Dictionary<string, string>();

        /// <summary>
        /// 是否需要用户选择
        /// </summary>
        protected bool NeedChoice { get; set; }

        /// <summary>
        /// 设置默认审批用户（界面可重新选择）
        /// </summary>
        /// <param name="choice"></param>
        /// <param name="userid"></param>
        public void SetDefault(string choice, string userid)
        {
            if (Users.ContainsKey(choice))
            {
                foreach (var item in Users[choice])
                {
                    if (item.Id.Equals(userid, StringComparison.OrdinalIgnoreCase))
                    {
                        item.Selected = true;
                    }
                }
            }
            else
            {
                this.Add(choice, userid, true);
            }
        }

        /// <summary>
        /// 设置默认审批项
        /// </summary>
        /// <param name="choice">审批项.</param>
        public void SetDefault(string choice)
        {
            if (m_NextChoice.Contains(choice))
            {
                m_NextChoice.Remove(choice);
                m_NextChoice.Insert(0, choice);
            }
            else
            {
                throw new ChoiceNotFoundException(choice);
            }
        }

        /// <summary>
        /// 添加环节用户
        /// </summary>
        /// <param name="choice">审核结果.</param>
        /// <param name="id">用户loginid或userid.</param>
        /// <param name="selected">是否默认选中.</param>
        public new void Add(string choice, string id, bool selected = false)
        {
            base.Add(choice, id, selected);
        }

        /// <summary>
        /// 移除审批结果
        /// </summary>
        /// <param name="choice">Choice.</param>
        public override void RemoveChoice(string choice)
        {
            if (m_NextChoice.Contains(choice))
            {
                m_NextChoice.Remove(choice);
            }
            if (Users.ContainsKey(choice))
            {
                Users.Remove(choice);
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
        ///    "next": ["营销接口人审核"],
        ///    "nextActi": {
        ///        "营销接口人审核": "营销接口人审核、评估"
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
        public override string ToString()
        {
            var nextActivities = this.m_NextChoice.OrderBy(p => p.Contains("退回"));
            var sbString = new StringBuilder("{");
            sbString.Append("\"needChoice\":\"" + this.NeedChoice + "\",");
            sbString.Append("\"next\":[");
            foreach (var activity in nextActivities)
            {
                sbString.Append("\"" + activity + "\",");
            }
            sbString.Remove(sbString.Length - 1, 1);
            sbString.Append("],");
            sbString.Append("\"nextActi\":{");
            foreach (var choice in m_NextChoice)
            {
                sbString.Append("\"" + choice + "\" : \"" + _activities[choice] + "\",");
            }
            sbString.Remove(sbString.Length - 1, 1);
            sbString.Append("},");
            foreach (var choice in m_NextChoice)
            {
                sbString.Append("\"" + choice + "\"").Append(":");
                var jUsers = JsonConvert.SerializeObject(this[choice].ToList());
                sbString.Append(jUsers);
                sbString.Append(",");
            }
            sbString.Remove(sbString.Length - 1, 1);
            sbString.Append(",");
            sbString.Append("\"default\":{");
            foreach (var choice in m_NextChoice)
            {
                sbString.Append("\"" + choice + "\" : [");
                if (Users.ContainsKey(choice))
                {
                    var hasUser = false;
                    foreach (var u in Users[choice].Where(p => p.Selected))
                    {
                        sbString.Append("\"" + u.Id + "\",");
                        hasUser = true;
                    }
                    if (hasUser)
                    {
                        sbString.Remove(sbString.Length - 1, 1);
                    }
                }
                sbString.Append("],");
            }
            sbString.Remove(sbString.Length - 1, 1);
            sbString.Append("}}");
            return sbString.ToString();
        }
    }
}