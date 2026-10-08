using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Application.DTOs.Payment.Requests;
using TrainingCenter.Application.Services.Interfaces;
using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IEnrollmentService _enrollmentService;

        public PaymentsController(IPaymentService paymentService, IEnrollmentService enrollmentService)
        {
            _paymentService = paymentService;
            _enrollmentService = enrollmentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPayments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] PaymentStatus? status = null)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            if (endDate < startDate) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "EndDate must greater than or equal the StartDate." }
            });

            var result = await _paymentService.GetPaymentsAsync(pageNumber, pageSize, startDate, endDate, status);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpGet("my-payments")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyPayments()
        {
            var userId = User.GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _enrollmentService.GetEnrollmentsAsync(1, 50, studentId: userId.Value);

            if (!result.Success)
            {
                if (result.StatusCode == 404)
                    return Ok(new
                    {
                        Success = true,
                        Message = "No payments found.",
                        StatusCode = 200,
                        Data = new List<object>()
                    });

                return this.FailureResponse(result.StatusCode, result);
            }

            var payments = result.Data!.Items
                .SelectMany(e => e.Payments.Select(p => new
                {
                    EnrollmentId = e.Id,
                    TrackTitle = e.Track.Title,
                    Payment = p
                }))
                .OrderByDescending(x => x.Payment.PaymentDate)
                .ToList();

            return Ok(new
            {
                Success = true,
                Message = "Payments retrieved successfully.",
                StatusCode = 200,
                Data = payments
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            if (request.EnrollmentId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be positive." }
            });

            var result = await _paymentService.CreatePaymentAsync(request);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpGet("enrollment/{enrollmentId:int}")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> GetEnrollmentPayments(int enrollmentId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            if (enrollmentId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be positive." }
            });

            if (!User.IsAdmin())
            {
                var error = await this.EnsureEnrollmentAccessAsync(_enrollmentService, enrollmentId);
                if (error != null) return error;
            }

            var result = await _paymentService.GetEnrollmentPaymentsAsync(enrollmentId, pageNumber, pageSize);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] PaymentStatusUpdateRequest request)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be positive." }
            });

            var result = await _paymentService.UpdatePaymentStatusAsync(id, request.Status);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }
    }
}