using EquipmentManagement.Models;
using EquipmentManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagement.Controllers
{
    [Route("api/devices")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceRepository _repository;

        public DeviceController(IDeviceRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _repository.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var device = await _repository.GetByIdAsync(id);
            return device == null ? NotFound() : Ok(device);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Device device)
        {
            await _repository.AddAsync(device);
            return CreatedAtAction(nameof(GetById), new { id = device.Id }, device);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Device device)
        {
            if (id != device.Id) return BadRequest();
            await _repository.UpdateAsync(device);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
