using Microsoft.AspNetCore.Identity;

namespace UserManagement.Models;

public class UserDetails : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public bool IsBlocked { get; set; }
    public DateTime? LastLoginDate { get; set; } 
}