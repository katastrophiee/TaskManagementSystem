using System.ComponentModel.DataAnnotations;

namespace TaskManagement.DTO.Requests;

public sealed class LoginRequest
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a username")]
    public string Username { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a password")]
    public string Password { get; set; }
}
