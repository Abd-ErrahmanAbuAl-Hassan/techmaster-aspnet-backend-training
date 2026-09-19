using Microsoft.AspNetCore.Mvc;
using Task_04_Querying_Filtering_Reporting.Services.Interfaces;

namespace Task_04_Querying_Filtering_Reporting.Controllers
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
            return Ok(result);
        }

        [HttpGet("revenue-summary")]
        public async Task<IActionResult> GetRevenueSummary()
        {
            var result = await _reportService.GetRevenueSummaryAsync();
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
            return Ok(result);
        }
        [HttpGet("top-tracks")]
        public async Task<IActionResult> GetRevenueByTrack([FromQuery] int topCount = 1)
        {
            if (topCount < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "Count must be positive." }
            });

            var result = await _reportService.GetTopTrackAsync(topCount);
            return Ok(result);
        }
        [HttpGet("instructors-workload")]
        public async Task<IActionResult> GetInstructorsWorkload()
        {
            var result = await _reportService.GetInstructorWorkLoadAsync();
            return Ok(result);
        }
        [HttpGet("students-without-payments")]
        public async Task<IActionResult> GetStudentsPaymentLess()
        {
            var result = await _reportService.GetStudentsWithoutPaymentsAsync();
            return Ok(result);
        }
    }
}