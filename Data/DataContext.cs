using CompanyApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyApp.Data {
	public class DataContext(DbContextOptions<DataContext> options) : DbContext(options) {
		public DbSet<Client> Clients { get; set; }
		public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeClient> EmployeeClients { get; set; }
		public DbSet<Truck> Trucks { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<Truck>()
				.HasIndex(t => t.Code).IsUnique();
			modelBuilder.Entity<EmployeeClient>()
				.HasKey(ec => new { ec.EmployeeId, ec.ClientId });
			modelBuilder.Entity<EmployeeClient>()
				.HasOne(e => e.Employee)
				.WithMany(ec => ec.EmployeeClients)
				.HasForeignKey(e => e.EmployeeId);
			modelBuilder.Entity<EmployeeClient>()
				.HasOne(c => c.Client)
				.WithMany(ec => ec.EmployeeClients)
				.HasForeignKey(c => c.ClientId);
		}
	}
}
