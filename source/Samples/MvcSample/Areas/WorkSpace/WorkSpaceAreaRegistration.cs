using System.Web.Mvc;

namespace MvcSample.Areas.WorkSpace
{
    public class WorkSpaceAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WorkSpace";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WorkSpace_default",
                "WorkSpace/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
