using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSample.Areas.WorkSpace.Controllers
{
    /// <summary>
    /// 流程已办事项及相关
    /// </summary>
    /// <remarks>
    /// 这里仅用作跳转，根据对应的流程类型，跳转到对应的已办页面
    /// </remarks>
    public class MyDoneController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

    }
}
