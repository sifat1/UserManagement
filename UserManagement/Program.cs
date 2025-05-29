using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Parse DATABASE_URL into Npgsql format with fallback to appsettings
var rawUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
var connectionString = string.IsNullOrEmpty(rawUrl) 
    ? builder.Configuration.GetConnectionString("DefaultConnection")
    : ConvertDatabaseUrlToNpgsql(rawUrl);

if (string.IsNullOrEmpty(connectionString))
    throw new Exception("No database connection configuration found");

// Register your DB context
builder.Services.AddDbContext<DBContext>(options =>
    options.UseNpgsql(connectionString));

// Configure Identity with relaxed password rules (for development)
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

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; 
});

// Register custom services
builder.Services.AddScoped<UserService>();

var app = builder.Build();

// Middleware pipeline
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ManageUser}/{action=ShowUsers}/{id?}");

app.Run();

// Convert Heroku/Render-style DATABASE_URL to Npgsql format
string ConvertDatabaseUrlToNpgsql(string databaseUrl)
{
    try
    {
        // Allow both postgresql:// and postgres://
        databaseUrl = databaseUrl.Replace("postgresql://", "postgres://");
        
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');

        if (userInfo.Length != 2)
            throw new Exception("DATABASE_URL must contain username and password");

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = uri.AbsolutePath.TrimStart('/'),
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        };
        if (uri.Port != -1)
        {
            builder.Port = uri.Port;
        }

        return builder.ToString();
    }
    catch (Exception ex)
    {
        throw new Exception("Failed to parse DATABASE_URL", ex);
    }
}