using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using Bingosoft.TrioFramework.DB;
using Newtonsoft.Json;

namespace Bingosoft.TrioFramework.Log
{
    /// <summary>
    /// 错误日志
    /// </summary>
    [Table("LOG_Error")]
    public class ErrorLog
    {
        #region Properties
        /// <summary>
        /// 日志编号
        /// </summary>
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
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 异常来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 异常错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 异常堆栈信息
        /// </summary>
        public string StackTrace { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        public string ExtraData { get; set; }
        /// <summary>
        /// 当前用户
        /// </summary>
        public string ErrorUser { get; set; }
        /// <summary>
        /// 出错时间
        /// </summary>
        public DateTime ErrorTime { get; set; }

        #endregion

        /// <summary>
        /// 记录错误日志
        /// </summary>
        internal void Save()
        {
            using (var db = DbContextBase.Get<ErrorLogContext>())
            {
                db.Logs.Add(this);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 获取所有错误日志
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IList<ErrorLog> FindAll(int pageIndex, int pageSize)
        {
            using (var db = DbContextBase.Get<ErrorLogContext>())
            {
                var query = (from l in db.Logs select l)
                                .OrderByDescending(l => l.Id)
                                .Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize);
                return query.ToList();
            }
        }
    }

    internal class ErrorLogContext : DbContextBase
    {
        #region ctor

        public ErrorLogContext()
            : base()
        {

        }

        public ErrorLogContext(DbConnection conn)
            : base(conn)
        {

        }

        #endregion

        public DbSet<ErrorLog> Logs { get; set; }
    }
}
