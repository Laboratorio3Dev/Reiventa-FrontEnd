using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebBackOffice.Infrastructure
{
    public class BasePageModel : PageModel
    {
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var token = context.HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToPageResult("/BackOffice/Login");
            }
            base.OnPageHandlerExecuting(context);
        }
    }
}
