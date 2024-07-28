using System.ComponentModel.DataAnnotations;

namespace TaskManagement.DTO.Requests;

public class CreateTaskUserAccountRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [PasswordRules]
    public string Password { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }
}
