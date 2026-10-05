using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Interfaces.Repositories;
using TrainingCenter.Infrastructure.Data;

namespace TrainingCenter.Infrastructure.Repositories
{
    public class UserRepository: GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
