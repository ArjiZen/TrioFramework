using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSample.Areas.WorkSpace.Controllers
{
    /// <summary>
    /// 发起新流程及相关
    /// </summary>
    public class NewWorkflowController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

    }
}
