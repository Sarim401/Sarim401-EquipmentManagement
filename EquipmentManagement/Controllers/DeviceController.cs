using EquipmentManagement.Models;
using EquipmentManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;


namespace EquipmentManagement.Controllers
{
    [Route("api/devices")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceRepository _repository;

        public DeviceController(IDeviceRepository repository)
        {
            _repository = repository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var token = Request.Headers["Authorization"].ToString();
            Console.WriteLine($"Token: {token}");
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Not authenticated");
            }

            var devices = await _repository.GetAllAsync();
            return Ok(devices);
        }


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
