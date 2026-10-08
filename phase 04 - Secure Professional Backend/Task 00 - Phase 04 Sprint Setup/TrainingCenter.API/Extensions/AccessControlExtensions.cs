using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainingCenter.Application.DTOs.Track.Responses;
using TrainingCenter.Application.Services.Interfaces;

namespace TrainingCenter.Controllers
{
    public static class AccessControlExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            return int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) && id > 0 ? id : null;
        }

        public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole("Admin");

        public static IActionResult ForbiddenResponse(this ControllerBase controller, string error) =>
            controller.StatusCode(StatusCodes.Status403Forbidden, new
            {
                Success = false,
                Message = "Access denied.",
                StatusCode = 403,
                Errors = new List<string> { error }
            });

        public static IActionResult FailureResponse(this ControllerBase controller, int? statusCode, object body) =>
            statusCode switch
            {
                400 => controller.BadRequest(body),
                401 => controller.Unauthorized(body),
                403 => controller.StatusCode(StatusCodes.Status403Forbidden, body),
                404 => controller.NotFound(body),
                409 => controller.Conflict(body),
                _ => controller.StatusCode(StatusCodes.Status500InternalServerError, body)
            };

        public static async Task<(TrackDetailsResponse? Track, IActionResult? Error)> EnsureTrackAccessAsync(
            this ControllerBase controller, ITrainingTrackService trackService, int trackId)
        {
            var result = await trackService.GetTrackByIdAsync(trackId);
            if (!result.Success)
                return (null, controller.FailureResponse(result.StatusCode, result));

            var track = result.Data!;
            if (controller.User.IsAdmin())
                return (track, null);

            var userId = controller.User.GetUserId();
            if (userId is null)
                return (null, controller.Unauthorized());

            if (track.Instructor.Id != userId)
                return (null, controller.ForbiddenResponse("You can only access tracks assigned to you."));

            return (track, null);
        }

        public static async Task<IActionResult?> EnsureEnrollmentAccessAsync(
            this ControllerBase controller, IEnrollmentService enrollmentService, int enrollmentId)
        {
            var result = await enrollmentService.GetEnrollmentByIdAsync(enrollmentId);
            if (!result.Success)
                return controller.FailureResponse(result.StatusCode, result);

            if (controller.User.IsAdmin())
                return null;

            var userId = controller.User.GetUserId();
            if (userId is null)
                return controller.Unauthorized();

            if (result.Data!.Student.Id != userId)
                return controller.ForbiddenResponse("You can only access your own enrollments.");

            return null;
        }
    }
}