using System.Linq;
using Bingosoft.Data;
using Bingosoft.Data.Attributes;

namespace Bingosoft.TrioFramework.Workflow.K2Client.Models {
    /// <summary>
    /// 流程环节人员配置
    /// </summary>
    [Table("K2_ActivityConfig")]
    public class K2ActivityConfig {

        #region Properties
        /// <summary>
        /// 流程环节名称
        /// </summary>
        public string ActivityName { get; set; }
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
        #endregion

        private readonly static Dao _dao = Dao.Get();

        /// <summary>
        /// 获取流程环节配置信息
        /// </summary>
        /// <param name="appCode">流程编号</param>
        /// <param name="version">流程版本号</param>
        /// <returns></returns>
        public static K2ActivityConfig[] GetAll(int appCode, int version) {
            return _dao.QueryEntities<K2ActivityConfig>("k2clinet.definition.getactors", new {AppCode = appCode, Version = version}).ToArray();
        }
    }
}
