using Microsoft.OpenApi.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CompanyApp.Models {
	public enum TruckStatus {
		[Display(Name = "Out of Service")]
		OutOfService,
		[Display(Name = "Loading")]
		Loading,
		[Display(Name = "To Job")]
		ToJob,
		[Display(Name = "At Job")]
		AtJob,
		[Display(Name = "Returning")]
		Returning
	}
	public class Truck {
		public int Id { get; set; }
		[Required]
		[RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Code must be alphanumeric.")]
		[StringLength(20, MinimumLength = 6, ErrorMessage = "Code length must be between 6 and 20 characters.")]
		public required string Code { get; set; }
		[Required]
		[StringLength(50, MinimumLength = 3, ErrorMessage = "Name length must be between 3 and 50 characters.")]
		public required string Name { get; set; }
		[Required]
		[EnumDataType(typeof(TruckStatus), ErrorMessage = "Invalid truck status.")]
		public required TruckStatus Status { get; set; }
		[StringLength(100, ErrorMessage = "Description cannot be longer than 100 characters.")]
		public string? Description { get; set; }
		[JsonIgnore]
		public Company? Company { get; set; }
		public string GetStatusAsString() {
			return Status.GetAttributeOfType<DisplayAttribute>().Name ?? "";
		}
	}
}
