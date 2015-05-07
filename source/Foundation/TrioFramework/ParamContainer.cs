using Bingosoft.Data;

namespace Bingosoft.TrioFramework
{
    /// <summary>
    /// 系统参数容器
    /// </summary>
    public static class ParamContainer
    {
        private static readonly Dao dao = Dao.Get();

        /// <summary>
        /// 获取系统参数
        /// </summary>
        /// <param name="code">参数编码</param>
        /// <param name="defaultVal">默认值</param>
        /// <returns></returns>
        public static string Get(string code, string defaultVal = "")
        {
            return dao.QueryScalar<string>("trio.framework.param.get", new { Code = code }) ?? defaultVal;
        }

        /// <summary>
        /// 更新系统参数
        /// </summary>
        /// <param name="code">参数编码</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public static bool Update(string code, string value)
        {
            return dao.ExecuteNonQuery("trio.framework.param.update", new { Code = code, Value = value }) > 0;
        }
    }
}
