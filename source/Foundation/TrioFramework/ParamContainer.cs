using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bingosoft.Data;

namespace Bingosoft.TrioFramework {
    /// <summary>
    /// 系统参数容器
    /// </summary>
    public static class ParamContainer {

        private static Dao _dao = Dao.Get();
        /// <summary>
        /// 获取系统参数
        /// </summary>
        /// <param name="code">参数编码</param>
        /// <param name="defaultVal">默认值</param>
        /// <returns></returns>
        public static string Get(string code, string defaultVal = "") {
            return _dao.QueryScalar<string>("framework.param.get", new { Code = code }) ?? defaultVal;
        }

        /// <summary>
        /// 更新系统参数
        /// </summary>
        /// <param name="code">参数编码</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public static bool Update(string code, string value) {
            return _dao.ExecuteNonQuery("framework.param.update", new { Code = code, Value = value }) > 0;
        }
    }
}
