using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minimal_api.Domain.DTO;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.Services;

namespace minimal_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] VehiclesDTO dto)
        {
            var vehicle = new Vehicle
            {
                Name = dto.Name,
                Mark = dto.Mark,
                Year = dto.Year
            };

            _vehicleService.IncludeVehicle(vehicle);
            return Created($"/api/vehicles/{vehicle.Id}", vehicle);
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] int? page)
        {
            var vehicles = _vehicleService.ListVehicles(page);
            if (!vehicles.Any()) return NotFound("Vehicle not found.");

            return Ok(vehicles);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var vehicle = _vehicleService.GetById(id);
            if (vehicle == null) return NotFound("Vehicle not found.");
            return Ok(vehicle);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] VehiclesDTO dto)
        {
            var vehicle = _vehicleService.GetById(id);
            if (vehicle == null) return NotFound("Vehicle not found.");

            vehicle.Name = dto.Name;
            vehicle.Mark = dto.Mark;
            vehicle.Year = dto.Year;

            _vehicleService.UpdateVehicle(vehicle);
            return Ok(vehicle);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var vehicle = _vehicleService.GetById(id);
            if (vehicle == null) return NotFound("Vehicle not found.");

            _vehicleService.DeleteVehicle(id);
            return Ok("Vehicle successfully deleted.");
        }
    }
}
