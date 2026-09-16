using Task_03___Training_Center_Database_API.Data;
using Task_03___Training_Center_Database_API.Services.Interfaces;

namespace Task_03___Training_Center_Database_API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
