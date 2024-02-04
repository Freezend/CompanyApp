using CompanyApp.Models;

namespace CompanyApp.Models {
	public class Company {
		public int Id { get; set; }
		public ICollection<Truck>? Trucks { get; set; }
		public ICollection<Employee>? Employees { get; set; }
		public ICollection<Client>? Clients { get; set; }
	}
}
