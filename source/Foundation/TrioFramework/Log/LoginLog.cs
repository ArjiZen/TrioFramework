using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Bingosoft.TrioFramework.DB;

namespace Bingosoft.TrioFramework.Log
{
    /// <summary>
    /// 登录日志
    /// </summary>
    [Table("LOG_Login")]
    public class LoginLog
    {
        #region Properties
        /// <summary>
        /// 编号
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 所属系统
        /// </summary>
        public string Application { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }
        /// <summary>
        /// Ip地址
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 登录来源
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 请求参数
        /// </summary>
        public string RequestContent { get; set; }
        #endregion

        /// <summary>
        /// 保存登录日志
        /// </summary>
        internal void Save()
        {
            using (var db = DbContextBase.Get<LoginLogContext>())
            {
                db.Logs.Add(this);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 获取所有登录日志
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数据量</param>
        /// <returns></returns>
        public static IList<LoginLog> FindAll(int pageIndex, int pageSize)
        {
            using (var db = DbContextBase.Get<LoginLogContext>())
            {
                var query = (from l in db.Logs select l)
                                .OrderByDescending(l => l.Id)
                                .Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize);
                return query.ToList();
            }
        }
    }

    /// <summary>
    /// 登录日志数据库操作类
    /// </summary>
    public class LoginLogContext : DbContextBase
    {
        #region ctor
        /// <summary>
        /// 实例化登录日志数据库操作类
        /// </summary>
        public LoginLogContext()
        {

        }
        /// <summary>
        /// 实例化登录日志数据库操作类
        /// </summary>
        /// <param name="conn">数据库链接</param>
        public LoginLogContext(DbConnection conn)
            : base(conn)
        {

        }
        #endregion

        /// <summary>
        /// 登录日志集合
        /// </summary>
        public DbSet<LoginLog> Logs { get; set; }
    }
}
