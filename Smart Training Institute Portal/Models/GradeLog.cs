namespace Smart_Training_Institute_Portal.Models
{
	public class GradeLog : BaseEntity
	{
		public decimal? PreviousMark { get; set; }
		public decimal? NewMark { get; set; }
		public string? PreviousGrade { get; set; }
		public string? NewGrade { get; set; }
		public string? Notes { get; set; }

		public int StudentEnrollmentId { get; set; }
		public StudentEnrollment StudentEnrollment { get; set; } = null!;

		public string UpdatedById { get; set; } = string.Empty;
		public User UpdatedBy { get; set; } = null!;
	}
}
