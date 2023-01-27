using Microsoft.AspNetCore.Authentication;

namespace Demo.Authentication.Authentication
{
    public class JwtSchemeOptions : AuthenticationSchemeOptions
    {
        public bool IsActive { get; set; }
    }
}
