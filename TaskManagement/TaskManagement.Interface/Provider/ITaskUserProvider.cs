using TaskManagement.DTO.Responses;

namespace TaskManagement.Interface.Provider;

public interface ITaskUserProvider
{
    Task<GetTaskUserDetailsResponse> GetTaskUserDetailsResponse(int userId);
}
