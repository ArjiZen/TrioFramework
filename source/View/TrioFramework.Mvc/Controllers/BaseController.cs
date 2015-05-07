using System.Web.Mvc;
using System;

namespace Bingosoft.TrioFramework.Mvc.Controllers
{
    /// <summary>
    /// Controller基类
    /// </summary>
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// 当前功能所属模块
        /// </summary>
        /// <value>The name of the module.</value>
        protected abstract string ModuleName { get; }

        /// <summary>
        /// 返回操作成功标示的json格式
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected JsonResult Success(object model = null)
        {
            if (model == null)
            {
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
        protected JsonResult Error(int code, string message)
        {
            return new JsonResult().Error(code, message);
        }

        /// <summary>
        /// 返回Json执行结果
        /// </summary>
        /// <param name="func">逻辑主体.</param>
        protected JsonResult JsonExecutor(Func<object> func)
        {
            return JsonExecutor<object>(func);
        }

        /// <summary>
        /// 返回Json执行结果
        /// </summary>
        /// <param name="func">逻辑主体.</param>
        /// <typeparam name="T">逻辑主体方法返回值类型.</typeparam>
        protected JsonResult JsonExecutor<T>(Func<T> func)
        {
            try
            {
                var obj = func.Invoke();
                return Success(obj);
            }
            catch (BusinessException ex)
            {
                return Error(200, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(this.ModuleName, ex, (Request != null ? Request.Params : null));
                return Error(500, ex.Message);
            }
        }

    }
}
