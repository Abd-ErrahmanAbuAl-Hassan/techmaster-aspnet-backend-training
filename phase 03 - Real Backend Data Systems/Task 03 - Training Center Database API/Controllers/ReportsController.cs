using Microsoft.AspNetCore.Mvc;
using Task_03___Training_Center_Database_API.Services.Interfaces;

namespace Task_03___Training_Center_Database_API.Controllers
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
            var result = await _reportService.GetUnpaidEnrollmentsAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("track-capacity")]
        public async Task<IActionResult> GetTrackCapacity([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _reportService.GetTrackCapacityAsync(pageNumber, pageSize);
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
            var result = await _reportService.GetRevenueByTrackAsync(pageNumber, pageSize);
            return Ok(result);
        }
    }
}