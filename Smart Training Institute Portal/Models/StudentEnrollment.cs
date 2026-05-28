namespace Smart_Training_Institute_Portal.Models
{
	public class StudentEnrollment : BaseEntity
	{
		public DateTime EnrollmentDate { get; set; }
		public string? Grade { get; set; }
		public decimal? Mark { get; set; }
		public string Status { get; set; } = "Enrolled";

		public int StudentProfileId { get; set; }
		public StudentProfile StudentProfile { get; set; }

		public int CourseId { get; set; }
		public Course Course { get; set; }

		public ICollection<GradeLog> GradeLogs { get; set; } = new List<GradeLog>();
	}
}
