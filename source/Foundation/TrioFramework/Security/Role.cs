using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using Bingosoft.TrioFramework.DB;

namespace Bingosoft.TrioFramework.Security
{
    /// <summary>
    /// 角色
    /// </summary>
    [Table("SEC_Role")]
    public class Role
    {
        #region Properties
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public string Id { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
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
        /// <summary>
        /// 角色用户
        /// </summary>
        public virtual ICollection<User> Users { get; set; }
        /// <summary>
        /// 拥有权限
        /// </summary>
        public virtual ICollection<Permission> Permissions { get; set; }
        #endregion

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="code">角色编码</param>
        /// <returns></returns>
        public static Role Get(string code)
        {
            using (var db = DBFactory.Get<RoleContext>())
            {
                var query = (from r in db.Roles.Include(r => r.Users).Include(r => r.Permissions)
                             where r.Code.Equals(code, StringComparison.OrdinalIgnoreCase)
                             select r).FirstOrDefault();
                return query;
            }
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        public static IQueryable<Role> GetAll()
        {
            using (var db = DBFactory.Get<RoleContext>())
            {
                var query = from r in db.Roles select r;
                return query;
            }
        }
    }

    /// <summary>
    /// 角色服务类
    /// </summary>
    public class RoleContext : DbContextBase
    {
        #region ctor
        /// <summary>
        /// 实例化角色服务类
        /// </summary>
        public RoleContext()
        {
        }
        /// <summary>
        /// 实例化角色服务类
        /// </summary>
        /// <param name="conn">数据库链接</param>
        public RoleContext(DbConnection conn)
            : base(conn)
        {

        }
        #endregion

        /// <summary>
        /// 角色
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithMany(u => u.Roles)
                .Map(m => {
                    m.ToTable("SEC_RoleUsers");
                    m.MapLeftKey("RoleId");
                    m.MapRightKey("UserId");
                });
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Permissions)
                .WithMany(p => p.Roles)
                .Map(m => {
                    m.ToTable("SEC_RolePermissions");
                    m.MapLeftKey("RoleId");
                    m.MapRightKey("PermissionId");
                });
            base.OnModelCreating(modelBuilder);
        }
    }
}
