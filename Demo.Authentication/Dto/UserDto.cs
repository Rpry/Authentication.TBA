using System.Collections.Generic;

namespace Demo.Authentication.Dto
{
    public class UserDto
    {
        public bool IsAuthenticated { get; set; }

        public string Scheme { get; set; }

        public List<object> Claims { get; set; } = new List<object>();
    }
}