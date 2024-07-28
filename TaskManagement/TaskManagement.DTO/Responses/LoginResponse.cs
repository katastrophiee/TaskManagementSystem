using TaskManagement.Common.Models;

namespace TaskManagement.DTO.Responses;

public sealed class LoginResponse
{
    public int UserId { get; set; }

    public string AccessToken { get; set; }

    public int ExpiresIn { get; set; }

    public ErrorResponse Error { get; set; }
}
