using TrainingCenter.Application.DTOs.Auth.Requests;
using TrainingCenter.Application.DTOs.Auth.Response;
using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Domain.Results;

namespace TrainingCenter.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
        Task<Result<CurrentUserResponse>> RegisterAsync(RegisterRequest request , Role role = Role.Student);
        Task<Result<CurrentUserResponse>> GetCurrentUserAsync(int userId);
        Task<Result> ChangePasswordAsync(int id, ChangePasswordRequest request);
        Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task<Result> LogoutAsync(int userId, string refreshToken);
    }
}
