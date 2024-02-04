using CompanyApp.Models;

namespace CompanyApp.Models {
	public class Client {
		public int Id { get; set; }
		public Company? Company { get; set; }
		public ICollection<EmployeeClient>? EmployeeClients { get; set; }
	}
}
