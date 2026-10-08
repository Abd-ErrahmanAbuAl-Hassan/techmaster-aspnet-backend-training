using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainingCenter.Application.DTOs.Auth.Requests;
using TrainingCenter.Application.Services.Interfaces;
using TrainingCenter.Controllers;
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
                return this.FailureResponse(result.StatusCode, result);

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
                return this.FailureResponse(result.StatusCode, result);

            return StatusCode(StatusCodes.Status201Created, result);

        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            var result = await _authService.GetCurrentUserAsync(userId.Value);
            if (!result.Success)
                return this.FailureResponse(result.StatusCode, result);

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
                return this.FailureResponse(result.StatusCode, result);

            return Ok(result);

        }

        [HttpPost("change-password")]
        [Authorize]
        [Consumes("application/json")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.ChangePasswordAsync(userId.Value, request);
            if (!result.Success)
                return this.FailureResponse(result.StatusCode, result);

            return Ok(result);

        }

        [HttpPost("logout")]
        [Authorize]
        [Consumes("application/json")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            var result = await _authService.LogoutAsync(userId.Value, refreshToken);
            if (!result.Success)
                return this.FailureResponse(result.StatusCode, result);

            return Ok(result);

        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}