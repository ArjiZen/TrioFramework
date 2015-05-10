using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using Bingosoft.TrioFramework.DB;

namespace Bingosoft.TrioFramework.Security
{
    /// <summary>
    /// 权限
    /// </summary>
    [Table("SEC_Permission")]
    public class Permission
    {
        #region Properties
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public string Id { get; set; }
        /// <summary>
        /// 父Id
        /// </summary>
        public string Parent { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 页面地址
        /// </summary>
        [MaxLength(255)]
        public string Url { get; set; }
        /// <summary>
        /// 权限类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 创建者
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
        /// <summary>
        /// 拥有权限角色
        /// </summary>
        public virtual Collection<Role> Roles { get; set; }
        #endregion
    }

    /// <summary>
    /// 权限服务类
    /// </summary>
    public class PermissionContext : DbContextBase
    {
        #region ctor
        /// <summary>
        /// 实例化权限服务类
        /// </summary>
        public PermissionContext()
            : base()
        {

        }
        /// <summary>
        /// 实例化权限服务类
        /// </summary>
        /// <param name="conn">数据库链接</param>
        public PermissionContext(DbConnection conn)
            : base(conn)
        {

        }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>()
                .HasMany(p => p.Roles)
                .WithMany(r => r.Permissions)
                .Map(m => {
                    m.ToTable("SEC_RolePermissions");
                    m.MapLeftKey("PermissionId");
                    m.MapRightKey("RoleId");
                });
            base.OnModelCreating(modelBuilder);
        }
    }
}
