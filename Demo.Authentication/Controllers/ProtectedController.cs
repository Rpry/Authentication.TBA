using System.Linq;
using Demo.Authentication.Authorization;
using Demo.Authentication.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Authentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProtectedController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("method")]
        public JsonResult Method()
        {
            return new JsonResult("Успех!");
        }
        
        [Authorize]
        //[Authorize(AuthenticationSchemes = "MyCustomScheme")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Policies.RequireAge18Plus)]
        [HttpPost("methodRequiringAuthorization")]
        public JsonResult MethodRequiringAuthorization()
        {
            //PassThroughAuthorizationHandler
            return new JsonResult("Успех!");
        }

        [HttpPost("GetUserInfo")]
        public UserDto GetUserInfo()
        {
            return new UserDto
            {
                Scheme = HttpContext.User.Identity.AuthenticationType,
                IsAuthenticated = HttpContext.User.Identity.IsAuthenticated,
                Claims = HttpContext
                    .User
                    .Claims
                    .Select(claim => (object) new
                    {
                        claim.Type,
                        claim.Value
                    })
                    .ToList()
            };
        }
    }
}
