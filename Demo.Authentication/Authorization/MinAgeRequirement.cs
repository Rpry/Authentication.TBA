using Microsoft.AspNetCore.Authorization;

namespace Demo.Authentication.Authorization
{
    public class MinAgeRequirement : IAuthorizationRequirement
    {
        public MinAgeRequirement(int minAge)
        {
            MinAge = minAge;
        }

        public int MinAge { get; }
    }
}