using Task_03___Training_Center_Database_API.Data;
using Task_03___Training_Center_Database_API.Services.Interfaces;

namespace Task_03___Training_Center_Database_API.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly ApplicationDbContext _context;
        public EnrollmentService(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
