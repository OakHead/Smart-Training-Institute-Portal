using Microsoft.AspNetCore.Identity;

namespace Smart_Training_Institute_Portal.Models
{
	public class User : IdentityUser
	{
		public string FullName { get; set; }
		public StudentProfile? StudentProfile { get; set; }
		public InstructorProfile? InstructorProfile { get; set; }
		public ICollection<GradeLog> GradeLogs { get; set; } = new List<GradeLog>();
	}
}
