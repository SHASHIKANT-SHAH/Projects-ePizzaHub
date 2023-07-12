using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace ePizzaHub.UI.Helpers
{
    public class CustomAuthorize : Attribute, IAuthorizationFilter
    {
        public String Roles { get; set; }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //check Authentication
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                // TODO: Get UserRoles from DB 

                //check authorization
                if(!context.HttpContext.User.IsInRole(Roles))
                {
                    context.Result = new RedirectToActionResult("UnAuthorize", "Account", new { area = "" });
                }
            }
            else
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
            }
        }
    }
}
