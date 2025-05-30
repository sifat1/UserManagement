using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();


var rawUrl = Environment.GetEnvironmentVariable("DATABASE_URL");


builder.Services.AddDbContext<DBContext>(options =>
    options.UseNpgsql(rawUrl, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
    }));

builder.Services.AddDefaultIdentity<UserDetails>(options =>
{
    options.Password.RequireDigit = false; 
    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<DBContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; 
});

builder.Services.AddScoped<UserService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DBContext>();
    db.Database.Migrate();
}

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ManageUser}/{action=ShowUsers}/{id?}");

app.Run();
