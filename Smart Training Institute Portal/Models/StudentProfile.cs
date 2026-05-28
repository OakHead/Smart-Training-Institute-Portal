using System.ComponentModel.DataAnnotations;

namespace Smart_Training_Institute_Portal.Models
{
	public class StudentProfile : BaseEntity
	{
		public string StudentId { get; set; }
		public string? ImageUrl { get; set; }
		public DateOnly? DateOfBirth { get; set; }
		public decimal? GPA { get; set; }

		[Required]
		public string UserId { get; set; }
		public User User { get; set; }
		public ICollection<StudentEnrollment> Enrollments { get; set; } =new List<StudentEnrollment>();
	}
}
