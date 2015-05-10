using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;

namespace Bingosoft.TrioFramework.DB
{
    /// <summary>
    /// 提供数据库操作链接
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class DBFactory
    {
        static DBFactory()
        {
            var connStrCount = ConfigurationManager.ConnectionStrings.Count;
            for (int i = 0; i < connStrCount; i++)
            {
                var keyValue = ConfigurationManager.ConnectionStrings[i];
                connectionStrs.Add(keyValue.Name, keyValue.ConnectionString);
            }
        }

        /// <summary>
        /// 数据库链接集合
        /// </summary>
        private readonly static IDictionary<string, string> connectionStrs = new Dictionary<string, string>();

        /// <summary>
        /// 获取链接字符串
        /// </summary>
        /// <param name="configurationName">链接字符串配置名称</param>
        /// <returns></returns>
        private static string GetConnectionStr(string configurationName)
        {
            if (connectionStrs.ContainsKey(configurationName))
            {
                return connectionStrs[configurationName];
            }
            else
            {
                throw new KeyNotFoundException(string.Format("未找到名称为{0}的数据库链接字符串配置", configurationName));
            }
        }

        /// <summary>
        /// 获取数据库链接
        /// </summary>
        /// <param name="configurationName"></param>
        /// <returns></returns>
        public static DbConnection GetConnection(string configurationName)
        {
            var connStr = GetConnectionStr(configurationName);
            // 通过反射获取对应数据库的Connection
            var provider = Type.GetType(SettingProvider.Db.ConnectionProvider);
            if (provider == null)
            {
                throw new TypeLoadException(string.Format("加载类型{0}失败", SettingProvider.Db.ConnectionProvider));
            }
            var entity = (DbConnection)provider.Assembly.CreateInstance(provider.FullName);
            if (entity != null)
            {
                entity.ConnectionString = connStr;
            }
            return entity;
            //return new SqlConnection(connStr);
        }
    }
}
