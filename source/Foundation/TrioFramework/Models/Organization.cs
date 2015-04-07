using Bingosoft.Data;
using Bingosoft.Data.Attributes;
using Bingosoft.Security;

namespace Bingosoft.TrioFramework.Models {

    /// <summary>
    /// 组织架构
    /// </summary>
    [Table("SEC_Organization")]
    public class Organization {

        private readonly static Dao _dao = Dao.Get();

        #region
        /// <summary>
        /// 部门Id
        /// </summary>
        [PrimaryKey]
        public string Id { get; set; }
        /// <summary>
        /// 父部门Id
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 父部门
        /// </summary>
        private Organization _parent = null;
        /// <summary>
        /// 父部门
        /// </summary>
        public Organization Parent {
            get {
                if (_parent == null) {
                    _parent = Organization.Load(this.ParentId);
                }
                return _parent;
            }
        }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 部门编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 部门完整名称
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 部门所在级别
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 部门显示顺序
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 部门类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 部门状态
        /// </summary>
        public string Status { get; set; }
        #endregion

        /// <summary>
        /// 加载部门信息
        /// </summary>
        /// <param name="id">部门id</param>
        /// <returns></returns>
        public static Organization Load(string id) {
            return _dao.QueryEntity<Organization>("framework.organization.get", new { Id = id });
        }

        /// <summary>
        /// 根据名称获取部门信息
        /// </summary>
        /// <param name="name">部门名称</param>
        /// <returns></returns>
        public static Organization LoadByName(string name) {
            return _dao.QueryEntity<Organization>("framework.organization.getbyname", new { Name = name });
        }
    }
}