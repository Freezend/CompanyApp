using CompanyApp.DTO;
using CompanyApp.Models;

namespace CompanyApp.Interfaces {
	public interface ITruckRepository {
		ICollection<Truck> GetTrucks();
		ICollection<Truck> GetTrucks(string sort);
		Truck? GetTruck(int id);
		Truck? GetTruck(string name);
		string? GetDescription(int id);
		bool TruckExists(int id);
		bool CreateTruck(Truck truck);
		bool UpdateTruck(int id, TruckDTO truck);
		bool DeleteTruck(Truck truck);
		bool IsStatusUpdateValid(int id, int truckStatus);
		bool Save();
	}
}
