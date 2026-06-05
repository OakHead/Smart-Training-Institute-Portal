namespace Smart_Training_Institute_Portal.Models
{
	public class Course : BaseEntity
	{
		public string CourseCode { get; set; }
		public string Title { get; set; }
		public string? Description { get; set; }
		public int CreditHours { get; set; }
		public int HoursPerWeek { get; set; }
		public int Level { get; set; }
		public bool IsPublished { get; set; } = false;

		public int DepartmentId { get; set; }
		public Department? Department { get; set; }

		public ICollection<CourseInstructor> Instructors { get; set; } = new List<CourseInstructor>();
		public ICollection<StudentEnrollment> Enrollments { get; set; } = new List<StudentEnrollment>();
		public ICollection<Prerequisite> Prerequisites { get; set; } = new List<Prerequisite>();
	}
}
