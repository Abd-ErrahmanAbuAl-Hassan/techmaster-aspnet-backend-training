using Task_03___Training_Center_Database_API.Data;
using Task_03___Training_Center_Database_API.Services.Interfaces;

namespace Task_03___Training_Center_Database_API.Services
{
    public class InstructorService : IInstructorService
    {
        private readonly ApplicationDbContext _context;
        public InstructorService(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
