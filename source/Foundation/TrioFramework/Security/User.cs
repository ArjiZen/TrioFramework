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
    /// 用户
    /// </summary>
    [Table("SEC_User")]
    public class User
    {
        #region Properties
        /// <summary>
        /// 用户Id
        /// </summary>
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }
        /// <summary>
        /// 登录账号
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string LoginId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// 账号类型(local/portal)
        /// </summary>
        [MaxLength(10)]
        public string Type { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        [MinLength(6)]
        public string Password { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(255)]
        public string Email { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string MobilePhone { get; set; }
        /// <summary>
        /// IM
        /// </summary>
        public string IM { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Telephone { get; set; }
        /// <summary>
        /// 性别(M/F)
        /// </summary>
        [MaxLength(1)]
        public string Sex { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string Status { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        [Required]
        public string OrgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Rank { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// 上次更新人
        /// </summary>
        public string LastUpdatedBy { get; set; }
        /// <summary>
        /// 上次更新时间
        /// </summary>
        public DateTime? LastUpdatedTime { get; set; }
        /// <summary>
        /// 拥有角色
        /// </summary>
        public virtual ICollection<Role> Roles { get; set; }
        #endregion

        /// <summary>
        /// 保存用户信息
        /// </summary>
        /// <returns></returns>
        public void Save()
        {
            using (var db = DbContextBase.Get<UserContext>())
            {
                db.Users.Add(this);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="loginidOrUserId">登录id或用户id</param>
        /// <returns></returns>
        public static User Get(string loginidOrUserId)
        {
            using (var db = DbContextBase.Get<UserContext>())
            {
                var query = (from u in db.Users.Include(r => r.Roles)
                             where u.LoginId.Equals(loginidOrUserId, StringComparison.OrdinalIgnoreCase) || u.Id.Equals(loginidOrUserId, StringComparison.OrdinalIgnoreCase)
                             select u).FirstOrDefault();
                return query;
            }
        }

        /// <summary>
        /// 验证登录
        /// </summary>
        /// <returns></returns>
        public bool CheckPassword()
        {
            using (var db = DbContextBase.Get<UserContext>())
            {
                var query = (from u in db.Users
                             where u.LoginId.Equals(this.LoginId, StringComparison.OrdinalIgnoreCase) && u.Password.Equals(this.Password, StringComparison.OrdinalIgnoreCase)
                             select u).FirstOrDefault();
                return query != null;
            }
        }

    }

    /// <summary>
    /// 用户服务类
    /// </summary>
    public class UserContext : DbContextBase
    {
        #region ctor
        /// <summary>
        /// 实例化用户服务类
        /// </summary>
        public UserContext()
            : base()
        {

        }

        /// <summary>
        /// 实例化用户服务类
        /// </summary>
        /// <param name="conn">数据库链接</param>
        public UserContext(DbConnection conn)
            : base(conn)
        {

        }

        #endregion

        /// <summary>
        /// 用户集合
        /// </summary>
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .Map(m => {
                    m.ToTable("SEC_RoleUsers");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("RoleId");
                });
            base.OnModelCreating(modelBuilder);
        }
    }
}
