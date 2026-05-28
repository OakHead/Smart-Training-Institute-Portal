using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;
using Smart_Training_Institute_Portal.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));

// Add Identity services
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
	.AddRoles<IdentityRole>() // Add role support
	.AddEntityFrameworkStores<ApplicationDbContext>(); // Use the same ApplicationDbContext for Identity

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	CreateRoles(services).Wait(); // Create roles and admin user
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();

async Task CreateRoles(IServiceProvider serviceProvider) //Task means that this method wont return anything but it allows us to use async/await inside it
{
	var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
	var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
	// Define roles
	string[] roleNames = { "Admin", "Student", "Instructor" };
	IdentityResult roleResult;
	foreach (var roleName in roleNames)
	{
		// Check if the role exists, if not create it
		var roleExist = await roleManager.RoleExistsAsync(roleName); // Check if the role already exists from the database named AspNetRoles
		if (!roleExist)
		{
			roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
		}
	}
	// Create an admin user

	string adminEmail = "admin@htu.edu.jo";
	string adminPassword = "Admin.336"; // Ensure this meets your password policy
	var adminUser = await userManager.FindByEmailAsync(adminEmail);
	if (adminUser == null)
	{
		var newAdminUser = new User
		{
			UserName = adminEmail,
			Email = adminEmail,
			EmailConfirmed = true // Set to true if you don't want email confirmation for the admin user
		};
		var createAdminResult = await userManager.CreateAsync(newAdminUser, adminPassword);
		if (createAdminResult.Succeeded)
		{
			await userManager.AddToRoleAsync(newAdminUser, "Admin");
		}
	}
}
