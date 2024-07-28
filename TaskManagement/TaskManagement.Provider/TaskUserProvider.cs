using Microsoft.AspNetCore.Http;
using TaskManagement.DTO.Responses;
using TaskManagement.Interface.Provider;
using TaskManagement.Interface.Repository;

namespace TaskManagement.Provider;

public class TaskUserProvider(ITaskUserRepository taskUserRepository) : ITaskUserProvider
{
    private readonly ITaskUserRepository _taskUserRepository = taskUserRepository;
    public async Task<GetTaskUserDetailsResponse> GetTaskUserDetailsResponse(int userId)
    {
		try
		{
            var taskUser = await _taskUserRepository.GetById(userId);
            if (taskUser is null)
            {
                return new()
                {
                    Error = new()
                    {
                        Title = "User not found",
                        Description = $"No user could be found with the user Id {userId}",
                        StatusCode = StatusCodes.Status404NotFound
                    }
                };
            }

            return new(taskUser);
		}
		catch (Exception ex)
		{
            return new()
            {
                Error = new()
                {
                    Title = "An unknown error occurred",
                    Description = "An unknown error occurred while trying to retrieve the current user's details.",
                    StatusCode = 500,
                    AdditionalDetails = ex.Message
                }
            };
        }
    }
}
