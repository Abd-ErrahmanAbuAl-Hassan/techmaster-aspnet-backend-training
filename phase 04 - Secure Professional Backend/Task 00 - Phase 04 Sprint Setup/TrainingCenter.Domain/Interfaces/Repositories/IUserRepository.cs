using TrainingCenter.Domain.Entities;

namespace TrainingCenter.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        public Task<User?> GetByIdAsync(int id);
    }
}
