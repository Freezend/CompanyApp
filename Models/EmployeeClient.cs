using CompanyApp.Models;

namespace CompanyApp.Models {
	public class EmployeeClient {
		public int EmployeeId { get; set; }
		public int ClientId { get; set; }
		public required Employee Employee { get; set; }
        public required Client Client { get; set; }
    }
}
