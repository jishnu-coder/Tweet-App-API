using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

            if (authFilterContext == null || !context.User.Claims.Any())
            {
                return Task.CompletedTask;
            }

            //Get the userName from the Token claim 
            var loggedUserName = context.User.Claims.First().Value;
            //Get the userName from the url -> Entity trigger the request
            var resourceOwnerUserName = authFilterContext.Request.RouteValues["userName"];

            //if both are equal authorize the request
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
