using TaskManagement.DTO.Requests;
using TaskManagement.DTO.Responses;

namespace TaskManagement.Interface.Provider;

public interface IAuthProvider
{
    Task<LoginResponse> TaskUserLogin(LoginRequest request);

    Task<LoginResponse> CreateTaskUserAccount(CreateTaskUserAccountRequest request);

    Task<LoginResponse> AdminLogin(LoginRequest request);

    Task<LoginResponse> CreateAdminAccount(AdminCreateAdminAccountRequest request);

    Task<LoginResponse> UpdatePassword(UpdatePasswordRequest request);
}
