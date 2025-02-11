using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using EquipmentManagement.Models;
using EquipmentManagement.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace EquipmentManagement.Tests
{
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            // Mockowanie IOptions<JwtSettings>
            var mockJwtSettings = new Mock<IOptions<JwtSettings>>();
            mockJwtSettings.Setup(s => s.Value).Returns(new JwtSettings
            {
                Secret = "TwojBardzoTajnyKluczJWT2025@1234567890!",
                Issuer = "test",
                Audience = "test"
            });

            // Przekazujemy mocka IOptions<JwtSettings> do JwtService
            _jwtService = new JwtService(mockJwtSettings.Object);
        }

        [Fact]
        public void GenerateToken_ShouldReturn_ValidJwtToken()
        {
            // Arrange
            var userId = "1";
            var userName = "testuser";
            var email = "test@example.com";
            var roles = new List<string> { "User" };

            // Act
            var token = _jwtService.GenerateToken(userId, userName, email, roles);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Sprawdzenie, czy token zawiera odpowiednie roszczenia
            var nameIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            Assert.Equal(userId, nameIdClaim);
            Assert.Equal(userName, nameClaim);
            Assert.Equal(email, emailClaim);
        }

        [Fact]
        public void GenerateToken_ShouldContain_CorrectExpirationTime()
        {
            // Arrange
            var userId = "1";
            var userName = "testuser";
            var email = "test@example.com";
            var roles = new List<string> { "User" };

            // Act
            var token = _jwtService.GenerateToken(userId, userName, email, roles);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Compare the expiration time with DateTime.UtcNow + 1 hour
            var expectedExpiration = DateTime.UtcNow.AddHours(1);

            Assert.True(jwtToken.ValidTo > DateTime.UtcNow);  // Token should not expire in the past
            Assert.True(jwtToken.ValidTo < expectedExpiration); // Token should expire within an hour
        }


    }
}
