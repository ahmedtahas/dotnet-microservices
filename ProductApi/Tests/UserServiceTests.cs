using NUnit.Framework;
using ProductApi.Services;
using Moq;
using Microsoft.Extensions.Configuration;


namespace ProductApi.Tests.Services
{
    public class UserServiceTests
    {
        [Test]
        public void Authenticate_UserDoesNotExist_ReturnsFalse()
        {
            var configMock = new Mock<IConfiguration>();

            var userService = new UserService(new AuthenticationService(configMock.Object));
            var result = userService.Authenticate("nonexistentUser", "password");
            Assert.IsFalse(result);
        }

        [Test]
        public void Authenticate_UserExistsButSaltIsNull_ReturnsFalse()
        {
           var configMock = new Mock<IConfiguration>();

            var userService = new UserService(new AuthenticationService(configMock.Object));
            var user = userService.Register("existingUser", "password");
            user.Salt = null;
        }
    }
}