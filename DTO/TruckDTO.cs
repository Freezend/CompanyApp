using CompanyApp.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CompanyApp.DTO {
	public class TruckDTO {
		[JsonIgnore]
		public int Id { get; set; }
		[RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Code must be alphanumeric.")]
		[StringLength(20, MinimumLength = 6, ErrorMessage = "Code length must be between 6 and 20 characters.")]
		public string? Code { get; set; }
		[StringLength(50, MinimumLength = 3, ErrorMessage = "Name length must be between 3 and 50 characters.")]
		public string? Name { get; set; }
		[EnumDataType(typeof(TruckStatus), ErrorMessage = "Invalid truck status.")]
		public TruckStatus? Status { get; set; }
		[StringLength(100, ErrorMessage = "Description cannot be longer than 100 characters.")]
		public string? Description { get; set; }
	}
}
