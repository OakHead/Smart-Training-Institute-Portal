using System.Globalization;

namespace Smart_Training_Institute_Portal.Models
{
	public class InstructorProfile : BaseEntity
	{
		public string InstructorId { get; set; }
		public string? ImageUrl { get; set; }
		public string? Qualifications { get; set; }
		public DateOnly? Experience { get; set; }
		public string Specialization { get; set; }
		public string? OfficeLocation { get; set; }
		public string? OfficeHours { get; set; }

		public string UserId { get; set; }
		public User User { get; set; }
		public ICollection<CourseInstructor> Courses { get; set; } = new List<CourseInstructor>();


	}
}
