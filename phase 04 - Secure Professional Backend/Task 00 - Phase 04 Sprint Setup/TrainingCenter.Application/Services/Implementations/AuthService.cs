using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrainingCenter.Application.DTOs.Auth.Requests;
using TrainingCenter.Application.DTOs.Auth.Response;
using TrainingCenter.Application.Helpers;
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
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ITokenGenerator jwtTokenGenerator,
            IOptions<Jwt> jwt,
            ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _jwt = jwt?.Value ?? throw new ArgumentNullException(nameof(jwt));
            _logger = logger;
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                _logger.LogServiceCall("AuthService", "LoginAsync", request.Email);
                var errors = AuthValidation.LoginValidate(request);
                if (errors.Any())
                {
                    _logger.LogValidationError("Validation of login request fields",errors);
                    return Result<AuthResponse>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogResourceNotFound("User",request.Email);

                    return Result<AuthResponse>.UnauthorizedResult(
                        ResultMessages.Unauthorized.InvalidCredentials);
                }

                if (!user.IsActive)
                {
                    _logger.LogBusinessLogicError($"Login failed: User is inactive. Email: {request.Email}");
                    return Result<AuthResponse>.ConflictResult(
                        ResultMessages.Unauthorized.AccountInactive);
                }

                if (!_passwordHasher.Verify(request.Password, user.PasswordHashed))
                {
                    _logger.LogValidationError($"Verify password for Email: {request.Email}",["Password is wrong"]);

                    return Result<AuthResponse>.UnauthorizedResult(
                        ResultMessages.Unauthorized.InvalidCredentials);
                }

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

                _logger.LogInformation("Login successful. UserId: {UserId}, Email: {Email}", user.Id, user.Email);

                return Result<AuthResponse>.SuccessResult(new AuthResponse
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(_jwt.AccessTokenExpiration),
                },
                ResultMessages.Success.LoginSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogServiceError("AuthService", "LoginAuth", ex);
                return Result<AuthResponse>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<CurrentUserResponse>> RegisterAsync(RegisterRequest request, Role role = Role.Student)
        {
            try
            {
                _logger.LogServiceCall("AuthService", "RegisterAsync", new { request.Email, role });

                var errors = AuthValidation.RegisterValidate(request);

                if (!Enum.IsDefined(role))
                    errors.Add(string.Format(ResultMessages.Validation.EnumInvalid, "Role"));

                if (errors.Any())
                {
                    _logger.LogValidationError("Registration validation failed", errors);
                    return Result<CurrentUserResponse>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    _logger.LogBusinessLogicError($"Registration failed: Email already exists. Email: {request.Email}");
                    return Result<CurrentUserResponse>.ConflictResult(
                        ResultMessages.Conflict.DuplicateEmail);
                }

                User user = role switch
                {
                    Role.Student => new Student(),
                    Role.Instructor => new Instructor { Bio = "", Specialization = "" },
                    Role.Admin => new Admin(),
                    _ => throw new NotImplementedException("Role not implemented.")
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

                _logger.LogInformation("Registration successful. UserId: {UserId}, Email: {Email}, Role: {Role}",
                    user.Id, user.Email, role);

                return Result<CurrentUserResponse>.SuccessResult(
                    new CurrentUserResponse
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Role = user.Role,
                    },
                    ResultMessages.Success.RegistrationSuccess,
                    201);
            }
            catch (Exception ex)
            {
                _logger.LogServiceError("AuthService", "RegisterAsync", ex);
                return Result<CurrentUserResponse>.FailureResult(ResultMessages.ServerError.ProcessingError,
                                                                 ex.Message);
            }
        }

        public async Task<Result<CurrentUserResponse>> GetCurrentUserAsync(int userId)
        {
            try
            {
                _logger.LogServiceCall("AuthService", "GetCurrentUserAsync", userId);
                if (userId < 1)
                {
                    _logger.LogValidationError("user id validation", [$"Invalid user ID: {userId}"]);

                    return Result<CurrentUserResponse>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        new List<string> { string.Format(ResultMessages.Validation.PositiveNumber, "User ID") });
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogResourceNotFound("User", userId);
                    return Result<CurrentUserResponse>.NotFoundResult(
                        string.Format(ResultMessages.NotFound.ResourceNotFound, "User"));
                }

                if (!user.IsActive)
                {
                    _logger.LogBusinessLogicError($"User is inactive. UserId: {userId}");
                    return Result<CurrentUserResponse>.ConflictResult(
                        ResultMessages.Unauthorized.AccountInactive);
                }

                _logger.LogInformation("Successfully retrieved current user. UserId: {UserId}", userId);

                return Result<CurrentUserResponse>.SuccessResult(
                    new CurrentUserResponse
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Role = user.Role,
                    },
                    "Current user retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogServiceError("AuthService", "GetCurrentUserAsync", ex);
                return Result<CurrentUserResponse>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result> ChangePasswordAsync(int id, ChangePasswordRequest request)
        {
            try
            {
                _logger.LogServiceCall("AuthService", "ChangePasswordAsync", new { id , request});

                var errors = new List<string>();
                if (id < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "User ID"));
                if (string.IsNullOrWhiteSpace(request.OldPassword))
                    errors.Add(string.Format(ResultMessages.Validation.RequiredField, "Old password"));
                if (string.IsNullOrWhiteSpace(request.NewPassword))
                    errors.Add(string.Format(ResultMessages.Validation.RequiredField, "New password"));
                if (string.IsNullOrWhiteSpace(request.ConfirmNewPassword))
                    errors.Add(string.Format(ResultMessages.Validation.RequiredField, "Confirm password"));
                else if (!string.IsNullOrWhiteSpace(request.NewPassword) && request.NewPassword != request.ConfirmNewPassword)
                    errors.Add("New password and confirm password do not match.");
                if (!string.IsNullOrWhiteSpace(request.NewPassword) && request.NewPassword == request.OldPassword)
                    errors.Add("New password must be different from the old password.");

                if (errors.Any())
                {
                    _logger.LogValidationError("Password change.", errors);
                    return Result.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogResourceNotFound("User", id);
                    return Result.NotFoundResult(
                        string.Format(ResultMessages.NotFound.ResourceNotFound, "User"));
                }

                if (!user.IsActive)
                {
                    _logger.LogBusinessLogicError($"Cannot change password for inactive user. UserId: {id}");
                    return Result.ConflictResult(
                        ResultMessages.Unauthorized.AccountInactive);
                }

                if (!_passwordHasher.Verify(request.OldPassword, user.PasswordHashed))
                {
                    _logger.LogValidationError($"Password change. UserId: {id}", ["Old password is incorrect."]);

                    return Result.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        new List<string> { "Old password is incorrect." });
                }

                user.PasswordHashed = _passwordHasher.Hash(request.NewPassword);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Password changed successfully. UserId: {UserId}", id);

                return Result.SuccessResult(
                    ResultMessages.Success.PasswordChanged);
            }
            catch (Exception ex)
            {
                _logger.LogServiceError("AuthService", "ChangePasswordAsync", ex);

                return Result.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                _logger.LogServiceCall("AuthService", "RefreshTokenAsync", request);

                if (string.IsNullOrEmpty(request.RefreshToken))
                {
                    _logger.LogValidationError("Token refresh.", ["Refresh token is required."]);

                    return Result<RefreshTokenResponse>.ValidationErrorResult("Validation errors occurred. Please review the details below.", new List<string> { "Refresh token is required." });
                }

                var tokenHashed = _jwtTokenGenerator.HashToken(request.RefreshToken);

                var dbToken = await _unitOfWork.RefreshTokens.GetFirstOrDefaultAsync(x => x.Token == tokenHashed,"User");

                if (dbToken == null)
                {
                    _logger.LogResourceNotFound("Db Refresh Token", request.RefreshToken);

                    return Result<RefreshTokenResponse>.UnauthorizedResult(ResultMessages.Unauthorized.TokenInvalid);
                }

                if (dbToken.ExpiresAt <= DateTime.UtcNow)
                {
                    _logger.LogValidationError("token refresh expire date", ["Refresh token is expired."]);
                    return Result<RefreshTokenResponse>.UnauthorizedResult(ResultMessages.Unauthorized.TokenExpired);
                }

                if (dbToken.RevokedAt != null)
                {
                    _logger.LogBusinessLogicError("Token refresh failed: Refresh token has been revoked.");
                    return Result<RefreshTokenResponse>.UnauthorizedResult(ResultMessages.Unauthorized.TokenInvalid);
                }

                var user = dbToken.User;

                if (user == null)
                {
                    _logger.LogResourceNotFound("User",dbToken);
                    return Result<RefreshTokenResponse>.UnauthorizedResult(ResultMessages.Unauthorized.TokenInvalid);
                }

                if(!user.IsActive)
                {
                    _logger.LogBusinessLogicError($"Refresh token failed: User is inactive. UserId:{user.Id}");
                    return Result<RefreshTokenResponse>.UnauthorizedResult(ResultMessages.Unauthorized.AccountInactive);
                }
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

                _logger.LogInformation("Token refreshed successfully. UserId: {UserId}", user.Id);

                return Result<RefreshTokenResponse>.SuccessResult(
                    new RefreshTokenResponse
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    },
                    "Token refreshed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogServiceError("AuthService", "RefreshTokenAsync", ex);

                return Result<RefreshTokenResponse>.FailureResult(ResultMessages.ServerError.ProcessingError,ex.Message);
            }
        }

        public async Task<Result> LogoutAsync(int userId, string refreshToken)
        {
            try
            {
                _logger.LogServiceCall("AuthService", "LogoutAsync", new { userId, refreshToken });

                var errors = new List<string>();
                if (string.IsNullOrEmpty(refreshToken))
                    errors.Add("Refresh token is required.");
                if (userId < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "User ID"));

                if (errors.Any())
                {
                    _logger.LogValidationError("logout", errors);
                    return Result.ValidationErrorResult("Validation errors occurred. Please review the details below.",errors);
                }

                var tokenHashed = _jwtTokenGenerator.HashToken(refreshToken);

                var dbToken = await _unitOfWork.RefreshTokens.GetFirstOrDefaultAsync(x => x.Token == tokenHashed);

                if (dbToken == null || dbToken.UserId != userId)
                {
                    _logger.LogValidationError("logout", [$"Invalid refresh token. UserId: {userId}"]);
                    return Result.UnauthorizedResult(ResultMessages.Unauthorized.TokenInvalid);
                }

                if (dbToken.RevokedAt != null)
                {
                    _logger.LogInformation("Logout successful (token already revoked). UserId: {UserId}", userId);
                    return Result.SuccessResult(ResultMessages.Success.LogoutSuccess);
                }

                dbToken.RevokedAt = DateTime.UtcNow;
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Logout successful. UserId: {UserId}", userId);

                return Result.SuccessResult(ResultMessages.Success.LogoutSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogServiceError("AuthService", "LogoutAsync", ex);

                return Result.FailureResult(ResultMessages.ServerError.ProcessingError,ex.Message);
            }
        }
    }
}