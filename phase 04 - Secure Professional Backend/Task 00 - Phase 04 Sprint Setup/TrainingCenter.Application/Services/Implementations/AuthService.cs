using Azure.Core;
using Microsoft.Extensions.Options;
using TrainingCenter.Application.DTOs.Auth.Requests;
using TrainingCenter.Application.DTOs.Auth.Response;
using TrainingCenter.Application.Helpers.Models;
using TrainingCenter.Application.Interfaces.Persistence;
using TrainingCenter.Application.Interfaces.Security;
using TrainingCenter.Application.Services.Interfaces;
using TrainingCenter.Application.Validations;
using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Domain.Results;

namespace TrainingCenter.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _jwtTokenGenerator;
        private readonly Jwt _jwt;
        public AuthService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, ITokenGenerator jwtTokenGenerator, IOptions<Jwt> jwt)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _jwt = jwt?.Value ?? throw new ArgumentNullException(nameof(jwt));
        }
        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var errors = AuthValidation.LoginValidate(request);
            if (errors.Any()) return Result<AuthResponse>.FailureResult("Validation errors", errors, 400);

            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (user == null) return Result<AuthResponse>.FailureResult("Invalid credentials.", statusCode: 401);

            if (!user.IsActive) return Result<AuthResponse>.FailureResult("Login Failed.", "User is inactive", statusCode: 409);

            if (!_passwordHasher.Verify(request.Password, user.PasswordHashed))
                return Result<AuthResponse>.FailureResult("Invalid credentials.", statusCode: 401);


            var token = _jwtTokenGenerator.GenerateJwtToken(user, _jwt);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var record = new RefreshToken
            {
                Token = _jwtTokenGenerator.HashToken(refreshToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpiration),
                UserId = user.Id
            };

            await _unitOfWork.RefreshTokens.AddAsync(record);

            user.LastLoginAt = DateTime.UtcNow;
            await _unitOfWork.SaveAsync();

            return Result<AuthResponse>.SuccessResult(new AuthResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwt.AccessTokenExpiration),
            });
        }
        public async Task<Result<CurrentUserResponse>> RegisterAsync(RegisterRequest request, Role role = Role.Student)
        {
            var errors = AuthValidation.RegisterValidate(request);

            if (!Enum.IsDefined(role)) errors.Add("Role not defined.");

            if (errors.Any()) return Result<CurrentUserResponse>.FailureResult("Validation errors", errors, 400);


            var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (existingUser != null) return Result<CurrentUserResponse>.FailureResult("Conflict", "Email was taken by another user.", 409);

            User user = role switch
            {
                Role.Student => new Student(),
                Role.Instructor => new Instructor { Bio = "", Specialization = "" },
                Role.Admin => new Admin(),
                _ => throw new NotImplementedException()
            };

            user.FName = request.FirstName;
            user.LName = request.LastName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.PasswordHashed = _passwordHasher.Hash(request.Password);
            user.Role = role;
            user.IsActive = true;

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveAsync();

            return Result<CurrentUserResponse>.SuccessResult(new CurrentUserResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
            }, "User registered successfully", 201);
        }

        public async Task<Result<CurrentUserResponse>> GetCurrentUserAsync(int userId)
        {
            if (userId < 1)
                return Result<CurrentUserResponse>.FailureResult("Validation errors", "User ID must be a positive value.", 400);

            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return Result<CurrentUserResponse>.FailureResult("User not found.", statusCode: 401);

            if (!user.IsActive)
                return Result<CurrentUserResponse>.FailureResult("Account is inactive.", "User is inactive.", 401);

            return Result<CurrentUserResponse>.SuccessResult(new CurrentUserResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
            }, "Current user retrieved successfully.");
        }
        public async Task<Result> ChangePasswordAsync(int id, ChangePasswordRequest request)
        {
            var errors = new List<string>();
            if (id < 1) errors.Add("User ID must be a positive value.");
            if (string.IsNullOrWhiteSpace(request.OldPassword)) errors.Add("Old password is required.");
            if (string.IsNullOrWhiteSpace(request.NewPassword)) errors.Add("New password is required.");
            if (string.IsNullOrWhiteSpace(request.ConfirmNewPassword)) errors.Add("Confirm password is required.");
            else if (!string.IsNullOrWhiteSpace(request.NewPassword) && request.NewPassword != request.ConfirmNewPassword)
                errors.Add("New password and confirm password do not match.");
            if (!string.IsNullOrWhiteSpace(request.NewPassword) && request.NewPassword == request.OldPassword)
                errors.Add("New password must be different from the old password.");

            if (errors.Any()) return Result.FailureResult("Validation errors.", errors, 400);

            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return Result.FailureResult("User not found.", statusCode: 401);

            if (!user.IsActive) return Result.FailureResult("Cannot do this operation.", "User is inactive.", statusCode: 409);

            if (!_passwordHasher.Verify(request.OldPassword, user.PasswordHashed))
                return Result.FailureResult("Validation errors.", "Old password is incorrect.", statusCode: 400);

            user.PasswordHashed = _passwordHasher.Hash(request.NewPassword);
            await _unitOfWork.SaveAsync();
            return Result.SuccessResult("Password changed successfully.");
        }
        public async Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                return Result<RefreshTokenResponse>.FailureResult("Validation error.", "refresh token is required.", 400);

            var tokenHashed = _jwtTokenGenerator.HashToken(request.RefreshToken);

            var dbToken = await _unitOfWork.RefreshTokens.GetFirstOrDefaultAsync(x => x.Token == tokenHashed,"User");

            if (dbToken == null) return Result<RefreshTokenResponse>.FailureResult("Invalid refresh token.", "Invalid refresh token.", 401);
            if (dbToken.ExpiresAt <= DateTime.UtcNow) return Result<RefreshTokenResponse>.FailureResult("Invalid refresh token.", "Invalid refresh token.", 401);
            if (dbToken.RevokedAt != null) return Result<RefreshTokenResponse>.FailureResult("Invalid refresh token.", "Invalid refresh token.", 401);

            var user = dbToken.User;

            if (user == null || !user.IsActive)
                return Result<RefreshTokenResponse>.FailureResult("Invalid refresh token.", "Invalid refresh token.", 401);

            dbToken.RevokedAt = DateTime.UtcNow;

            var accessToken = _jwtTokenGenerator.GenerateJwtToken(user, _jwt);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            var record = new RefreshToken
            {
                Token = _jwtTokenGenerator.HashToken(refreshToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpiration),
                UserId = user.Id
            };
            await _unitOfWork.RefreshTokens.AddAsync(record);
            await _unitOfWork.SaveAsync();
            return Result<RefreshTokenResponse>.SuccessResult(new RefreshTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        public async Task<Result> LogoutAsync(int userId, string refreshToken)
        {
            var errors = new List<string>();
            if (string.IsNullOrEmpty(refreshToken)) errors.Add("refresh token is required.");
            if (userId < 1) errors.Add("User id must be a positive value.");
            if (errors.Any()) return Result.FailureResult("Validation error.", errors, 400);

            var tokenHashed = _jwtTokenGenerator.HashToken(refreshToken);

            var dbToken = await _unitOfWork.RefreshTokens.GetFirstOrDefaultAsync(x => x.Token == tokenHashed);

            if (dbToken == null || dbToken.UserId != userId) return Result.FailureResult("Invalid refresh token.", "Invalid refresh token.", 401);

            if (dbToken.RevokedAt != null) return Result.SuccessResult("logged out.");

            dbToken.RevokedAt = DateTime.UtcNow;
            await _unitOfWork.SaveAsync();
            return Result.SuccessResult("logged out.");
        }
    }
}