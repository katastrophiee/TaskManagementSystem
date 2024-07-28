using TaskManagement.Common.Models;

namespace TaskManagement.DTO.Responses;

public class GetTaskUserDetailsResponse
{
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ErrorResponse? Error { get; set; }

    public GetTaskUserDetailsResponse()
    {
    }

    public GetTaskUserDetailsResponse(TaskUser user)
    {
        UserId = user.TaskUserId;
        FirstName = user.FirstName;
        LastName = user.LastName;
    }
}
