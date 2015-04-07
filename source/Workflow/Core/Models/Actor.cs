using System.Collections.Generic;
using Bingosoft.Security.Principal;

namespace Bingosoft.TrioFramework.Workflow.Core.Models {
    /// <summary>
    /// 环节参与人计算
    /// </summary>
    public abstract class Actor {
        /// <summary>
        /// 角色名
        /// </summary>
        /// <remarks>
        /// 内置角色
        /// 1、建单人
        /// 2、指定环节
        /// </remarks>
        public string RoleName { get; set; }
        /// <summary>
        /// 角色基准
        /// </summary>
        /// <remarks>
        /// 1、当前用户
        /// 2、建单用户
        /// 3、指定部门
        /// </remarks>
        public string RoleBase { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        public string DeptId { get; set; }
        /// <summary>
        /// 引用环节名
        /// </summary>
        public string RefActivityName { get; set; }

        /// <summary>
        /// 计算环节参与人
        /// </summary>
        public abstract IEnumerable<IUser> Resolve(WorkflowInstance instance);
    }
}
