using Microsoft.AspNetCore.Mvc.Filters;

namespace ePizzaHub.UI.Helpers
{
    public class ActivityLogFilter : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var user = context.HttpContext.User;
            string conrollername = context.Controller.ToString();
            string actionName = context.ActionDescriptor.DisplayName;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            string conrollername = context.Controller.ToString();
            string actionName = context.ActionDescriptor.DisplayName;
        }
    }
}
