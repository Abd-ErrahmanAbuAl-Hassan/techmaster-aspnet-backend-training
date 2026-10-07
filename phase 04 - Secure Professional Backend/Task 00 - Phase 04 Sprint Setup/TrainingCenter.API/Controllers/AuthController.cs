using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainingCenter.Application.DTOs.Auth.Requests;
using TrainingCenter.Application.Services.Interfaces;
using TrainingCenter.Domain.Enums;

namespace TrainingCenter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [Consumes("application/json")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(request);
            if (!result.Success)
                return result.StatusCode switch
                {
                    400 => BadRequest(result),
                    404 => NotFound(result),
                    401 => Unauthorized(result),
                    409 => Conflict(result),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result)
                };

            return Ok(result);

        }


        [HttpPost("register")]
        [Consumes("application/json")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.Role == Role.Admin) return BadRequest(new
            {
                Success = false,
                Message = "You can only register as student or instructor.",
                Error = "Invalid Role."
            });

            var result = await _authService.RegisterAsync(request, request.Role);
            if (!result.Success)
                return result.StatusCode switch
                {
                    400 => BadRequest(result),
                    404 => NotFound(result),
                    401 => Unauthorized(result),
                    409 => Conflict(result),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result)
                };

            return StatusCode(StatusCodes.Status201Created, result);

        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            if (!int.TryParse(GetUserId(), out int userId) || userId < 1) return Unauthorized();

            var result = await _authService.GetCurrentUserAsync(userId);
            if (!result.Success)
                return result.StatusCode switch
                {
                    400 => BadRequest(result),
                    401 => Unauthorized(result),
                    404 => NotFound(result),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result)
                };

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        [Consumes("application/json")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RefreshTokenAsync(request);
            if (!result.Success)
                return result.StatusCode switch
                {
                    400 => BadRequest(result),
                    404 => NotFound(result),
                    401 => Unauthorized(result),
                    409 => Conflict(result),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result)
                };

            return Ok(result);

        }

        [HttpPost("change-password")]
        [Authorize]
        [Consumes("application/json")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!int.TryParse(GetUserId(), out int userId) || userId < 1) return Unauthorized();

            var result = await _authService.ChangePasswordAsync(userId, request);
            if (!result.Success)
                return result.StatusCode switch
                {
                    400 => BadRequest(result),
                    401 => Unauthorized(result),
                    409 => Conflict(result),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result)
                };

            return Ok(result);

        }

        [HttpPost("logout")]
        [Authorize]
        [Consumes("application/json")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!int.TryParse(GetUserId(), out int userId) || userId < 1) return Unauthorized();

            var result = await _authService.LogoutAsync(userId, refreshToken);
            if (!result.Success)
                return result.StatusCode switch
                {
                    400 => BadRequest(result),
                    401 => Unauthorized(result),
                    409 => Conflict(result),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result)
                };

            return Ok(result);

        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}