using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Demo.Authentication.Authorization
{
    public class AgeAuthorizationHandler : AuthorizationHandler<MinAgeRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            MinAgeRequirement requirement)
        {
            if (int.TryParse(context.User.Claims.FirstOrDefault(c => c.Type == AppClaimTypes.Age)?.Value,
                out var age))
                if (age >= requirement.MinAge)
                    context.Succeed(requirement);
                else
                    context.Fail();
        }
    }
}