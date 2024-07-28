using System.ComponentModel.DataAnnotations;

namespace TaskManagement.DTO.Requests;

public class AdminCreateAdminAccountRequest
{
    [Required]
    public int RequestingAdminId { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public bool IsActive { get; set; }
}
