using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;
using Smart_Training_Institute_Portal.Models;
using Smart_Training_Institute_Portal.ViewModels;
using System.Data;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
	private readonly UserManager<User> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly ApplicationDbContext _context;

	public UsersController(
		UserManager<User> userManager,
		RoleManager<IdentityRole> roleManager,
		ApplicationDbContext context)
	{
		_userManager = userManager;
		_roleManager = roleManager;
		_context = context;
	}

	public async Task<IActionResult> Index()
	{
		var students = await _context.StudentProfiles
			.Include(s => s.User)
			.Include(s => s.Enrollments)
				.ThenInclude(e => e.Course)
			.ToListAsync();

		var instructors = await _context.InstructorProfiles
			.Include(i => i.User)
			.Include(i => i.Courses)
				.ThenInclude(ci => ci.Course)
			.ToListAsync();

		ViewBag.Students = students;
		ViewBag.Instructors = instructors;

		return View();
	}

	public async Task<IActionResult> StudentDetails(int id)
	{
		var student = await _context.StudentProfiles
			.Include(s => s.User)
			.Include(s => s.Enrollments)
				.ThenInclude(e => e.Course)
			.FirstOrDefaultAsync(s => s.Id == id);

		if (student == null)
		{
			return NotFound();
		}

		return View(student);
	}

	public async Task<IActionResult> InstructorDetails(int id)
	{
		var instructor = await _context.InstructorProfiles
			.Include(i => i.User)
			.Include(i => i.Courses)
				.ThenInclude(ci => ci.Course)
			.FirstOrDefaultAsync(i => i.Id == id);

		if (instructor == null)
		{
			return NotFound();
		}

		return View(instructor);
	}


	public IActionResult Create()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(CreateUserViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var user = new User
		{
			UserName = model.Email,
			Email = model.Email,
			FullName = model.FullName
		};

		var result = await _userManager.CreateAsync(user, model.Password);

		if (!result.Succeeded)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}

			return View(model);
		}

		await _userManager.AddToRoleAsync(user, model.Role);

		if (model.Role == "Student")
		{
			var studentProfile = new StudentProfile
			{
				UserId = user.Id,
				StudentId = user.Id
			};

			_context.StudentProfiles.Add(studentProfile);
		}
		else if (model.Role == "Instructor")
		{
			var instructorProfile = new InstructorProfile
			{
				UserId = user.Id,
				InstructorId = user.Id
			};

			_context.InstructorProfiles.Add(instructorProfile);
		}

		await _context.SaveChangesAsync();

		return RedirectToAction(nameof(Index));
	}
	public async Task<IActionResult> EditStudent(int id)
	{
		var student = await _context.StudentProfiles
			.Include(s => s.User)
			.FirstOrDefaultAsync(s => s.Id == id);

		if (student == null)
		{
			return NotFound();
		}
		var roles = await _userManager.GetRolesAsync(student.User);
		var model = new EditStudentViewModel
		{
			Id = student.Id,
			UserId = student.UserId,
			FullName = student.User.FullName,
			Email = student.User.Email,
			Role = roles.FirstOrDefault(),
			StudentId = student.StudentId,
			ImageUrl = student.ImageUrl,
			DateOfBirth = student.DateOfBirth,
			GPA = student.GPA
		};

		return View(model);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> EditStudent(EditStudentViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var student = await _context.StudentProfiles
			.Include(s => s.User)
			.FirstOrDefaultAsync(s => s.Id == model.Id);

		if (student == null)
		{
			return NotFound();
		}

		student.StudentId = model.StudentId;
		student.ImageUrl = model.ImageUrl;
		student.DateOfBirth = model.DateOfBirth;
		student.GPA = model.GPA;

		student.User.FullName = model.FullName;
		student.User.Email = model.Email;
		student.User.UserName = model.Email;

		if (!string.IsNullOrWhiteSpace(model.NewPassword))
		{
			var token = await _userManager.GeneratePasswordResetTokenAsync(student.User);
			var result = await _userManager.ResetPasswordAsync(student.User, token, model.NewPassword);
			var currentRoles = await _userManager.GetRolesAsync(student.User);

			await _userManager.RemoveFromRolesAsync(student.User, currentRoles);
			await _userManager.AddToRoleAsync(student.User, model.Role);
			if (model.Role == "Instructor")
			{
				var instructorExists = await _context.InstructorProfiles
					.AnyAsync(i => i.UserId == student.UserId);

				if (!instructorExists)
				{
					var instructorProfile = new InstructorProfile
					{
						UserId = student.UserId,
						InstructorId = student.StudentId,
						Specialization = "Not Set"
					};

					_context.InstructorProfiles.Add(instructorProfile);
				}

				_context.StudentProfiles.Remove(student);
			}
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}

				return View(model);
			}
		}

		await _context.SaveChangesAsync();

		return RedirectToAction(nameof(Index));
	}
	public async Task<IActionResult> EditInstructor(int id)
	{
		var instructor = await _context.InstructorProfiles
			.Include(i => i.User)
			.FirstOrDefaultAsync(i => i.Id == id);

		if (instructor == null)
		{
			return NotFound();
		}
		var roles = await _userManager.GetRolesAsync(instructor.User);
		var model = new EditInstructorViewModel
		{
			Id = instructor.Id,
			UserId = instructor.UserId,
			FullName = instructor.User.FullName,
			Email = instructor.User.Email,
			Role = roles.FirstOrDefault(),
			InstructorId = instructor.InstructorId,
			ImageUrl = instructor.ImageUrl,
			Qualifications = instructor.Qualifications,
			Experience = instructor.Experience,
			Specialization = instructor.Specialization,
			OfficeLocation = instructor.OfficeLocation,
			OfficeHours = instructor.OfficeHours
		};

		return View(model);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> EditInstructor(EditInstructorViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var instructor = await _context.InstructorProfiles
			.Include(i => i.User)
			.FirstOrDefaultAsync(i => i.Id == model.Id);

		if (instructor == null)
		{
			return NotFound();
		}

		instructor.InstructorId = model.InstructorId;
		instructor.ImageUrl = model.ImageUrl;
		instructor.Qualifications = model.Qualifications;
		instructor.Experience = model.Experience;
		instructor.Specialization = model.Specialization;
		instructor.OfficeLocation = model.OfficeLocation;
		instructor.OfficeHours = model.OfficeHours;

		instructor.User.FullName = model.FullName;
		instructor.User.Email = model.Email;
		instructor.User.UserName = model.Email;

		if (!string.IsNullOrWhiteSpace(model.NewPassword))
		{
			var token = await _userManager.GeneratePasswordResetTokenAsync(instructor.User);
			var result = await _userManager.ResetPasswordAsync(instructor.User, token, model.NewPassword);
			var currentRoles = await _userManager.GetRolesAsync(instructor.User);

			await _userManager.RemoveFromRolesAsync(instructor.User, currentRoles);
			await _userManager.AddToRoleAsync(instructor.User, model.Role);
			if (model.Role == "Student")
			{
				var studentExists = await _context.StudentProfiles
					.AnyAsync(s => s.UserId == instructor.UserId);

				if (!studentExists)
				{
					var studentProfile = new StudentProfile
					{
						UserId = instructor.UserId,
						StudentId = instructor.InstructorId
					};

					_context.StudentProfiles.Add(studentProfile);
				}

				_context.InstructorProfiles.Remove(instructor);
			}
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}

				return View(model);
			}
		}

		await _context.SaveChangesAsync();

		return RedirectToAction(nameof(Index));
	}
}