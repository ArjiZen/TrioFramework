using System.Web.Mvc;
using Bingosoft.TrioFramework.Mvc.Models;

namespace Bingosoft.TrioFramework.Mvc.Controllers {
    /// <summary>
    /// Controller基类
    /// </summary>
    public class BaseController : Controller {
        /// <summary>
        /// 返回操作成功标示的json格式
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected JsonResult Success(object model = null) {
            if (model == null) {
                return new JsonResult().Succeed();
            }
            return new JsonResult().Succeed(model);
        }

        /// <summary>
        /// 返回操作错误标示的json格式
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected JsonResult Error(int code, string message) {
            return new JsonResult().Error(code, message);
        }
    }
}
