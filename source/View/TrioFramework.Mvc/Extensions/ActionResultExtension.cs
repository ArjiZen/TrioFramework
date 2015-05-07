using System.Text;
using System.Web.Mvc;
using Bingosoft.TrioFramework.Communication;

// ReSharper disable once CheckNamespace
namespace Bingosoft.TrioFramework.Mvc
{
    /// <summary>
    /// JsonResult扩展类
    /// </summary>
    public static class JsonResultExtension
    {
        /// <summary>
        /// 返回操作成功的结果数据
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static JsonResult Succeed(this JsonResult result)
        {
            result.ContentEncoding = Encoding.UTF8;
            result.Data = TrioMessage.Succeed();
            return result;
        }

        /// <summary>
        /// 返回操作成功的结果数据
        /// </summary>
        /// <param name="result"></param>
        /// <param name="data">附带数据</param>
        /// <returns></returns>
        public static JsonResult Succeed(this JsonResult result, object data)
        {
            result.ContentEncoding = Encoding.UTF8;
            result.Data = TrioMessage.Succeed(data);
            return result;
        }

        /// <summary>
        /// 返回操作出错的结果数据
        /// </summary>
        /// <param name="result"></param>
        /// <param name="code">错误码</param>
        /// <param name="message">错误信息</param>
        /// <returns></returns>
        public static JsonResult Error(this JsonResult result, int code, string message)
        {
            result.ContentEncoding = Encoding.UTF8;
            result.Data = TrioMessage.Error(code, message);
            return result;
        }
    }
}