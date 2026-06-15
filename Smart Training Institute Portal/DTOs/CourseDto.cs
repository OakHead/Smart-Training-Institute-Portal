namespace Smart_Training_Institute_Portal.DTOs
{
	public class CourseDto
	{
		public int Id { get; set; }
		public string CourseCode { get; set; }
		public string Title { get; set; }
		public string? Description { get; set; }
		public int CreditHours { get; set; }
		public int HoursPerWeek { get; set; }
		public int Level { get; set; }
		public bool IsPublished { get; set; }
		public int DepartmentId { get; set; }

		public int[] SelectedPrerequisiteIds { get; set; } = Array.Empty<int>();
		public int[] SelectedInstructorIds { get; set; } = Array.Empty<int>();
	}
}
