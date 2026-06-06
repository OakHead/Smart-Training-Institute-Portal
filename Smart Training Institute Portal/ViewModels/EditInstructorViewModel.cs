using System.ComponentModel.DataAnnotations;

namespace Smart_Training_Institute_Portal.ViewModels
{
	public class EditInstructorViewModel
	{
		public int Id { get; set; }

		public string UserId { get; set; }

		[Required]
		public string FullName { get; set; }

		[Required]
		public string Email { get; set; }

		public string? NewPassword { get; set; }
		[Required]
		public string Role { get; set; }

		public string InstructorId { get; set; }
		public string? ImageUrl { get; set; }
		public string? Qualifications { get; set; }
		public DateOnly? Experience { get; set; }
		public string Specialization { get; set; }
		public string? OfficeLocation { get; set; }
		public string? OfficeHours { get; set; }
	}
}