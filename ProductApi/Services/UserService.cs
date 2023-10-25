using System.Collections.Generic;
using System.Linq;
using ProductApi.Models;

namespace ProductApi.Services
{
    public class UserService
    {
        private readonly List<User> _users = new List<User>();
        private readonly AuthenticationService _authenticationService;

        public UserService(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public User Register(string username, string password)
        {
            var salt = Guid.NewGuid().ToString();
            var passwordHash = _authenticationService.HashPassword(password, salt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                PasswordHash = passwordHash,
                Salt = salt
            };

            _users.Add(user);

            return user;
        }

        public User GetByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username) ?? throw new Exception("User not found");
        }

        public bool Authenticate(string username, string password)
        {
            User user;
            try
            {
                user = GetByUsername(username);
            }
            catch (Exception)
            {
                return false;
            }

            if (user == null || user.Salt == null)
            {
                return false;
            }

            var hashedPassword = _authenticationService.HashPassword(password, user.Salt);
            return hashedPassword == user.PasswordHash;
        }
    }
}