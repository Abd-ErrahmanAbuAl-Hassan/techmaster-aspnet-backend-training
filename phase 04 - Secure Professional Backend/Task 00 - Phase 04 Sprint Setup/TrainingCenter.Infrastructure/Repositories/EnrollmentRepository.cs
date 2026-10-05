using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Interfaces.Repositories;
using TrainingCenter.Infrastructure.Data;

namespace TrainingCenter.Infrastructure.Repositories
{
    public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(ApplicationDbContext context) : base(context) { }

    }
}
