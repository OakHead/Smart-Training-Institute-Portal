using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Models;

namespace Smart_Training_Institute_Portal.Data
{
	public class ApplicationDbContext : IdentityDbContext<User>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}
		public DbSet<StudentProfile> StudentProfiles { get; set; }
		public DbSet<InstructorProfile> InstructorProfiles { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<Prerequisite> Prerequisites { get; set; }
		public DbSet<CourseInstructor> CourseInstructors { get; set; }
		public DbSet<StudentEnrollment> StudentEnrollments { get; set; }
		public DbSet<GradeLog> GradeLogs { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<CourseInstructor>()
				.HasKey(ci => new { ci.CourseId, ci.InstructorProfileId });

			modelBuilder.Entity<CourseInstructor>()
				.HasOne(ci => ci.Course)
				.WithMany(c => c.Instructors)
				.HasForeignKey(ci => ci.CourseId);

			modelBuilder.Entity<CourseInstructor>()
				.HasOne(ci => ci.InstructorProfile)
				.WithMany(i => i.Courses)
				.HasForeignKey(ci => ci.InstructorProfileId);



			modelBuilder.Entity<StudentEnrollment>()
				.HasIndex(se => new { se.StudentProfileId, se.CourseId })
				.IsUnique();

			modelBuilder.Entity<StudentEnrollment>()
				.HasOne(se => se.StudentProfile)
				.WithMany(sp => sp.Enrollments)
				.HasForeignKey(se => se.StudentProfileId);

			modelBuilder.Entity<StudentEnrollment>()
				.HasOne(se => se.Course)
				.WithMany(c => c.Enrollments)
				.HasForeignKey(se => se.CourseId);

			modelBuilder.Entity<GradeLog>()
				.HasOne(g => g.UpdatedBy)
				.WithMany(u => u.GradeLogs)
				.HasForeignKey(g => g.UpdatedById)
				.OnDelete(DeleteBehavior.Restrict);
			modelBuilder.Entity<Course>()
				.HasOne(c => c.Department)
				.WithMany(d => d.Courses)
				.HasForeignKey(c => c.DepartmentId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
