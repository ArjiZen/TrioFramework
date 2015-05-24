using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using Bingosoft.TrioFramework.DB;

namespace Bingosoft.TrioFramework.Log
{
    /// <summary>
    /// 操作日志
    /// </summary>
    [Table("LOG_Operation")]
    public class OperationLog
    {
        #region Properties
        /// <summary>
        /// 日志编号
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
        /// 操作者
        /// </summary>
        public string Actor { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        #endregion

        /// <summary>
        /// 保存操作日志
        /// </summary>
        internal void Save()
        {
            using (var db = DBFactory.Get<OperationLogContext>())
            {
                db.Logs.Add(this);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 获取所有操作日志
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页数据量</param>
        /// <returns></returns>
        public static IList<OperationLog> FindAll(int pageIndex, int pageSize)
        {
            using (var db = DBFactory.Get<OperationLogContext>())
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
    /// 操作日志数据库操作类
    /// </summary>
    public class OperationLogContext : DbContextBase
    {
        #region ctor
        /// <summary>
        /// 实例化操作日志数据库操作类
        /// </summary>
        public OperationLogContext()
        {

        }

        /// <summary>
        /// 实例化操作日志数据库操作类
        /// </summary>
        /// <param name="conn">数据库链接</param>
        public OperationLogContext(DbConnection conn)
            : base(conn)
        {

        }
        #endregion

        /// <summary>
        /// 操作日志集合
        /// </summary>
        public DbSet<OperationLog> Logs { get; set; }
    }
}
