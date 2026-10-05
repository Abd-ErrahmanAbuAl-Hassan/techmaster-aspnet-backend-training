using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Interfaces.Repositories;
using TrainingCenter.Infrastructure.Data;

namespace TrainingCenter.Infrastructure.Repositories
{
    public class AdminRepository: GenericRepository<Admin>, IAdminRepository
    {
        public AdminRepository(ApplicationDbContext context) : base(context) { }

    }
}
