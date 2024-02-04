using AutoMapper;
using CompanyApp.DTO;
using CompanyApp.Interfaces;
using CompanyApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace CompanyApp.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class TrucksController : Controller {
		private readonly ITruckRepository _truckRepository;
		private readonly IMapper _mapper;

		public TrucksController(ITruckRepository truckRepository, IMapper mapper) {
			_truckRepository = truckRepository;
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<Truck>))]
		[ProducesResponseType(400)]
		public IActionResult GetTrucks([FromQuery] string? sort, [FromQuery] string? code, [FromQuery] string? name, [FromQuery] int? status) {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var trucks = _truckRepository.GetTrucks(sort ?? "");

			if (!string.IsNullOrEmpty(code))
				trucks = trucks.Where(t => t.Code.Contains(code, StringComparison.OrdinalIgnoreCase)).ToList();

			if (!string.IsNullOrEmpty(name))
				trucks = trucks.Where(t => t.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

			if (status.HasValue)
				trucks = trucks.Where(t => (int)t.Status == status.Value).ToList();

			return Ok(trucks);
		}

		[HttpGet("{truckId}")]
		[ProducesResponseType(200, Type = typeof(Truck))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public IActionResult GetTruck(int truckId) {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (!_truckRepository.TruckExists(truckId))
				return NotFound();

			var truck = _truckRepository.GetTruck(truckId);

			return Ok(truck);
		}

		[HttpGet("{truckId}/description")]
		[ProducesResponseType(200, Type = typeof(string))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public IActionResult GetTruckDescription(int truckId) {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (!_truckRepository.TruckExists(truckId))
				return NotFound();

			var description = _truckRepository.GetDescription(truckId);

			return Ok(description);
		}

		[HttpGet("{truckId}/status")]
		[ProducesResponseType(200, Type = typeof(string))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public IActionResult GetTruckStatus(int truckId) {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var truck = _truckRepository.GetTruck(truckId);

			if (truck == null)
				return NotFound();

			string statusString = truck.GetStatusAsString();

			if (string.IsNullOrEmpty(statusString))
				return NoContent();

			return Ok(statusString);
		}

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(422)]
		[ProducesResponseType(500)]
		public IActionResult CreateTruck([FromBody] TruckDTO truckCreate) {
			if (truckCreate == null)
				return BadRequest(ModelState);

			if (truckCreate.Code == null || truckCreate.Name == null || truckCreate.Status == null) {
				ModelState.AddModelError("errorCreate", "Code, name and status are required.");
				return BadRequest(ModelState);
			}

			truckCreate.Code = truckCreate.Code.Trim();
			var truck = _truckRepository.GetTrucks()
				.Where(t => t.Code.ToLower() == truckCreate.Code.ToLower())
				.FirstOrDefault();
			if (truck != null) {
				ModelState.AddModelError("errorCode", "Truck with the specified code already exists.");
				return StatusCode(400, ModelState);
			}

			truckCreate.Name = truckCreate.Name.Trim();

			var truckMap = _mapper.Map<Truck>(truckCreate);

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (!_truckRepository.CreateTruck(truckMap)) {
				ModelState.AddModelError("errorCreate", "Failed to save the truck.");
				return StatusCode(500, ModelState);
			}

			return Ok("Truck successfully created.");
		}

		[HttpPut("{truckId}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(422)]
		[ProducesResponseType(500)]
		public IActionResult UpdateTruck(int truckId, [FromBody] TruckDTO truckUpdate) {
			if (truckUpdate == null)
				return BadRequest(ModelState);

			if (!_truckRepository.TruckExists(truckId))
				return NotFound();

			if (truckUpdate.Code != null) {
				var truck = _truckRepository.GetTrucks()
					.Where(t => t.Code.Equals(truckUpdate.Code, StringComparison.CurrentCultureIgnoreCase) && t.Id != truckId)
					.FirstOrDefault();

				if (truck != null) {
					ModelState.AddModelError("codeUpdate", "Truck with the specified code already exists.");
					return StatusCode(422, ModelState);
				}
			}

			if (truckUpdate.Name != null)
				truckUpdate.Name = truckUpdate.Name.Trim();

			if (truckUpdate.Status != null && !_truckRepository.IsStatusUpdateValid(truckId, (int)truckUpdate.Status)) {
				ModelState.AddModelError("statusUpdate", "Invalid status transition. The allowed transitions are: (1) Loading -> (2) To Job -> (3) At Job -> (4) Returning -> (1) Loading");
				return StatusCode(400, ModelState);
			}

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (!_truckRepository.UpdateTruck(truckId, truckUpdate)) {
				ModelState.AddModelError("errorUpdate", "Failed to update the truck.");
				return StatusCode(500, ModelState);
			}

			return NoContent();
		}

		[HttpPut("{truckId}/changeStatus")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(422)]
		[ProducesResponseType(500)]
		public IActionResult ChangeTruckStatus(int truckId, [FromQuery] int newStatus) {
			if (!_truckRepository.TruckExists(truckId))
				return NotFound();

			if (!Enum.IsDefined(typeof(TruckStatus), newStatus)) {
				ModelState.AddModelError("statusUpdate", "Invalid truck status. Accepted values are: 0 (Out of Service), 1 (Loading), 2 (To Job), 3 (At Job), 4 (Returning).");
				return StatusCode(422, ModelState);
			}

			var truckUpdate = new TruckDTO { Status = (TruckStatus)newStatus };

			if (!_truckRepository.IsStatusUpdateValid(truckId, (int)truckUpdate.Status)) {
				ModelState.AddModelError("statusUpdate", "Invalid status transition. The allowed transitions are: (1) Loading -> (2) To Job -> (3) At Job -> (4) Returning -> (1) Loading");
				return StatusCode(400, ModelState);
			}

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (!_truckRepository.UpdateTruck(truckId, truckUpdate)) {
				ModelState.AddModelError("errorUpdate", "Failed to update the status of the truck.");
				return StatusCode(500, ModelState);
			}

			return NoContent();
		}

		[HttpDelete("{truckId}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public IActionResult DeleteTruck(int truckId) {
			var truckToDelete = _truckRepository.GetTruck(truckId);
			if (truckToDelete == null)
				return NotFound();

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (!_truckRepository.DeleteTruck(truckToDelete)) {
				ModelState.AddModelError("errorDelete", "Failed to delete the truck.");
				return StatusCode(500, ModelState);
			}

			return NoContent();
		}
	}
}
