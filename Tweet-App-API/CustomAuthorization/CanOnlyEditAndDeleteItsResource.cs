using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet_App_API.CustomAuthorization
{
    public class CanOnlyEditAndDeleteItsResource : AuthorizationHandler<ManageUserResourceEdit>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
               ManageUserResourceEdit requirement)
        {
            var authFilterContext = context.Resource as HttpContext;
            if(authFilterContext == null || !context.User.Claims.Any())
            {
                return Task.CompletedTask;
            }

            var loggedUserName = context.User.Claims.First().Value;
            var resourceOwnerUserName = authFilterContext.Request.RouteValues["userName"];
            if (loggedUserName.Equals(resourceOwnerUserName))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            context.Fail();
            return Task.CompletedTask;

        }
    }
}
