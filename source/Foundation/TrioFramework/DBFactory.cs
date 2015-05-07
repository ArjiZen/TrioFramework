using Bingosoft.Data;

namespace Bingosoft.TrioFramework
{
    /// <summary>
    /// 数据工厂
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class DBFactory
    {
        /// <summary>
        /// 数据库操作对象
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static Dao DB
        {
            get { return Dao.Get(SettingProvider.Common.DbName); }
        }

        /// <summary>
        /// 流程类数据库操作对象
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static Dao WorkflowDB
        {
            get { return Dao.Get(SettingProvider.Workflow.DbName); }
        }
    }
}
