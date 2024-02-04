using CompanyApp.Data;
using CompanyApp.DTO;
using CompanyApp.Interfaces;
using CompanyApp.Models;

namespace CompanyApp.Repository {
	public class TruckRepository(DataContext context) : ITruckRepository {
		private readonly DataContext _context = context;

		public ICollection<Truck> GetTrucks() {
			return _context.Trucks.OrderBy(t => t.Id).ToList();
		}

		public ICollection<Truck> GetTrucks(string sort) {
			var trucks = _context.Trucks.AsQueryable();

			trucks = sort.ToLowerInvariant() switch {
				"-id" => trucks.OrderByDescending(t => t.Id),
				"code" => trucks.OrderBy(t => t.Code),
				"-code" => trucks.OrderByDescending(t => t.Code),
				"name" => trucks.OrderBy(t => t.Name),
				"-name" => trucks.OrderByDescending(t => t.Name),
				"status" => trucks.OrderBy(t => t.Status),
				"-status" => trucks.OrderByDescending(t => t.Status),
				_ => trucks.OrderBy(t => t.Id),
			};
			return trucks.ToList();
		}

		public Truck? GetTruck(int id) {
			var truck = _context.Trucks.Where(t => t.Id == id).FirstOrDefault();

			return truck ?? null;
		}

		public Truck? GetTruck(string name) {
			var truck = _context.Trucks.Where(t => t.Name == name).FirstOrDefault();

			return truck ?? null;
		}

		public string? GetDescription(int id) {
			var truck = _context.Trucks.FirstOrDefault(t => t.Id == id);

			if (truck != null && !string.IsNullOrEmpty(truck.Description))
				return truck.Description;

			return null;
		}

		public bool TruckExists(int id) {
			return _context.Trucks.Any(t => t.Id == id);
		}

		public bool CreateTruck(Truck truck) {
			_context.Add(truck);
			return Save();
		}

		public bool UpdateTruck(int id, TruckDTO truckUpdate) {
			var truck = _context.Trucks.FirstOrDefault(t => t.Id == id);

			if (truck != null) {
				if (truckUpdate.Name != null && truck.Name != truckUpdate.Name)
					truck.Name = truckUpdate.Name;
				if (truckUpdate.Code != null && truck.Code != truckUpdate.Code)
					truck.Code = truckUpdate.Code;
				if (truckUpdate.Status != null && truck.Status != truckUpdate.Status)
					if (IsStatusUpdateValid(id, (int)truckUpdate.Status))
						truck.Status = (TruckStatus)truckUpdate.Status;
					else
						return false;
				if (truckUpdate.Description != null && truck.Description != truckUpdate.Description)
					truck.Description = truckUpdate.Description;

				Save();
				return true;
			}

			return false;
		}

		public bool DeleteTruck(Truck truck) {
			_context.Remove(truck);
			return Save();
		}

		public bool IsStatusUpdateValid(int id, int truckStatus) {
			var truck = _context.Trucks.FirstOrDefault(t => t.Id == id);

			if (truck != null)
				if (Enum.IsDefined(typeof(TruckStatus), truckStatus))
					if (truck.Status == 0 || truckStatus == 0 || truckStatus == ((int)truck.Status % 4) + 1)
						return true;

			return false;
		}

		public bool Save() {
			return _context.SaveChanges() > 0;
		}
	}
}
