using Microsoft.AspNetCore.Authorization;

namespace Demo.Authentication.Authorization
{
    public class OneTimeJwtTokenRequirement : IAuthorizationRequirement
    {
        public OneTimeJwtTokenRequirement(string redisKeyPrefix)
        {
            RedisKeyPrefix = redisKeyPrefix;
        }

        public string RedisKeyPrefix { get; }
    }
}