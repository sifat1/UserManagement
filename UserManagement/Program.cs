using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();


var rawUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(rawUrl))
    throw new Exception("DATABASE_URL is not set");


rawUrl = rawUrl.Replace("postgresql://", "postgres://");

var connectionString = ConvertDatabaseUrlToNpgsql(rawUrl);

builder.Services.AddDbContext<DBContext>(options =>
    options.UseNpgsql(connectionString));

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

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ManageUser}/{action=ShowUsers}/{id?}");

app.Run();

string ConvertDatabaseUrlToNpgsql(string databaseUrl)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');

    return new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port,
        Username = userInfo[0],
        Password = userInfo[1],
        Database = uri.AbsolutePath.TrimStart('/'),
        SslMode = SslMode.Require,
        TrustServerCertificate = true
    }.ToString();
}
