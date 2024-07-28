using System.Diagnostics.Metrics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System;
using TaskManagement.Common.Models;
using TaskManagement.DTO.Requests;
using TaskManagement.DTO.Responses;
using TaskManagement.Interface.Provider;
using TaskManagement.Interface.Repository;
using Microsoft.Extensions.Configuration;
using TaskManagement.Common.Utils;
using Microsoft.AspNetCore.Http;
using TaskManagement.Common.Enums;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace TaskManagement.Provider;

public class AuthProvider(
    ITaskUserRepository taskUserRepository,
    IConfigurationSection jwtValues,
    IAdminUserRepository adminUserRepository) : IAuthProvider
{
    private readonly ITaskUserRepository _taskUserRepository = taskUserRepository;
    private readonly IConfigurationSection _jwtValues = jwtValues;
    private readonly IAdminUserRepository _adminUserRepository = adminUserRepository;

    public async Task<LoginResponse> TaskUserLogin(LoginRequest request)
    {
        try
        {
            var user = await _taskUserRepository.GetByUsername(request.Username);
            if (user is null)
            {
                return new()
                {
                    Error = new()
                    {
                        Title = "Invalid Credentials",
                        Description = "Incorrect username or password",
                        StatusCode = StatusCodes.Status400BadRequest,
                    }
                };
            }

            return await LoginAsTaskUser(user, request.Password);
        }
        catch (Exception ex)
        {
            return new()
            {
                Error = new ErrorResponse()
                {
                    Title = "Internal Server Error",
                    Description = $"An unknown error occured",
                    StatusCode = StatusCodes.Status500InternalServerError,
                    AdditionalDetails = ex.Message

                }
            };
        }
    }

    private async Task<LoginResponse> LoginAsTaskUser(TaskUser user, string password)
    {
        var passwordSalt = user.PasswordSalt;
        var pbkdf2HashedPassword = password.Pbkdf2HashString(ref passwordSalt);

        if (!string.Equals(pbkdf2HashedPassword, user.Password))
        {
            return new()
            {
                Error = new()
                {
                    Title = "Invalid Credentials",
                    Description = "Incorrect username or password",
                    StatusCode = StatusCodes.Status400BadRequest,
                }
            };
        }

        if (!user.IsActive)
            return new()
            {
                Error = new()
                {
                    Title = "Inactive Account",
                    Description = "Cannot login as your account is not active. Please contact admins for additional information.",
                    StatusCode = StatusCodes.Status403Forbidden,
                }
            };

        user.LastLoggedIn = DateTime.UtcNow;

        await _taskUserRepository.Update(user);

        var accessToken = await GenerateAccessToken(user.TaskUserId, false);

        return new()
        {
            UserId = user.TaskUserId,
            AccessToken = accessToken,
            ExpiresIn = 30,
        };
    }

    public async Task<LoginResponse> CreateTaskUserAccount(CreateTaskUserAccountRequest request)
    {
        try
        {
            var existingTaskUsers = await _taskUserRepository.GetListByUsernameOrEmail(request.Username, request.Email);
            if (existingTaskUsers.Count != 0)
            {
                return new()
                {
                    Error = new()
                    {
                        Title = "Credentials Already Used",
                        Description = "Cannot create account as the username or email is already in use",
                        StatusCode = StatusCodes.Status400BadRequest,
                    }
                };
            }

            var passwordSalt = GenerateSalt();
            var pbkdf2HashedPassword = request.Password.Pbkdf2HashString(ref passwordSalt);

            var newTaskUser = new TaskUser()
            {
                Username = request.Username,
                Email = request.Email,
                Password = pbkdf2HashedPassword,
                PasswordSalt = passwordSalt,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
                LastLoggedIn = DateTime.UtcNow,
                UserRoles = [
                    new()
                    {
                        RoleId = (int)Role.TaskUser,
                        RoleName = Role.TaskUser.EnumDisplayName()
                    }
                ]
            };

            await _taskUserRepository.Add(newTaskUser);

            var taskUser = await _taskUserRepository.GetByUsername(request.Username);

            var accessToken = await GenerateAccessToken(taskUser!.TaskUserId, false);

            return new()
            {
                UserId = taskUser.TaskUserId,
                AccessToken = accessToken,
                ExpiresIn = 30   
            };
        }
        catch (Exception ex)
        {
            return new()
            {
                Error = new ErrorResponse()
                {
                    Title = "Internal Server Error",
                    Description = $"An unknown error occured when creating the task user's account",
                    StatusCode = StatusCodes.Status500InternalServerError,
                    AdditionalDetails = ex.Message

                }
            };
        }
    }

    // I used this video to help me add my tokens and use them
    // https://www.youtube.com/watch?v=7kyIEnQ8kF4&t=240s

    private async Task<string> GenerateAccessToken(int userId, bool isAdmin)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var user = isAdmin
            ? null // TO DO - Change to get admin account to add the roles to the token
            : await _taskUserRepository.GetById(userId);

        // I used this page when trying to add roles to my token
        // https://stackoverflow.com/questions/943398/get-int-value-from-enum-in-c-sharp

        foreach (var role in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, ((Role)role.RoleId).EnumDisplayName()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtValues["Key"] ?? ""));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtValues["Issuer"],
            audience: _jwtValues["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public static string GenerateSalt(int size = 32)
    {
        var buff = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buff);

        return Convert.ToBase64String(buff);
    }

    public async Task<LoginResponse> AdminLogin(LoginRequest request)
    {
        try
        {
            var admin = await _adminUserRepository.GetByUsername(request.Username);
            if (admin is null)
                return new()
                {
                    Error = new()
                    {
                        Title = "Invalid Credentials",
                        Description = "Incorrect username or password",
                        StatusCode = StatusCodes.Status400BadRequest,
                    }
                };

            return await LoginAsAdmin(admin, request.Password);
        }
        catch (Exception ex)
        {
            return new()
            {
                Error = new ErrorResponse()
                {
                    Title = "Internal Server Error",
                    Description = $"An unknown error occured",
                    StatusCode = StatusCodes.Status500InternalServerError,
                    AdditionalDetails = ex.Message

                }
            };
        }
    }

    private async Task<LoginResponse> LoginAsAdmin(AdminUser admin, string password)
    {
        var passwordSalt = admin.PasswordSalt;
        var pbkdf2HashedPassword = password.Pbkdf2HashString(ref passwordSalt);

        if (!string.Equals(pbkdf2HashedPassword, admin.Password))
        {
            return new()
            {
                Error = new()
                {
                    Title = "Invalid Credentials",
                    Description = "Incorrect username or password",
                    StatusCode = StatusCodes.Status400BadRequest,
                }
            };
        }

        if (!admin.IsActive)
            return new()
            {
                Error = new()
                {
                    Title = "Inactive Account",
                    Description = "Cannot login as your account is not active. Please contact admins for additional information.",
                    StatusCode = StatusCodes.Status403Forbidden,
                }
            };

        admin.LastLoggedIn = DateTime.UtcNow;
        await _adminUserRepository.Update(admin);

        var accessToken = await GenerateAccessToken(admin.AdminUserId, true);

        return new()
        {
            UserId = admin.AdminUserId,
            AccessToken = accessToken,
            ExpiresIn = 30,
        };
    }

    public async Task<LoginResponse> CreateAdminAccount(AdminCreateAdminAccountRequest request)
    {
        try
        {
            var requestingAdmin = await _adminUserRepository.GetById(request.RequestingAdminId);
            if (requestingAdmin is null || requestingAdmin.AdminUserId == 0)
                return new()
                {
                    Error = new ErrorResponse()
                    {
                        Title = "Admin Not Found",
                        Description = $"No admin found with the ID {request.RequestingAdminId}",
                        StatusCode = StatusCodes.Status404NotFound
                    }
                };

            var existingAdmins = await _adminUserRepository.GetListByUsernameOrEmail(request.Username, request.Email);
            if (existingAdmins.Count != 0)
            {
                return new()
                {
                    Error = new()
                    {
                        Title = "Credentials Already Used",
                        Description = "Cannot create account as the username or email is already in use",
                        StatusCode = StatusCodes.Status400BadRequest,
                    }
                };
            }

            var passwordSalt = GenerateSalt();
            var pbkdf2HashedPassword = request.Password.Pbkdf2HashString(ref passwordSalt);

            var newAdmin = new AdminUser()
            {
                Username = request.Username,
                Email = request.Email,
                Password = pbkdf2HashedPassword,
                PasswordSalt = passwordSalt,
                IsActive = request.IsActive,
                LastLoggedIn = null,
                UserRoles = [
                    new()
                    {
                        RoleId = (int)Role.Admin,
                        RoleName = Role.Admin.EnumDisplayName()
                    }
                ]
            };

            await _adminUserRepository.Add(newAdmin);

            var admin = await _adminUserRepository.GetByUsername(request.Username);

            var accessToken = await GenerateAccessToken(admin!.AdminUserId, true);

            return new()
            {
                UserId = admin.AdminUserId,
                AccessToken = accessToken,
                ExpiresIn = 30
            };
        }
        catch (Exception ex)
        {
            return new()
            {
                Error = new ErrorResponse()
                {
                    Title = "Internal Server Error",
                    Description = $"An unknown error occured when creating the admin user's account",
                    StatusCode = StatusCodes.Status500InternalServerError,
                    AdditionalDetails = ex.Message

                }
            };
        }
    }

    public async Task<LoginResponse> UpdatePassword(UpdatePasswordRequest request)
    {
        try
        {
            var user = await _taskUserRepository.GetByUsername(request.Username);
            if (user is null)
            {
                return new()
                {
                    Error = new()
                    {
                        Title = "No Account Found",
                        Description = $"No account was found with the username {request.Username}",
                        StatusCode = StatusCodes.Status400BadRequest,
                    }
                };
            }

            if (request.Password != request.ConfirmPassword)
                return new()
                {
                    Error = new()
                    {
                        Title = "Confirm Password MisMatch",
                        Description = "The password and confirmed password do not match",
                        StatusCode = StatusCodes.Status400BadRequest
                    }
                };

            var passwordSalt = GenerateSalt();
            var pbkdf2HashedPassword = request.Password.Pbkdf2HashString(ref passwordSalt);

            user.PasswordSalt = passwordSalt;
            user.Password = pbkdf2HashedPassword;

            await _taskUserRepository.Update(user);

            return new()
            {
                UserId = user.TaskUserId,
                AccessToken = await GenerateAccessToken(user.TaskUserId, false),
                ExpiresIn = 30
            };
        }
        catch (Exception ex)
        {
            return new()
            {
                Error = new ErrorResponse()
                {
                    Title = "Internal Server Error",
                    Description = $"An unknown error occured when updating password for task user",
                    StatusCode = StatusCodes.Status500InternalServerError,
                    AdditionalDetails = ex.Message

                }
            };
        }
    }
}

