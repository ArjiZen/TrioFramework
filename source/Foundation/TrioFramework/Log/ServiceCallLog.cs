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
    /// 接口操作日志
    /// </summary>
    [Table("LOG_ServiceCall")]
    public class ServiceCallLog
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
        /// 所属模块
        /// </summary>
        public string Module { get; set; }
        /// <summary>
        /// 接口名称
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 请求内容
        /// </summary>
        public string RequestContent { get; set; }
        /// <summary>
        /// 请求时间
        /// </summary>
        public DateTime RequestTime { get; set; }
        /// <summary>
        /// 响应内容
        /// </summary>
        public string ResponseContent { get; set; }
        /// <summary>
        /// 响应时间
        /// </summary>
        public DateTime ResponseTime { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 异常堆栈信息
        /// </summary>
        public string StackTrace { get; set; }
        #endregion

        /// <summary>
        /// 保存接口日志
        /// </summary>
        internal void Save()
        {
            using (var db = DbContextBase.Get<ServiceCallLogContext>())
            {
                if (this.Id == 0)
                {
                    db.Logs.Add(this);
                }
                else
                {
                    db.Logs.Attach(this);
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 获取接口日志
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static ServiceCallLog Get(int id)
        {
            using (var db = DbContextBase.Get<ServiceCallLogContext>())
            {
                var query = (from l in db.Logs where l.Id == id select l).FirstOrDefault();
                return query;
            }
        }

        /// <summary>
        /// 获取所有接口日志
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IList<ServiceCallLog> FindAll(int pageIndex, int pageSize)
        {
            using (var db = DbContextBase.Get<ServiceCallLogContext>())
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
    /// 接口日志数据库操作类
    /// </summary>
    public class ServiceCallLogContext : DbContextBase
    {
        #region ctor

        /// <summary>
        /// 实例化接口日志数据库操作类
        /// </summary>
        public ServiceCallLogContext()
        {

        }
        /// <summary>
        /// 实例化接口日志数据库操作类
        /// </summary>
        /// <param name="conn">链接字符串</param>
        public ServiceCallLogContext(DbConnection conn)
            : base(conn)
        {

        }

        #endregion

        /// <summary>
        /// 日志集合
        /// </summary>
        public DbSet<ServiceCallLog> Logs { get; set; }
    }
}
