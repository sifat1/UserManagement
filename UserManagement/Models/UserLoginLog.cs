namespace UserManagement.Models;

public class UserLoginLog
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; } = DateTime.UtcNow;
    public string IpAddress { get; set; } = string.Empty;
    public UserDetails User { get; set; } = null!; // null! tells compiler we'll initialize it
}