using System.Data.Common;
using System.Data.Entity;

namespace Bingosoft.TrioFramework.DB
{
    /// <summary>
    /// DBContext基类
    /// </summary>
    public abstract class DbContextBase : DbContext
    {
        #region ctor

        /// <summary>
        /// 实例化DbContextBase
        /// </summary>
        protected DbContextBase()
            : base()
        {
        }

        /// <summary>
        /// 实例化DbContextBase
        /// </summary>
        /// <param name="conn"></param>
        protected DbContextBase(DbConnection conn)
            : base(conn, true)
        {

        }

        #endregion

        /// <summary>
        /// 获取数据库操作上下文
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>(string configurationName = "") where T : DbContextBase
        {
            if (string.IsNullOrWhiteSpace(configurationName))
            {
                configurationName = SettingProvider.Db.SystemDb;
            }
            var conn = DBFactory.GetConnection(configurationName);
            return typeof(T).Create<T>(conn);
        }

    }
}
