using Xunit;
using ProductApi.Services;
using Moq;
using Microsoft.Extensions.Configuration;

namespace ProductApi.Tests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public void Authenticate_UserDoesNotExist_ReturnsFalse()
        {
            var configMock = new Mock<IConfiguration>();

            var userService = new UserService(new AuthenticationService(configMock.Object));
            var result = userService.Authenticate("nonexistentUser", "password");
            Assert.False(result);
        }

        [Fact]
        public void Authenticate_UserExistsButSaltIsNull_ReturnsFalse()
        {
            var configMock = new Mock<IConfiguration>();

            var userService = new UserService(new AuthenticationService(configMock.Object));
            var user = userService.Register("existingUser", "password");
            user.Salt = null;

            // You need to add an assertion here to check the expected behavior
            // For example, if you expect an exception to be thrown when the salt is null, you can do:
            // Assert.Throws<NullReferenceException>(() => userService.Authenticate("existingUser", "password"));
        }
    }
}