using Microsoft.AspNetCore.Mvc;
using Task_05_Business_Rules_Data_Integrity.Services.Interfaces;

namespace Task_05_Business_Rules_Data_Integrity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("dashboard-summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var result = await _reportService.GetDashboardSummaryAsync();
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }

        [HttpGet("unpaid-enrollments")]
        public async Task<IActionResult> GetUnpaidEnrollments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            var result = await _reportService.GetUnpaidEnrollmentsAsync(pageNumber, pageSize);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }

        [HttpGet("track-capacity")]
        public async Task<IActionResult> GetTrackCapacity([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            var result = await _reportService.GetTrackCapacityAsync(pageNumber, pageSize);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }

        [HttpGet("tracks-with-available-seats")]
        public async Task<IActionResult> GetTracksWithAvailableSeats([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            var result = await _reportService.GetTracksWithAvailableSeatsAsync(pageNumber, pageSize);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }

        [HttpGet("revenue-summary")]
        public async Task<IActionResult> GetRevenueSummary()
        {
            var result = await _reportService.GetRevenueSummaryAsync();
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }

        [HttpGet("revenue-by-track")]
        public async Task<IActionResult> GetRevenueByTrack([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            var result = await _reportService.GetRevenueByTrackAsync(pageNumber, pageSize);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }
        [HttpGet("top-tracks")]
        public async Task<IActionResult> GetTopTracksAsync([FromQuery] int topCount = 1)
        {
            if (topCount < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "Count must be positive." }
            });

            var result = await _reportService.GetTopTrackAsync(topCount);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }
        [HttpGet("instructors-workload")]
        public async Task<IActionResult> GetInstructorsWorkload()
        {
            var result = await _reportService.GetInstructorWorkLoadAsync();
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }
        [HttpGet("students-without-payments")]
        public async Task<IActionResult> GetStudentsPaymentLess()
        {
            var result = await _reportService.GetStudentsWithoutPaymentsAsync();
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }
    }
}