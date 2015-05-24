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

    }
}
