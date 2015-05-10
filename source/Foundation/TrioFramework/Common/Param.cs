using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using Bingosoft.TrioFramework.DB;

namespace Bingosoft.TrioFramework.Common
{
    /// <summary>
    /// 系统参数
    /// </summary>
    [Table("Sys_Param")]
    public class Param
    {
        #region Properties
        /// <summary>
        /// 参数Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        #endregion

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="code">参数编码</param>
        /// <param name="defaultVal">默认值</param>
        /// <returns></returns>
        public static string Get(string code, string defaultVal = "")
        {
            using (var db = DbContextBase.Get<ParamContext>())
            {
                var query = (from p in db.Params
                             where p.Code.Eq(code)
                             select p).FirstOrDefault();
                return query != null ? query.Value : defaultVal;
            }
        }

        /// <summary>
        /// 更新参数值
        /// </summary>
        /// <param name="code">参数编码</param>
        /// <param name="value">参数值</param>
        public static void Update(string code, string value)
        {
            using (var db = DbContextBase.Get<ParamContext>())
            {
                var query = (from p in db.Params
                             where p.Code.Eq(code)
                             select p).FirstOrDefault();
                if (query != null)
                {
                    query.Value = value;
                    db.SaveChanges();
                }
            }
        }
    }

    /// <summary>
    /// 参数操作上下文
    /// </summary>
    public class ParamContext : DbContextBase
    {
        #region ctor
        public ParamContext(DbConnection conn)
            : base(conn)
        {

        }
        #endregion

        /// <summary>
        /// 参数集合
        /// </summary>
        public DbSet<Param> Params { get; set; }
    }
}
