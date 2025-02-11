using EquipmentManagement.Data;
using EquipmentManagement.Models;
using EquipmentManagement.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EquipmentManagement.Tests
{
    public class DeviceRepositoryTests
    {
        private readonly DeviceRepository _repository;
        private readonly ApplicationDbContext _context;

        public DeviceRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "EquipmentManagementDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new DeviceRepository(_context);
        }

        // This method will ensure that the database is cleared before each test
        private void ClearDatabase()
        {
            _context.Devices.RemoveRange(_context.Devices);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllDevices_WhenDevicesExist()
        {
            // Arrange: Clear any existing data and add new devices
            ClearDatabase();
            var devices = new List<Device>
            {
                new Device { Name = "Device1", SerialNumber = "1234" },
                new Device { Name = "Device2", SerialNumber = "5678" }
            };

            await _context.Devices.AddRangeAsync(devices);
            await _context.SaveChangesAsync();

            // Act: Call the method
            var result = await _repository.GetAllAsync();

            // Assert: Verify the result
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDevice_WhenDeviceExists()
        {
            // Arrange: Clear any existing data and add a new device
            ClearDatabase();
            var device = new Device { Name = "Device1", SerialNumber = "1234" };
            await _context.Devices.AddAsync(device);
            await _context.SaveChangesAsync();

            // Act: Call the method
            var result = await _repository.GetByIdAsync(device.Id);

            // Assert: Verify the result
            Assert.NotNull(result);
            Assert.Equal("Device1", result.Name);
        }

        [Fact]
        public async Task AddAsync_AddsDevice_WhenDeviceIsValid()
        {
            // Arrange: Clear any existing data and prepare test data
            ClearDatabase();
            var device = new Device { Name = "Device1", SerialNumber = "1234" };

            // Act: Call the method
            await _repository.AddAsync(device);

            // Assert: Verify that the device was added
            var result = await _context.Devices.FindAsync(device.Id);
            Assert.NotNull(result);
            Assert.Equal("Device1", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesDevice_WhenDeviceExists()
        {
            // Arrange: Clear any existing data, add a device, and then update it
            ClearDatabase();
            var device = new Device { Name = "Device1", SerialNumber = "1234" };
            await _context.Devices.AddAsync(device);
            await _context.SaveChangesAsync();

            // Act: Update the device
            device.Name = "Updated Device";
            await _repository.UpdateAsync(device);

            // Assert: Verify that the device was updated
            var updatedDevice = await _context.Devices.FindAsync(device.Id);
            Assert.NotNull(updatedDevice);
            Assert.Equal("Updated Device", updatedDevice.Name);
        }

        [Fact]
        public async Task DeleteAsync_DeletesDevice_WhenDeviceExists()
        {
            // Arrange: Clear any existing data, add a device, and then delete it
            ClearDatabase();
            var device = new Device { Name = "Device1", SerialNumber = "1234" };
            await _context.Devices.AddAsync(device);
            await _context.SaveChangesAsync();

            // Act: Call the delete method
            await _repository.DeleteAsync(device.Id);

            // Assert: Verify that the device was deleted
            var deletedDevice = await _context.Devices.FindAsync(device.Id);
            Assert.Null(deletedDevice);
        }
    }
}
