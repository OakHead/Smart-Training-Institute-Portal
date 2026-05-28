namespace Smart_Training_Institute_Portal.Models
{
	public class Department : BaseEntity
	{
		public string Name { get; set; }
		public ICollection<Course> Courses { get; set; } = new List<Course>();
	}
}
