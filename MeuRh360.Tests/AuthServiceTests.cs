using Application.Services;
using Application.Interfaces;
using Domain.DTOs.Auth;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Threading;
using System.Security.Claims;
using System.Linq;

namespace MeuRh360.Tests
{
    public class AuthServiceTests
    {
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private Mock<IConfiguration> _configMock;
        private AuthService _authService;

        public AuthServiceTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(c => c["Jwt:Key"]).Returns("testkeytestkeytestkeytestkey123456");
            _authService = new AuthService(_userManagerMock.Object, _roleManagerMock.Object, _configMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsSuccess_WhenUserCreated()
        {
            var request = new RegisterRequestDto { Email = "test@test.com", Password = "Password123!", Role = "User" };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(x => x.RoleExistsAsync(request.Role)).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), request.Role)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            var (success, errors) = await _authService.RegisterAsync(request);

            Assert.True(success);
            Assert.Empty(errors);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsFailure_WhenUserNotCreated()
        {
            var request = new RegisterRequestDto { Email = "fail@test.com", Password = "Password123!", Role = "User" };
            var identityErrors = new List<IdentityError> { new IdentityError { Description = "Error" } };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
                .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            var (success, errors) = await _authService.RegisterAsync(request);

            Assert.False(success);
            Assert.Contains("Error", errors);
        }

        [Fact]
        public async Task LoginAsync_ReturnsSuccess_WhenCredentialsAreValid()
        {
            var request = new LoginRequestDto { Email = "test@test.com", Password = "Password123!" };
            var user = new ApplicationUser { Email = request.Email, Id = "1" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, request.Password)).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetClaimsAsync(user)).ReturnsAsync(new List<Claim>());
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            var (success, token, roles) = await _authService.LoginAsync(request);

            Assert.True(success);
            Assert.NotNull(token);
            Assert.Contains("User", roles);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailure_WhenUserNotFound()
        {
            var request = new LoginRequestDto { Email = "notfound@test.com", Password = "Password123!" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser)null);

            var (success, token, roles) = await _authService.LoginAsync(request);

            Assert.False(success);
            Assert.Null(token);
            Assert.Null(roles);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailure_WhenPasswordInvalid()
        {
            var request = new LoginRequestDto { Email = "test@test.com", Password = "wrongpassword" };
            var user = new ApplicationUser { Email = request.Email, Id = "1" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, request.Password)).ReturnsAsync(false);

            var (success, token, roles) = await _authService.LoginAsync(request);

            Assert.False(success);
            Assert.Null(token);
            Assert.Null(roles);
        }
    }
} 