namespace Smart_Training_Institute_Portal.Models
{
	public class BaseEntity
	{
		public int Id { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public DateTime? UpdatedDate { get; set; }
		public DateTime? DeleteDate { get; set; }
		public bool? IsDeleted { get; set; }
	}
}
