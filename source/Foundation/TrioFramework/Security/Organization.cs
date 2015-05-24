using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using Bingosoft.TrioFramework.DB;

namespace Bingosoft.TrioFramework.Security
{
    /// <summary>
    /// 组织架构
    /// </summary>
    [Table("SEC_Organization")]
    public class Organization
    {
        #region Properties
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 父Id
        /// </summary>
        public string Parent { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 完整编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 完整名称
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 部门级别
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 部门类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 状态 enabled/disabled
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// 上一次更新人
        /// </summary>
        public string LastUpdatedBy { get; set; }
        /// <summary>
        /// 上一次更新时间
        /// </summary>
        public DateTime? LastUpdatedTime { get; set; }
        #endregion

        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <param name="dpidOrDpCode">部门id或部门编号</param>
        /// <returns></returns>
        public static Organization Get(string dpidOrDpCode)
        {
            using (var db = DBFactory.Get<OrganizationContexnt>())
            {
                var query = (from o in db.Organizations
                             where o.Id.Eq(dpidOrDpCode) || o.Code.Eq(dpidOrDpCode)
                             select o).FirstOrDefault();
                return query;
            }
        }

        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <param name="fullName">部门全称</param>
        /// <returns></returns>
        public static Organization GetByFullName(string fullName)
        {
            using (var db = DBFactory.Get<OrganizationContexnt>())
            {
                var query = (from o in db.Organizations
                             where o.FullName.Eq(fullName)
                             select o).FirstOrDefault();
                return query;
            }
        }

    }

    /// <summary>
    /// 组织架构服务类
    /// </summary>
    public class OrganizationContexnt : DbContextBase
    {
        #region ctor
        /// <summary>
        /// 实例化组织架构服务类
        /// </summary>
        public OrganizationContexnt()
            : base()
        {
        }

        /// <summary>
        /// 实例化组织架构服务类
        /// </summary>
        /// <param name="conn">数据库链接</param>
        public OrganizationContexnt(DbConnection conn)
            : base(conn)
        {
        }
        #endregion

        /// <summary>
        /// 部门列表
        /// </summary>
        public DbSet<Organization> Organizations { get; set; }
    }
}
