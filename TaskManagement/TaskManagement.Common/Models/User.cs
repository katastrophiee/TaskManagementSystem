using Microsoft.AspNet.Identity.EntityFramework;

namespace TaskManagement.Common.Models;

public class User
{
    public string Username { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string PasswordSalt { get; set; }

    public bool IsActive { get; set; }

    public DateTime? LastLoggedIn { get; set; }

    public List<Roles> UserRoles { get; set; }
}
