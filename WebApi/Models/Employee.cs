using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApi.Models;

public class Employee
{
	[JsonPropertyName("guid")] 
	public Guid? Guid { get; set; }

	[JsonPropertyName("fullName")]
	[Required]
	[StringLength(maximumLength: 100, MinimumLength = 3)]
	public string? FullName { get; set; }

	[JsonPropertyName("position")]
	[Required]
	[StringLength(maximumLength: 100, MinimumLength = 3)]
	public string? Position { get; set; }
}