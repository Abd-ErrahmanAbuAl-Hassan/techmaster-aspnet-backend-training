using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Task_04_Querying_Filtering_Reporting.DTOs.Requests;
using Task_04_Querying_Filtering_Reporting.Services.Interfaces;
using Task_04_Querying_Filtering_Reporting.Utilities.Enums;

namespace Task_04_Querying_Filtering_Reporting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPayments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] PaymentStatus? status = null)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            if (endDate < startDate) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "EndDate must greater than or equal the StartDate." }
            });
            var result = await _paymentService.GetPaymentsAsync(pageNumber, pageSize, startDate, endDate, status);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            if (request.EnrollmentId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be positive." }
            });

            var result = await _paymentService.CreatePaymentAsync(request);
            if (!result.Success)
            {
                if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }

        [HttpGet("enrollment/{enrollmentId}")]
        public async Task<IActionResult> GetEnrollmentPayments(int enrollmentId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            if (enrollmentId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be positive." }
            });

            var result = await _paymentService.GetEnrollmentPaymentsAsync(enrollmentId, pageNumber, pageSize);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] PaymentStatusUpdateRequest request)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be positive." }
            });

            var result = await _paymentService.UpdatePaymentStatusAsync(id, request.Status);
            if (!result.Success)
            {
                if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }
    }
}