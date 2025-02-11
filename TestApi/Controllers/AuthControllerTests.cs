using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using EquipmentManagement.Controllers;
using EquipmentManagement.Models;
using EquipmentManagement.Services;
using Microsoft.AspNetCore.Http;

namespace EquipmentManagement.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            // Mockowanie UserManager
            _userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                null, null, null, null, null, null, null, null);

            // Mockowanie SignInManager
            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                null, null, null, null);

            // Mockowanie JwtService
            _jwtServiceMock = new Mock<IJwtService>();
            _jwtServiceMock.Setup(s => s.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .Returns("mockToken123");

            // Tworzymy AuthController z mockowanymi zależnościami
            _authController = new AuthController(_userManagerMock.Object, _signInManagerMock.Object, _jwtServiceMock.Object);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenUserIsCreated()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "Test@123",
                Role = "User"
            };

            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authController.Register(registerModel);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUserCreationFails()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "Test@123",
                Role = "User"
            };

            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

            // Act
            var result = await _authController.Register(registerModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsAssignableFrom<IEnumerable<IdentityError>>(badRequestResult.Value);
            Assert.Single(errors);
            Assert.Equal("Error", errors.First().Description);
        }

        [Fact]
        public async Task Login_ReturnsOk_WithValidToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "test@example.com",
                Password = "Test@123"
            };

            var user = new User { Id = "1", Email = loginModel.Email, UserName = "testuser" };

            _userManagerMock.Setup(u => u.FindByEmailAsync(loginModel.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, loginModel.Password)).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await _authController.Login(loginModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var token = okResult.Value?.GetType().GetProperty("Token")?.GetValue(okResult.Value, null) as string;
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenUserNotFound()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "wrong@example.com",
                Password = "WrongPassword"
            };

            _userManagerMock.Setup(u => u.FindByEmailAsync(loginModel.Email)).ReturnsAsync((User)null);

            // Act
            var result = await _authController.Login(loginModel);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenPasswordIsIncorrect()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            var user = new User { Id = "1", Email = loginModel.Email, UserName = "testuser" };

            _userManagerMock.Setup(u => u.FindByEmailAsync(loginModel.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, loginModel.Password)).ReturnsAsync(false);

            // Act
            var result = await _authController.Login(loginModel);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials", unauthorizedResult.Value);
        }
        [Fact]
        public async Task RefreshToken_ReturnsUnauthorized_WhenTokenDoesNotMatch()
        {
            // Arrange
            var request = new RefreshTokenRequest
            {
                UserId = "validUser",
                RefreshToken = "invalidToken"
            };

            var user = new User
            {
                Id = request.UserId,
                RefreshToken = "validToken",
                RefreshTokenExpiration = DateTime.Now.AddDays(1)
            };

            _userManagerMock.Setup(um => um.FindByIdAsync(request.UserId)).ReturnsAsync(user);

            // Act
            var result = await _authController.RefreshToken(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);

            // Pobierz właściwość 'message' przez refleksję
            var value = unauthorizedResult.Value;
            var messageProperty = value.GetType().GetProperty("message");
            Assert.NotNull(messageProperty);
            var messageValue = messageProperty.GetValue(value);
            Assert.Equal("Invalid refresh token", messageValue);
        }

        [Fact]
        public async Task RefreshToken_ReturnsUnauthorized_WhenTokenExpired()
        {
            // Arrange
            var request = new RefreshTokenRequest
            {
                UserId = "validUser",
                RefreshToken = "validToken"
            };

            var user = new User
            {
                Id = request.UserId,
                RefreshToken = request.RefreshToken,
                RefreshTokenExpiration = DateTime.Now.AddDays(-1)
            };

            _userManagerMock.Setup(um => um.FindByIdAsync(request.UserId)).ReturnsAsync(user);

            // Act
            var result = await _authController.RefreshToken(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);

            // Pobierz właściwość 'message' przez refleksję
            var value = unauthorizedResult.Value;
            var messageProperty = value.GetType().GetProperty("message");
            Assert.NotNull(messageProperty);
            var messageValue = messageProperty.GetValue(value);
            Assert.Equal("Invalid refresh token", messageValue);
        }

        [Fact]
        public async Task RefreshToken_ReturnsOkWithNewTokens_WhenValidRequest()
        {
            // Arrange
            var request = new RefreshTokenRequest
            {
                UserId = "validUser",
                RefreshToken = "validToken"
            };

            var user = new User
            {
                Id = request.UserId,
                UserName = "testuser",
                Email = "test@example.com",
                RefreshToken = request.RefreshToken,
                RefreshTokenExpiration = DateTime.Now.AddDays(1)
            };

            _userManagerMock.Setup(um => um.FindByIdAsync(request.UserId))
                .ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "User" });
            _userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            _jwtServiceMock.Setup(s => s.GenerateToken(user.Id, user.UserName, user.Email, It.IsAny<List<string>>()))
                .Returns("newToken123");
            _jwtServiceMock.Setup(s => s.GenerateRefreshToken())
                .Returns("newRefreshToken123");

            // Act
            var result = await _authController.RefreshToken(request);

            // Assert
            // Sprawdzenie odpowiedzi
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("newToken123", okResult.Value.GetType().GetProperty("Token").GetValue(okResult.Value));
            Assert.Equal("newRefreshToken123", okResult.Value.GetType().GetProperty("RefreshToken").GetValue(okResult.Value));

            // Sprawdzenie aktualizacji użytkownika
            _userManagerMock.Verify(um => um.UpdateAsync(It.Is<User>(u =>
                u.RefreshToken == "newRefreshToken123" &&
                u.RefreshTokenExpiration > DateTime.Now.AddDays(7).AddMinutes(-1) &&
                u.RefreshTokenExpiration < DateTime.Now.AddDays(7).AddMinutes(1)
            )), Times.Once);
        }
    }
}
