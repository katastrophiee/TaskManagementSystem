namespace TaskManagement.Common.Models;

public sealed class TaskUser : User
{
    public int TaskUserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? ContactNumber { get; set; }
}
