using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Interfaces.Repositories;
using TrainingCenter.Infrastructure.Data;

namespace TrainingCenter.Infrastructure.Repositories
{
    public class StudentRepository: GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context) : base(context) { }

    }
}
