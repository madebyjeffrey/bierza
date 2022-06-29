using Bierza.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Bierza.Server.Helpers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        UserBaseModel? user = (UserBaseModel?) context.HttpContext.Items["User"];
        
        if (user == null)
        {
            context.Result = new JsonResult(new {message = "Unauthorized"})
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }
    }
}