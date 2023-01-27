using System.Collections.Generic;

namespace Demo.Authentication.Data
{
    public class Database
    {
        private readonly List<string> _tokens = new List<string>();

        private readonly Dictionary<string, string> _userToPassword = new Dictionary<string, string>
        {
            { "admin", "admin" },
            { "user", "user" },
            { "123", "123" }
        };

        public bool Login(string login, string password)
        {
            var userFound = this._userToPassword.TryGetValue(login, out var storedPassword);

            if (!userFound)
            {
                return false;
            }

            return storedPassword == password;
        }

        public void AddToken(string token) => this._tokens.Add(token);

        public void RemoveToken(string token) => this._tokens.Remove(token);
    }
}
