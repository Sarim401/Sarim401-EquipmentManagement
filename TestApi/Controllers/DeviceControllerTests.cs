using EquipmentManagement.Controllers;
using EquipmentManagement.Models;
using EquipmentManagement.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace EquipmentManagement.Tests
{
    public class DeviceControllerTests
    {
        private readonly Mock<IDeviceRepository> _repositoryMock;
        private readonly DeviceController _controller;

        public DeviceControllerTests()
        {
            _repositoryMock = new Mock<IDeviceRepository>();
            _controller = new DeviceController(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenDevicesExist()
        {
            // Arrange
            var devices = new List<Device>
        {
            new Device { Id = 1, Name = "Device1", Category = "Laptop", SerialNumber = "SN123", PurchaseDate = DateTime.Now },
            new Device { Id = 2, Name = "Device2", Category = "Phone", SerialNumber = "SN456", PurchaseDate = DateTime.Now }
        };

            _repositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(devices);


            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkMTYxZDJmMC1mY2YxLTQwOTEtYTI3Ni04YmY2OWM1NDM5MjQiLCJlbWFpbCI6Im5ld3VzZXIxQGV4YW1wbGUuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6IlBhd2VsIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJleHAiOjE3MzkyOTI4ODgsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjUyMjMiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo1MjIzIn0.GC_ddTdzIHWLhgytyyAkR6lKxwhMTXMmB8fq_PA7UvI";
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            var claimsIdentity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, "testUser"),
            new Claim(ClaimTypes.Role, "Admin")
        }, "Bearer");  // Określamy typ autoryzacji
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            mockHttpContext.User = claimsPrincipal; 

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext
            };

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);  
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Device>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }



        [Fact]
        public async Task GetById_ReturnsOk_WhenDeviceExists()
        {
            // Arrange
            var device = new Device { Id = 1, Name = "Device1", Category = "Laptop", SerialNumber = "SN123", PurchaseDate = DateTime.Now };
            _repositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(device);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Device>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("Device1", returnValue.Name);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenDeviceDoesNotExist()
        {
            // Arrange
            _repositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Device)null);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenDeviceIsValid()
        {
            // Arrange
            var device = new Device
            {
                Id = 1,
                Name = "Device1",
                Category = "Laptop",
                SerialNumber = "SN123",
                PurchaseDate = DateTime.Now
            };
            _repositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Device>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(device);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(DeviceController.GetById), createdResult.ActionName);
            var returnValue = Assert.IsType<Device>(createdResult.Value);
            Assert.Equal(device.Name, returnValue.Name);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenDeviceIsValid()
        {
            // Arrange
            var device = new Device
            {
                Id = 1,
                Name = "Updated Device",
                Category = "Laptop",
                SerialNumber = "SN123",
                PurchaseDate = DateTime.Now
            };
            _repositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Device>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(1, device);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenDeviceIdDoesNotMatch()
        {
            // Arrange
            var device = new Device
            {
                Id = 2,
                Name = "Updated Device",
                Category = "Laptop",
                SerialNumber = "SN123",
                PurchaseDate = DateTime.Now
            };

            // Act
            var result = await _controller.Update(1, device);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeviceIsDeleted()
        {
            // Arrange
            _repositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
