using System.Web.Mvc;

namespace MvcSample.Areas.Member.Controllers
{
    /// <summary>
    /// 账户及相关
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SignInView()
        {
            return View();
        }

        /// <summary>
        /// 登录操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SignIn()
        {
            return View();
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public ActionResult SignOut()
        {
            return View();
        }
    }
}
