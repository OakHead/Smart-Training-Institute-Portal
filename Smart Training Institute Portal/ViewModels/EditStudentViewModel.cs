using System.ComponentModel.DataAnnotations;

namespace Smart_Training_Institute_Portal.ViewModels
{
	public class EditStudentViewModel
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

		public string StudentId { get; set; }
		public string? ImageUrl { get; set; }
		public DateOnly? DateOfBirth { get; set; }
		public decimal? GPA { get; set; }
	}
}